using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wavetransform : MonoBehaviour
{
    private float downwave = -10f;
    public float waveY;
    private Vector3 wave;

    private MaterialPropertyBlock propertyBlock;

    float waveHeightTime = 0;
    public float currentTime = 0;
    public float normalizedTime;

    void Start()
    {
        downwave = -30 / GameValue.MaxRound;
        wave = new Vector3(gameObject.transform.position.x, -15f+(GameValue.Round * downwave), gameObject.transform.position.z);
        waveY = -15f + (GameValue.Round * downwave);

        propertyBlock = new MaterialPropertyBlock();
       
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 currentPosition = gameObject.transform.position;
        currentPosition.y = waveY;
        transform.position = currentPosition;
        waveHeightTime += Time.deltaTime;

        //정규화
        normalizedTime = 1.0f - (GameValue.WaveTimer / 600.0f);
        //보간
        currentTime = Mathf.Lerp(0.0f, 15.0f, normalizedTime);

        propertyBlock.SetFloat("_Strength", currentTime);
        propertyBlock.SetFloat("_waveHeightTime", waveHeightTime);
        gameObject.GetComponent<MeshRenderer>().SetPropertyBlock(propertyBlock);

        if (RenderSettings.skybox.HasProperty("_Strength"))
            RenderSettings.skybox.SetFloat("_Strength", currentTime);
    }

    public float GetWaveHeight(float _x, float _z, float _strength, float _time_scale = 1)
    {
        float Wavelength;
        if (_strength > 1) Wavelength = 1 / _strength;
        else Wavelength = 1;
    
        float WaveTime = waveHeightTime * _time_scale * _strength;
        float _tx, _tz;
        _tx = (_x + WaveTime) * 2 * Mathf.PI;
        _tz = (_z - (WaveTime * 0.25f)) * 2 * Mathf.PI;

        return Mathf.Sin((_tx - _tz + 1) * 0.5f * Wavelength) * _strength + Mathf.Sin(_tz * 0.25f * Wavelength) * _strength;
}
}
