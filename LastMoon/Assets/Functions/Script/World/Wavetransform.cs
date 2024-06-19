using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wavetransform : MonoBehaviour
{
    private float downwave = -1.5f;

    void Start()
    {
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, -15f+(GameValue.Round * downwave), gameObject.transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
