using UnityEngine;
using Intel.RealSense;
public class GetDistanceTest : MonoBehaviour
{
    public Terrain terrain;
    public RsDevice rsDevice;


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
                            if(currHeight > 0.9f)
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
                            // (using a vector3, since it can be lerped &amp; normalized)
                            // Normalise x/y coordinates to range 0-1 

                            float y_01 = (float)y / (float)dfHeight;
                            float x_01 = (float)x / (float)dfWidth;

                            // Setup an array to record the mix of texture weights at this point
                            float[] splatWeights = new float[4];
                            
                            if(x == 100 && y == 100)
                            {
                                Debug.Log(height);
                                Debug.Log(Map[100,100]);
                            }

                       
                            if(height < 1)
                            {
                                splatWeights[0] = 1;
                            }

                            if (height >= 1 && height < 2)
                            {
                                splatWeights[1] = 1;
                            }
                            if (height >= 2f && height < 2.5f)
                            {
                                var diff = height - 2f;
                                splatWeights[1] = 0.5f - diff;
                                splatWeights[2] = 0.5f + diff;
                            }

                            if (height >= 2.5f && height < 3.5f)
                            {
                                splatWeights[2] = 1;
                            }
                            if (height >= 3.5f && height < 4)
                            {
                                var diff = height - 3.5f;
                                splatWeights[2] = 0.5f - diff;
                                splatWeights[3] = 0.5f + diff;
                            }
                          
                            if (height >= 4)
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
                                // N

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