using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wavetransform : MonoBehaviour
{
    private float downwave = -10f;
    public float waveY;

    private MaterialPropertyBlock propertyBlock;

    float waveHeightTime = 0;
    public float currentTime = 0;
    public float normalizedTime;

    void Start()
    {
        downwave = -17.5f / (GameValue.MaxRound - 1);
        waveY = -22.5f + ((GameValue.Round - 1) * downwave);

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
        normalizedTime = 1.0f - (GameValue.WaveTimer / GameValue.WaveTimerMax);
        //보간
        currentTime = Mathf.Lerp(0.0f, 2.0f + GameValue.Round, normalizedTime);

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
        _tx = (_x + WaveTime) * Mathf.PI * 2;
        _tz = (_z + (WaveTime * 0.25f)) * Mathf.PI * -2;

        return (Mathf.Sin((_tx - _tz + 1) * 0.5f * Wavelength) * _strength) + (Mathf.Sin(_tz * 0.25f * Wavelength) * _strength);
    }
}
