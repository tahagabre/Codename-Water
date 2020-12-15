using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public int depth = 10;
    public int width = 400;
    public int height = 400;
    public float scale = 4f;
    public float offsetX = 100f;
    public float offsetY = 100f;
    


    void Start()
    {
        Generate();
    }



    void Update()
    {
        Terrain terrain = GetComponent<Terrain>();
        terrain.terrainData = GenerateTerrain(terrain.terrainData);
        
    }

    TerrainData GenerateTerrain(TerrainData terrainData)
    {
        terrainData.heightmapResolution = width + 1;

        terrainData.size = new Vector3(width, depth, height);
        terrainData.SetHeights(0, 0, GenerateHeights());
        return terrainData;
    }

    float[,] GenerateHeights()
    {
        float[,] heights = new float[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                heights[x, y] = CalculateHeight(x, y);
            }
        }
        return heights;
    }

    float CalculateHeight (int x, int y)
    {
        float xCoord = (float)x / width * scale + offsetX;
        float yCoord = (float)y / height * scale + offsetY;

        return Mathf.PerlinNoise(xCoord, yCoord);
    }

    public void Generate()
    {
        offsetX = Random.Range(0f, 10f);
        offsetY = Random.Range(0f, 10f);
    }

    public void AdjustOffsetX(float newOffsetX)
    {
        offsetX += newOffsetX;
    }

    public void AdjustOffsetY(float newOffsetY)
    {
        offsetY += newOffsetY;
    }

    public void AddDepth(int newDepthAdd)
    {
        depth += newDepthAdd;
    }

    public void ReduceDepth(int newDepthReduce)
    {
        depth -= newDepthReduce;
    }

    public void AddScale(float newScaleAdd)
    {
        scale += newScaleAdd;
    }

    public void ReduceScale(float newScaleReduce)
    {
        scale -= newScaleReduce;
    }
}
