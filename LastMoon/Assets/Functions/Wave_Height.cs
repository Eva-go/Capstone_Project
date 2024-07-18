using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave_Height : MonoBehaviour
{
    float offset;
    private void Update()
    {
        offset += Time.deltaTime;
    }
    public float GetWaveHeight(float _x, float _z, float _strength, float _time_scale = 1)
    {
        float Wavelength = 1;
        if (_strength > 1) Wavelength = 1 / _strength;
        else Wavelength = 1;

        float WaveTime = offset * _time_scale * _strength;

        return Mathf.Sin(((_x - _z + 1) * 0.5f + WaveTime) * Wavelength) * _strength + Mathf.Sin((_z - WaveTime) * 0.25f * Wavelength) * _strength;
    }
}
