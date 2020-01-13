using UnityEngine;
using Intel.RealSense;
using System.Threading;
using System.Collections.Concurrent;

public class GetDistanceForShader: MonoBehaviour
{
    public Terrain terrain;
    public RsProcessingPipe processingPipe;
    public RsDevice rsDevice;
    ConcurrentQueue<float[,]> heightMapQueue = new ConcurrentQueue<float[,]>();
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
                    && heightMapQueue.IsEmpty)
                {

                    int dfWidth = depthFrame.Width;
                    int dfHeight = depthFrame.Height;
                    float[,] Map = new float[dfHeight, dfWidth];
                    float[,,] splatmapData = new float[dfHeight, dfWidth, 4];
                    for (int y = 0; y < depthFrame.Width; y++)
                    {
                        float prevHeight = 0;
                        for (int x = 0; x < depthFrame.Height; x++)
                        {
                            float currHeight = 1.2f - depthFrame.GetDistance(y, x);
                            if (currHeight > noiseCutoff)
                            {
                                Map[x, y] = prevHeight;
                            }
                            else
                            {
                                Map[x, y] = prevHeight = 1.2f - depthFrame.GetDistance(y, x);
                            }

                        }
                    }

                    height = dfHeight;
                    width = dfWidth;
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





    private void Start()
    {
        processingPipe.OnNewSample += OnNewSample;

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



    }



}

