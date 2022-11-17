using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGenerator : MonoBehaviour
{
    [Header("Parameters")]
    public int noiseSampleSize;
    public float scale;
    public float maxHeight = 1.0f;
    public int textureResolution = 1;

    [HideInInspector]
    public Vector2 offset;

    [Header("Terrain Types")]
    public TerrainType[] heightTerrainTypes;
    public TerrainType[] heatTerrainTypes;

    [Header("Waves")]
    public Wave[] waves;
    public Wave[] heatWaves;

    [Header("Curves")]
    public AnimationCurve heightCurve;

    private MeshRenderer tileMeshRenderer;
    private MeshFilter tileMeshFilter;
    private MeshCollider tileMeshColleder;

    private MeshGenerator meshGenerator;
    private MapGenerator mapGenerator;


    private void Start()
    {
        // get the tile components
        tileMeshColleder = GetComponent<MeshCollider>();
        tileMeshRenderer = GetComponent<MeshRenderer>();
        tileMeshFilter = GetComponent<MeshFilter>();
        meshGenerator = GetComponent<MeshGenerator>();
        mapGenerator = FindObjectOfType<MapGenerator>();

        GenerateTile();
    }

    void GenerateTile()
    {
        // generate a new height map
        float[,] heightMap = NoiseGenerator.GenerateNoiseMap(noiseSampleSize, scale, waves, offset);

        float[,] hdHeightMap = NoiseGenerator.GenerateNoiseMap(noiseSampleSize -1, scale, waves, offset, textureResolution);

        Vector3[] verts = tileMeshFilter.mesh.vertices;

        for (int x = 0; x < noiseSampleSize; x++)
        {
            for (int z = 0; z < noiseSampleSize; z++)
            {
                int index = (x * noiseSampleSize) + z;

                verts[index].y = heightCurve.Evaluate(heightMap[x, z]) * maxHeight;
            }
        }

        // appy these to the mesh filter to see the new mesh
        tileMeshFilter.mesh.vertices = verts;
        tileMeshFilter.mesh.RecalculateBounds();
        tileMeshFilter.mesh.RecalculateNormals();

        // update mesh collider
        tileMeshColleder.sharedMesh = tileMeshFilter.mesh;

        // create the height map texture
        Texture2D heightMapTexture = TextureBuilder.BuildTexture(hdHeightMap, heightTerrainTypes);

        // apply the height map texture to the MeshRenderer
        //tileMeshRenderer.material.mainTexture = heightMapTexture;

        float[,] heatMap = GenerateHeatMap(heightMap);
        tileMeshRenderer.material.mainTexture = TextureBuilder.BuildTexture(heatMap, heatTerrainTypes);
    }

    float[,] GenerateHeatMap(float[,] heightMap)
    {
        float[,] uniformHeatMap = NoiseGenerator.GenerateUniformNoiseMap(noiseSampleSize, transform.position.z * (noiseSampleSize / meshGenerator.xSize), (noiseSampleSize / 2 * mapGenerator.numX) + 1);
        float[,] randomHeatMap = NoiseGenerator.GenerateNoiseMap(noiseSampleSize, scale, heatWaves, offset);

        float[,] heatMap = new float[noiseSampleSize, noiseSampleSize];

        for (int x = 0; x < noiseSampleSize; x++)
        {
            for (int z = 0; z < noiseSampleSize; z++)
            {
                heatMap[x, z] = randomHeatMap[x, z] * uniformHeatMap[x, z];
                heatMap[x, z] += 0.5f * heatMap[x, z]; 

                heatMap[x, z] = Mathf.Clamp(heatMap[x, z], 0.0f, 0.99f);
            }
            
        }
        return heatMap;

    }
}

[System.Serializable]
public class TerrainType
{
    [Range(0.0f, 1.0f)]
    public float threshold;
    public Gradient colorGradient;
}
