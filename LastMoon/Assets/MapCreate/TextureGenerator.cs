using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureGenerator : MonoBehaviour
{
    public static Texture2D TextureFromColourMap(Color[] colourMap, int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels (colourMap);
        texture.Apply ();
        return texture;
    }

    public static Texture2D TextureFromHeightMap(float[,] heightMap) 
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        Color[] colourMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // 원형 그라디언트를 적용하여 노이즈 값을 색상으로 변환
                float normalizedHeight = heightMap[x, y];
                colourMap[y * width + x] = EvaluateGradient(normalizedHeight);
            }
        }
        return TextureFromColourMap(colourMap, width, height);

    }

    private static Color EvaluateGradient(float normalizedHeight)
    {
        // 원형 그라디언트 중심점
        Vector2 center = new Vector2(0.5f, 0.5f);
        // 현재 점의 위치
        Vector2 point = new Vector2(normalizedHeight, normalizedHeight);
        // 중심에서의 거리
        float distanceFromCenter = Vector2.Distance(point, center);

        // 그라디언트 색상
        Color color = Color.white;
        if (distanceFromCenter > 0.5f)
        {
            // 그라디언트의 외부 영역은 검은색
            color = Color.black;
        }
        else
        {
            // 그라디언트의 내부 영역은 노이즈 값을 적용하여 색상 결정
            float noiseValue = Mathf.PerlinNoise(point.x * 10, point.y * 10);
            color = new Color(noiseValue, noiseValue, noiseValue);
        }

        return color;
    }

}
