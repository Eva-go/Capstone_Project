using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveShader : MonoBehaviour
{
    private MaterialPropertyBlock propertyBlock;
    void Start()
    {
        propertyBlock = new MaterialPropertyBlock();
    }
    void Update()
    {
        float waveMat_Strength, waveMat_Time;

        waveMat_Strength = gameObject.GetComponentInParent<Wavetransform>().waveStrength;
        waveMat_Time = gameObject.GetComponentInParent<Wavetransform>().waveHeightTime;

        propertyBlock.SetFloat("_Strength", waveMat_Strength);
        propertyBlock.SetFloat("_waveHeightTime", waveMat_Time);
        gameObject.GetComponent<MeshRenderer>().SetPropertyBlock(propertyBlock);
    }
}
