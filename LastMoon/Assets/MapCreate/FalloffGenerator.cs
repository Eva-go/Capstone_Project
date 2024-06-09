using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FalloffGenerator
{
    public static float[,] GenerateFalloffMap(int size)
    {
        float[,] map = new float[size, size];
        Vector2 center = new Vector2(size / 2f, size / 2f);
        float maxDistance = size / 2f;

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                float x = (i - center.x) / center.x;
                float y = (j - center.y) / center.y;

                float distance = Mathf.Sqrt(x * x + y * y);
                float scaledDistance = distance / (maxDistance/239f);

                map[i, j] = 1 - Evaluate(scaledDistance);  // Invert the evaluation result
            }
        }
        return map;
    }

    static float Evaluate(float value)
    {
        float a = 3;
        float b = 1.5f;

        return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));
    }
}
