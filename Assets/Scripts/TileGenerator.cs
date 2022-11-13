using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGenerator : MonoBehaviour
{
    [Header("Parameters")]
    public int noiseSampleSize;
    public float scale;

    private MeshRenderer tileMeshRenderer;
    private MeshFilter tileMeshFilter;
    private MeshCollider tileMeshColleder;

    private void Start()
    {
        // get the tile components
        tileMeshColleder = GetComponent<MeshCollider>();
        tileMeshRenderer = GetComponent<MeshRenderer>();
        tileMeshFilter = GetComponent<MeshFilter>();

        GenerateTile();
    }

    void GenerateTile()
    {
        float[,] heightMap = NoiseGenerator.GenerateNoiseMap(noiseSampleSize, scale);

        Texture2D heightMapTexture = TextureBuilder.BuildTexture(heightMap);

        tileMeshRenderer.material.mainTexture = heightMapTexture;
    }
}
