using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wavetransform : MonoBehaviour
{
    private float downwave = -10f;
    public float waveY;
    private Vector3 wave;

    private MaterialPropertyBlock propertyBlock;

    public float currentTime = 0;
    public float normalizedTime;

    void Start()
    {
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

        //정규화
        normalizedTime = 1.0f - (GameValue.WaveTimer / 600.0f);
        //보간
        currentTime = Mathf.Lerp(0.0f, 3.0f, normalizedTime);


        propertyBlock.SetFloat("_Strength", currentTime);
        gameObject.GetComponent<MeshRenderer>().SetPropertyBlock(propertyBlock);
    }
}
