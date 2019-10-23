using UnityEngine;
using Intel.RealSense;
public class Handler : MonoBehaviour
{
    public Terrain terrain;
    public int depth = 40;
    public int width = 640;
    public int height = 480;

    public float scale = 20f;

    public float xOffset = 10;
    public float yOffset = 10;



    public bool scroll;
    public float scrollSpeed;


    private float[] vertices;
    private Pipeline pipe;
    private PointCloud pc;


    private void Start()
    {
        Debug.Log("START");
        //realsense 1d array
        pipe = new Pipeline();
        pc = new PointCloud();

        pipe.Start();

        using (var frames = pipe.WaitForFrames())
        using (var depth = frames.DepthFrame)
        using (var points = pc.Process(depth).As<Points>())
        {
            // CopyVertices is extensible, any of these will do:
            vertices = new float[points.Count * 3];
            // var vertices = new Intel.RealSense.Math.Vertex[points.Count];
            // var vertices = new UnityEngine.Vector3[points.Count];
            // var vertices = new System.Numerics.Vector3[points.Count]; // SIMD
            // var vertices = new GlmSharp.vec3[points.Count];
            //  var vertices = new byte[points.Count * 3 * sizeof(float)];
            points.CopyVertices(vertices);


        }
        Debug.Log(vertices.Length + " : is the length of the array");



        xOffset = Random.Range(0f, 9999f);
        yOffset = Random.Range(0f, 9999f);

        scroll = false;
        scrollSpeed = 0;


        terrain.terrainData = GenerateTerrain(terrain.terrainData);




    }
    void LateUpdate()
    {

        if (scroll)
            xOffset += Time.deltaTime * scrollSpeed;

    }
    TerrainData GenerateTerrain(TerrainData tData)
    {
        tData.heightmapResolution = width + 1;
        tData.size = new Vector3(width, depth, height);
        // tData.SetHeights(0, 0, GenerateHeights());
        tData.SetHeights(0, 0, Convert1DTo2DArray());
        return tData;
    }
    float[,] GenerateHeights()
    {
        float[,] heights = new float[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                heights[x, y] = CalculateHeight(x, y);
                //later will become the depth value from the real sense camera

            }
        }
        return heights;
    }

    float CalculateHeight(int x, int y)
    {

        float xCoord = (float)x / width * scale + xOffset;
        float yCoord = (float)y / height * scale + yOffset;

        return Mathf.PerlinNoise(xCoord, yCoord);


    }

    float[,] Convert1DTo2DArray()
    {
       
        int width = 640;
        int height = 480;
        var Map = new float[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Map[j, i] = vertices[j + i];
            }
        }
        return Map;
        
    }
}