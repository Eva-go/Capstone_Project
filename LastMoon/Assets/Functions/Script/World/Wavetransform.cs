using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wavetransform : MonoBehaviour
{
    private float downwave = -10f;
    public float waveY;

    //매테리얼 인스턴스 값 변경
    private MaterialPropertyBlock propertyBlock;

    public float waveHeightTime = 0;
    public float waveStrength = 0;
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
        waveStrength = Mathf.Lerp(GameValue.Round - 1.0f, 2.0f + GameValue.Round, normalizedTime);

        //매테리얼 인스턴스 값 변경
        propertyBlock.SetFloat("_Strength", waveStrength);
        propertyBlock.SetFloat("_waveHeightTime", waveHeightTime);
        gameObject.GetComponent<MeshRenderer>().SetPropertyBlock(propertyBlock);
        //

        if (RenderSettings.skybox.HasProperty("_Strength"))
            RenderSettings.skybox.SetFloat("_Strength", waveStrength);
    }

    public float GetWaveHeight(float _x, float _z, float _strength, float _time_scale = 0.1f)
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
