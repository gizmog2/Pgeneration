using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeBuilder : MonoBehaviour
{
    public BiomeRow[] biomeRows;

    public static BiomeBuilder instance;

    private void Awake()
    {
        instance= this;
    }

    public Texture2D BuildTexture(TerrainType[,] heatMapType, TerrainType[,] moistureMapTypes)
    {
        int size = heatMapType.GetLength(0);
        Color[] pixels = new Color[size * size];

        for (int x = 0; x < size; x++)
        {
            for (int z = 0; z < size; z++)
            {
                int index = (x * size) + z;

                int heatMapIndex = heatMapType[x, z].index;
                int moistureMapIndex = moistureMapTypes[x, z].index;

                Biome biome = biomeRows[moistureMapIndex].biomes[heatMapIndex];

                pixels[index] = biome.color;
            }
        }

        Texture2D texture = new Texture2D(size, size);
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Bilinear;
        texture.SetPixels(pixels);
        texture.Apply();

        return texture;
    }

    public Biome GetBiome(TerrainType heatTerrainType, TerrainType moistureTerrainType)
    {
        return biomeRows[moistureTerrainType.index].biomes[heatTerrainType.index];
    }
}

[System.Serializable]
public class BiomeRow
{
    public Biome[] biomes;
}

[System.Serializable]
public class Biome
{
    public string name;
    public Color color;
}
