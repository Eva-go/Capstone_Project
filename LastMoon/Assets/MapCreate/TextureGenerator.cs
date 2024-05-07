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
                // ���� �׶���Ʈ�� �����Ͽ� ������ ���� �������� ��ȯ
                float normalizedHeight = heightMap[x, y];
                colourMap[y * width + x] = EvaluateGradient(normalizedHeight);
            }
        }
        return TextureFromColourMap(colourMap, width, height);

    }

    private static Color EvaluateGradient(float normalizedHeight)
    {
        // ���� �׶���Ʈ �߽���
        Vector2 center = new Vector2(0.5f, 0.5f);
        // ���� ���� ��ġ
        Vector2 point = new Vector2(normalizedHeight, normalizedHeight);
        // �߽ɿ����� �Ÿ�
        float distanceFromCenter = Vector2.Distance(point, center);

        // �׶���Ʈ ����
        Color color = Color.white;
        if (distanceFromCenter > 0.5f)
        {
            // �׶���Ʈ�� �ܺ� ������ ������
            color = Color.black;
        }
        else
        {
            // �׶���Ʈ�� ���� ������ ������ ���� �����Ͽ� ���� ����
            float noiseValue = Mathf.PerlinNoise(point.x * 10, point.y * 10);
            color = new Color(noiseValue, noiseValue, noiseValue);
        }

        return color;
    }

}
