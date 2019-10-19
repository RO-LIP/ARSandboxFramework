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
                    for(int y = 0; y < depthFrame.Width; y++)
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
                        }
                    }

                    terrain.terrainData.heightmapResolution = dfWidth;
                    terrain.terrainData.size = new Vector3(dfWidth, 100, dfHeight);
                    terrain.terrainData.SetHeightsDelayLOD(0, 0, Map);
                }
            }
        }
    }
    private void Start()
    {
        rsDevice.OnNewSample += OnNewSample;
    }
}