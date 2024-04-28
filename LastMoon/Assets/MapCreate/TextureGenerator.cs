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
                colourMap[y * width + x] = Color.Lerp(Color.black, Color.white, heightMap[x, y]);
            }
        }
        return TextureFromColourMap(colourMap, width, height);

    }
    public static Texture2D CreateIslandGradientTexture(int width, int height, Color innerColor, Color outerColor)
    {
        Texture2D texture = new Texture2D(width, height);

        float centerX = width / 2f;
        float centerY = height / 2f;
        float maxRadius = Mathf.Min(width, height) / 2f;

        Color[] pixels = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float distanceToCenter = Vector2.Distance(new Vector2(x, y), new Vector2(centerX, centerY));
                float normalizedDistance = distanceToCenter / maxRadius;

                // 중앙에 가까울수록 높은 값을 갖도록 조절
                float heightValue = 1f - Mathf.Clamp01(normalizedDistance * normalizedDistance);

                Color color = Color.Lerp(innerColor, outerColor, heightValue);
                pixels[y * width + x] = color;
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();

        return texture;
    }

}
