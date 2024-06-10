using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureGenerator : MonoBehaviour
{
    public static Texture2D TextureFromColourMap(Color[] colourMap, int width, int height)
    {
        if (colourMap == null || colourMap.Length != width * height)
        {
            Debug.LogError("Invalid color map or dimensions.");
            return null;
        }
        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels (colourMap);
        texture.Apply ();
        return texture;
    }

    public static Texture2D TextureFromHeightMap(float[,] heightMap) 
    {
        if (heightMap == null || heightMap.Length == 0)
        {
            Debug.LogError("Invalid height map.");
            return null;
        }

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
}
