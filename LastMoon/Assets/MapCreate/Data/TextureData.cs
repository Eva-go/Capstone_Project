using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class TextureData : UpdatableData
{
    public Color[] baseColours;
    [Range(0,1)]
    public float[] baseStartHeights;
    [Range(0, 1)]
    public float[] baseBlends;

    float saveMinHeight;
    float saveMaxHeight;

    public void ApplyToMaterial(Material material)
    {
        material.SetInt("baseColoursCount", baseColours.Length);
        material.SetColorArray("baseColours", baseColours);
        material.SetFloatArray("baseStartHeights", baseStartHeights);
        material.SetFloatArray("baseBlends", baseBlends);

        UpdateMeshHeights(material, saveMinHeight, saveMaxHeight);
    }
    public void UpdateMeshHeights(Material material, float minHeight, float maxHeight)
    {
        saveMinHeight = minHeight;
        saveMaxHeight = maxHeight;

        material.SetFloat("minHeight", minHeight);
        material.SetFloat("maxHeight", maxHeight);
    }
}
