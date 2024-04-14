using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandGenerator : MonoBehaviour
{
    public int mapWidth = 1000;
    public int mapHeight = 1000;
    public float scale = 50.0f;
    public float islandThreshold = 100f;

    void Start()
    {
        GenerateIslandMap();
    }

    void GenerateIslandMap()
    {
        // 새로운 텍스처 생성
        Texture2D islandMap = new Texture2D(mapWidth, mapHeight);

        // 각 픽셀에 대해 펄린 노이즈 값을 계산하여 지형 생성
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                // Normalize coordinates to fit into Perlin noise space
                float xCoord = (float)x / mapWidth * scale;
                float yCoord = (float)y / mapHeight * scale;

                // Calculate Perlin noise value
                float perlinValue = Mathf.PerlinNoise(xCoord, yCoord);

                // Calculate distance from center
                float distanceFromCenter = Vector2.Distance(new Vector2(x, y), new Vector2(mapWidth / 2, mapHeight / 2)) / (mapWidth / 2);

                // Apply circular gradient
                perlinValue *= Mathf.SmoothStep(0.0f, 1.0f, 1.0f - distanceFromCenter);

                // Determine if the pixel represents land or water based on threshold
                Color pixelColor = (perlinValue > islandThreshold) ? Color.white : Color.blue;

                // Set pixel color
                islandMap.SetPixel(x, y, pixelColor);

            }
        }
        islandMap.Apply();
        islandMap.wrapMode = TextureWrapMode.Clamp;

        // Assign the texture to the material
        GetComponent<Renderer>().material.mainTexture = islandMap;
    }
}
