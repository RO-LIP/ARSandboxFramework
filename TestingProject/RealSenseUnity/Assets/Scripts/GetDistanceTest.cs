using UnityEngine;
using Intel.RealSense;
public class GetDistanceTest : MonoBehaviour
{
    public Terrain terrain;
    public RsDevice rsDevice;

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
                if (depthFrame != null)
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

                            // SPLATTIN
                            float height = Map[x,y] * 10;
                            
                            // determine the mix of textures 1, 2 &amp; 3 to use 
                            // (using a vector3, since it can be lerped and normalized)
                            // Normalise x/y coordinates to range 0-1 

                            float y_01 = (float)y / (float)dfHeight;
                            float x_01 = (float)x / (float)dfWidth;

                            // Setup an array to record the mix of texture weights at this point
                            float[] splatWeights = new float[4];

                            if(height < texture0Cutoff)
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
                    terrain.terrainData.heightmapResolution = dfWidth;
                    terrain.terrainData.size = new Vector3(dfWidth, 100, dfHeight);
                    terrain.terrainData.SetHeightsDelayLOD(0, 0, Map);
                    terrain.terrainData.SetAlphamaps(0, 0, splatmapData);
                }
            }
        }
    }

    private void Start()
    {
        rsDevice.OnNewSample += OnNewSample;
    }
}