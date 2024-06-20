using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoiAnimator : MonoBehaviour
{
    public Animator animator;
    private bool ani;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ani = PlayerController.Poi;
        if (ani)
        {
            animator.SetBool("isActvie", ani);
        }
        else
            animator.SetBool("isActvie", ani);
    }
}
