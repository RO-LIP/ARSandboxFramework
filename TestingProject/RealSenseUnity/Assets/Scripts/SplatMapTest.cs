using UnityEngine;
using Intel.RealSense;
public class SplatMapTest : MonoBehaviour
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
                    for (int y = 0; y < depthFrame.Width; y++)
                    {
                        float prevHeight = 0;
                        for (int x = 0; x < depthFrame.Height; x++)
                        {
                            float currHeight = 1.2f - depthFrame.GetDistance(y, x);
                            if (currHeight > 0.9f)
                            {
                                Map[x, y] = prevHeight;
                            }
                            else
                            {
                                Map[x, y] = prevHeight = 1.2f - depthFrame.GetDistance(y, x);
                            }
                        }
                    }

                    terrain.terrainData.heightmapResolution = dfWidth + 1;
                    terrain.terrainData.size = new Vector3(dfWidth, 100, dfHeight);
                    terrain.terrainData.SetHeights(0, 0, Map);

                    float[,,] splatmapData = new float[terrain.terrainData.alphamapWidth, terrain.terrainData.alphamapHeight, terrain.terrainData.alphamapLayers];
                    for (int y = 0; y < terrain.terrainData.alphamapHeight; y++)
                    {
                        for (int x = 0; x < terrain.terrainData.alphamapWidth; x++)
                        {
                            // read the height at this location
                            float height = terrain.terrainData.GetHeight(x, y);
                            // determine the mix of textures 1, 2 &amp; 3 to use 
                            // (using a vector3, since it can be lerped &amp; normalized)
                            Vector3 splat = new Vector3(0, 1, 0);
                            if (height > 0.2)
                            {
                                splat = Vector3.Lerp(splat, new Vector3(0, 0, 1), (height - 0.5f) * 2);
                            }
                            else
                            {
                                splat = Vector3.Lerp(splat, new Vector3(1, 0, 0), height * 2);
                            }
                            // now assign the values to the correct location in the array
                            splat.Normalize();
                            splatmapData[x, y, 0] = splat.x;
                            splatmapData[x, y, 1] = splat.y;
                            splatmapData[x, y, 2] = splat.z;
                        }

                        terrain.terrainData.SetAlphamaps(0, 0, splatmapData);
                    }
                }
            }
        }
    }
    private void Start()
    {
        rsDevice.OnNewSample += OnNewSample;
    }
}