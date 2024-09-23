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

    private float normalizedTidalCycle;

    private float Tidelevel = 0;
    private float Lunascale = 12.5f;
    private float LunaYPos = 0.375f;

    //노드 스폰관련 변수
    public GameObject PlaceNodeObject;
    public PlaceNode placeNode;
    void Start()
    {
        //downwave = 17.5f / (GameValue.MaxRound - 1);
        downwave = 5.0f;
        waveY = -22.5f;

        propertyBlock = new MaterialPropertyBlock();

        LowTide = true;

        if (RenderSettings.skybox.HasProperty("_Moon_Scale"))
            RenderSettings.skybox.SetFloat("_Moon_Scale", 12.5f);
        if (RenderSettings.skybox.HasProperty("_Moon_Position_Y"))
            RenderSettings.skybox.SetFloat("_Moon_Position_Y", 0.375f);

        //노드 리스폰
        PlaceNodeObject = GameObject.Find("PlaceNode");

        // 만약 오브젝트를 찾지 못했다면 오류 메시지를 출력
        if (PlaceNodeObject == null)
        {
            Debug.LogError("PlaceNode 씬에서 찾을 수 없습니다.");
        }
        else
        {
            // PlaceNodeObject 안에 있는 PlaceNode 컴포넌트 가져오기
            placeNode = PlaceNodeObject.GetComponent<PlaceNode>();
        }


    }

    // Update is called once per frame
    void Update()
    {

        if (LowTide != GameValue.LowTide)
        {
            //waveY = -22.5f + ((GameValue.TideCycle) * downwave);

            LowTide = GameValue.LowTide;

            normalizedTidalCycle = 1.0f - (((float)GameValue.TideCycle - 1.0f) / 10.0f);
            Lunascale = Mathf.Lerp(2.5f, 12.5f, normalizedTidalCycle);
            LunaYPos = Mathf.Lerp(0.375f, 0.5f, normalizedTidalCycle);

            if (RenderSettings.skybox.HasProperty("_Moon_Scale"))
                RenderSettings.skybox.SetFloat("_Moon_Scale", Lunascale);
            if (RenderSettings.skybox.HasProperty("_Moon_Position_Y"))
                RenderSettings.skybox.SetFloat("_Moon_Position_Y", LunaYPos);
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
                Tidelevel = -0.75f;
                //placeNode.nodespawns();
            }
            else
            {
                SeaLevel = Mathf.Lerp(0.0f, (float)GameValue.TideCycle / 2.0f + 1.0f, normalizedTidalChange);
                waveStrength = Mathf.Lerp(1.0f + (float)GameValue.TideCycle, (float)GameValue.TideCycle - 1.0f, normalizedTidalChange);
                Tidelevel = 0.0f;
                placeNode.All_node_Destory();
            }
        }
        else
        {
            if (LowTide)
            {
                SeaLevel = Mathf.Lerp(-(float)GameValue.TideCycle - 2.0f, 0.0f, normalizedTime);
                Tidelevel = Mathf.Lerp(-0.75f, 0.0f, normalizedTime);
            }
            else
            {
                SeaLevel = Mathf.Lerp((float)GameValue.TideCycle / 2.0f + 1.0f, 0.0f, normalizedTime);
                Tidelevel = Mathf.Lerp(0.0f, 0.75f, normalizedTime);
            }
            waveStrength = Mathf.Lerp((float)GameValue.TideCycle - 1.0f, 1.0f + (float)GameValue.TideCycle, normalizedTime);
        }

        waveY = (SeaLevel * downwave) - 10f;

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
        if (RenderSettings.skybox.HasProperty("_Wave_Height"))
            RenderSettings.skybox.SetFloat("_Wave_Height", waveY);
        if (RenderSettings.skybox.HasProperty("_Moon_Position_X"))
            RenderSettings.skybox.SetFloat("_Moon_Position_X", Tidelevel);
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
