using UnityEngine;
using Intel.RealSense;
public class RealSenseTest : MonoBehaviour
{
    public Terrain terrain;

    public RsDevice rsDevice;

    Texture2D resultTexture;
    int dfWidth = 0, dfHeight = 0, dfStride = 0;
    byte[] data;
    bool isInitTexture = false;

    void OnNewSample(Frame frame)
    {
        using (var frameSet = frame.AsFrameSet())
        {
            using (var depthFrame = frameSet.DepthFrame as DepthFrame)
            {
                if (depthFrame != null)
                {
                    dfWidth = depthFrame.Width;
                    dfHeight = depthFrame.Height;
                    dfStride = depthFrame.Stride;

                    data = data ?? new byte[dfStride * dfHeight];
                    depthFrame.CopyTo(data);
                 
                    float[] arr = new float[dfHeight * dfWidth];
                    var x = 0;
                    for (int i = 0; i < data.Length; i++)
                    {
                        if(i % 2 != 0)
                        {
                            int f = data[i - 1] | data[i] << 8;
                            arr[x] = (float)f / 6000; //range of f is approx. 400 - 6000
                            x++;
                        }
                      
                    }

                    var Map = new float[dfHeight, dfWidth];
                    for (int i = 0; i < dfHeight; i++)
                    {
                        for (int j = 0; j < dfWidth; j++)
                        {
                            Map[i, j] =  arr[j + i];
                        }
                    }
                
                    terrain.terrainData.heightmapResolution = dfWidth + 1;
                    terrain.terrainData.size = new Vector3(dfWidth, 100, dfHeight);
                    terrain.terrainData.SetHeights(0, 0, Map);
                }
            }
        }
    }
    private void Start()
    {
        rsDevice.OnNewSample += OnNewSample;
    }
}