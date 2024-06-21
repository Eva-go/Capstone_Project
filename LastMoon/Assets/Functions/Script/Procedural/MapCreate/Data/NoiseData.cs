using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class NoiseData : UpdatableData
{
    public Noise.NormalizedMode normalizedMode;

    public float noiseScale1;
    public float noiseScale2;

    [Range(0, 10)]
    public int octaves1;
    [Range(0, 10)]
    public int octaves2;


    [Range(0, 1)]
    public float persistance;
    public float lacunarity;

    protected override void OnValidate()
    {
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }
        if (octaves1 < 0)
        {
            octaves1 = 0;
        }
        if (octaves2 < 0)
        {
            octaves2 = 0;
        }
        base.OnValidate();
    }
}
