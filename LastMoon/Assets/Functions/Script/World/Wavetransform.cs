using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wavetransform : MonoBehaviour
{
    private float downwave = -1.5f;
    public float waveY;
    private Vector3 wave;

    private MaterialPropertyBlock propertyBlock;

    public float currentTime = 0;
    GameTimer gameTimer;

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

        currentTime = gameTimer.currentTime;
        propertyBlock.SetFloat("_Strength", currentTime);
        gameObject.GetComponent<MeshRenderer>().SetPropertyBlock(propertyBlock);
    }
}
