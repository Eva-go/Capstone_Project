using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wavetransform : MonoBehaviour
{
    private float downwave = -10f;
    public float waveY;

    //매테리얼 인스턴스 값 변경
    private MaterialPropertyBlock propertyBlock;

    private bool LowTide;
    //private int TideCycle;
    //private float TideProgress;
    private float SeaLevel;

    public float waveHeightTime = 0;
    public float waveStrength = 0;
    public float normalizedTime;

    public float normalizedTidalChange;

    void Start()
    {
        //downwave = 17.5f / (GameValue.MaxRound - 1);
        downwave = 10.0f;
        waveY = -22.5f;

        propertyBlock = new MaterialPropertyBlock();

        LowTide = true;
}

    // Update is called once per frame
    void Update()
    {
        if (LowTide != GameValue.LowTide)
        {
            //waveY = -22.5f + ((GameValue.TideCycle) * downwave);

            LowTide = GameValue.LowTide;
        }

        //보간

        //정규화
        normalizedTime = 1.0f - (GameValue.WaveTimer / GameValue.WaveTimerMax);
        //정규화
        normalizedTidalChange = 1.0f - (GameValue.TideChangeProgress / 5.0f);

        if (GameValue.TideChange)
        {

            if (LowTide)
            {
                SeaLevel = Mathf.Lerp(0.0f, -(float)GameValue.TideCycle - 2.0f, normalizedTidalChange);
                waveStrength = Mathf.Lerp((float)GameValue.TideCycle, (float)GameValue.TideCycle - 1.0f, normalizedTidalChange);
            }
            else
            {
                SeaLevel = Mathf.Lerp(0.0f, (float)GameValue.TideCycle / 2.0f + 1.0f, normalizedTidalChange);
                waveStrength = Mathf.Lerp(1.0f + (float)GameValue.TideCycle, (float)GameValue.TideCycle - 1.0f, normalizedTidalChange);
            }
        }
        else
        {
            if (LowTide)
            {
                SeaLevel = Mathf.Lerp(-(float)GameValue.TideCycle - 1.0f, 0.0f, normalizedTime);
            }
            else
            {
                SeaLevel = Mathf.Lerp((float)GameValue.TideCycle / 2.0f + 1.0f, 0.0f, normalizedTime);
            }

            waveStrength = Mathf.Lerp((float)GameValue.TideCycle - 1.0f, 1.0f + (float)GameValue.TideCycle, normalizedTime);
        }

        waveY = (SeaLevel * downwave) - 2.5f;

        Vector3 currentPosition = gameObject.transform.position;
        currentPosition.y = waveY;
        transform.position = currentPosition;
        waveHeightTime += Time.deltaTime;


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
