using UnityEngine;
using Intel.RealSense;
using System.Threading;
using System.Collections.Concurrent;

public class GetDistanceTest : MonoBehaviour
{
    public Terrain terrain;
    public RsProcessingPipe processingPipe;
    ConcurrentQueue<float[,]> heightMapQueue = new ConcurrentQueue<float[,]>();
    ConcurrentQueue<float[,]> tempheightMapQueue = new ConcurrentQueue<float[,]>();
    ConcurrentQueue<float[,,]> splatMapQueue = new ConcurrentQueue<float[,,]>();
    DepthFrame currentDepthFrame;
    int width;
    int height;
    float[,] Map;


    public float noiseCutoff;

    public float texture0Cutoff;
    public float texture1Cutoff;
    public float texture2Cutoff;

    public float textureTransitionSize;

    public void SetTexture0Cutoff(float newValue)
    {
        texture0Cutoff = newValue;
    }

    public void SetTexture1Cutoff(float newValue)
    {
        texture1Cutoff = newValue;
    }

    public void SetTexture2Cutoff(float newValue)
    {
        texture2Cutoff = newValue;
    }

    public void SetTextureTransitionSize(float newValue)
    {
        textureTransitionSize = newValue;
    }

    void OnNewSample(Frame frame)
    {
        using (var frameSet = frame.AsFrameSet())
        {
            using (var depthFrame = frameSet.DepthFrame as DepthFrame)
            {
                if (depthFrame != null 
                    && heightMapQueue.IsEmpty && tempheightMapQueue.IsEmpty
                    )
                {
                   
                    int dfWidth = depthFrame.Width;
                    int dfHeight = depthFrame.Height;
                    float[,] Map = new float[dfHeight, dfWidth];
                    float[,,] splatmapData = new float[dfHeight, dfWidth, 4];
                    for (int y = 0; y < depthFrame.Width; y++)
                    {
                        float prevHeight = 0;
                        for(int x = 0; x < depthFrame.Height; x++)
                        {
                            float currHeight = 1.2f - depthFrame.GetDistance(y, x);
                            if(currHeight > noiseCutoff)
                            {
                                Map[x, y] = prevHeight;
                            }
                            else
                            {
                                Map[x, y] = prevHeight  = 1.2f - depthFrame.GetDistance(y,x);
                            }

                        }
                    }

                    height = dfHeight;
                    width = dfWidth;
                    if (!tempheightMapQueue.IsEmpty)
                    {
                        var clear = new float[width, width];
                        tempheightMapQueue.TryDequeue(out clear);
                    }
                    if (tempheightMapQueue.IsEmpty)
                    {
                        tempheightMapQueue.Enqueue(Map);
                    }
                    if (!heightMapQueue.IsEmpty)
                    {
                        var clear = new float[width, width];
                        heightMapQueue.TryDequeue(out clear);
                    }
                    if (heightMapQueue.IsEmpty)
                    {
                        heightMapQueue.Enqueue(Map);
                    }

                 
                }
            }
        }
    }


    public void generateSplatmapData()
    {
        while (true)
        {
            if (tempheightMapQueue.IsEmpty)
            {
                Thread.Sleep(10);
            }
            else
            {
                float[,] heightmap = new float[width, width];
                tempheightMapQueue.TryDequeue(out heightmap);
                float[,,] splatmapData = new float[width, width, 4];
                for (int y = 0; y < width; y++)
                {
                    for (int x = 0; x < height; x++)
                    {
                        float height = heightmap[x, y] * 10;
                        

                        // determine the mix of textures 1, 2 &amp; 3 to use 
                        // (using a vector3, since it can be lerped and normalized)
                        // Normalise x/y coordinates to range 0-1 

                        float y_01 = (float)y / (float)height;
                        float x_01 = (float)x / (float)width;

                        // Setup an array to record the mix of texture weights at this point
                        float[] splatWeights = new float[4];

                        if (height < texture0Cutoff)
                        {
                            splatWeights[0] = 1;
                        }

                        if (height >= texture0Cutoff && height < texture1Cutoff)
                        {
                            splatWeights[1] = 1;
                        }
                        if (height >= texture1Cutoff && height < texture1Cutoff + textureTransitionSize)
                        {
                            var diff = height - texture1Cutoff;
                            splatWeights[1] = 0.5f - diff;
                            splatWeights[2] = 0.5f + diff;
                        }

                        if (height >= texture1Cutoff + textureTransitionSize && height < texture2Cutoff)
                        {
                            splatWeights[2] = 1;
                        }
                        if (height >= texture2Cutoff && height < texture2Cutoff + textureTransitionSize)
                        {
                            var diff = height - texture2Cutoff;
                            splatWeights[2] = 0.5f - diff;
                            splatWeights[3] = 0.5f + diff;
                        }

                        if (height >= texture2Cutoff + textureTransitionSize)
                        {
                            splatWeights[3] = 1;
                        }

                        // Sum of all textures weights must add to 1, so calculate normalization factor from sum of weights
                        float z = splatWeights[0] + splatWeights[1] + splatWeights[2] + splatWeights[3];

                        // Loop through each terrain texture
                        for (int i = 0; i < 4; i++)
                        {
                            // Normalize so that sum of all texture weights = 1
                            splatWeights[i] /= z;

                            // Assign this point to the splatmap array
                            splatmapData[x, y, i] = splatWeights[i];
                        }
                    }
                }
                if (!splatMapQueue.IsEmpty)
                {
                    var clear = new float[width, width, 4];
                    splatMapQueue.TryDequeue(out clear);
                }
                if (splatMapQueue.IsEmpty)
                {
                    splatMapQueue.Enqueue(splatmapData);
                }

            }
        }
    }


    private void Start()
    {
        processingPipe.OnNewSample += OnNewSample;
        Thread thread = new Thread(generateSplatmapData);
        thread.Start();

    }

    private void Update()
    {
        if (!heightMapQueue.IsEmpty)
        {
            float[,] heightmap = new float[width, width];
            heightMapQueue.TryDequeue(out heightmap);

            terrain.terrainData.heightmapResolution = width;
            terrain.terrainData.size = new Vector3(width, 100, width);
            terrain.terrainData.SetHeights(0, 0, heightmap);
        }

        if (!splatMapQueue.IsEmpty)
        {
            float[,,] splatmapData = new float[width, width, 4];
            splatMapQueue.TryDequeue(out splatmapData);
            terrain.terrainData.SetAlphamaps(1, 1, splatmapData);
        }


    }



}
