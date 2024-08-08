using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewPoiController : MonoBehaviour
{
    
    public Material defaultMaterial; // �⺻ ����
    public Material collisionMaterial; // �浹 �� ����� ����

    private Renderer objectRenderer;
    private bool isColliding = false;

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer != null)
        {
            objectRenderer.material = defaultMaterial;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        ChangeMaterial(true);
    }

    void OnTriggerEnter(Collider other)
    {
        ChangeMaterial(true);
    }

    void OnCollisionExit(Collision collision)
    {
        ChangeMaterial(false);
    }

    void OnTriggerExit(Collider other)
    {
        ChangeMaterial(false);
    }

    private void ChangeMaterial(bool colliding)
    {
        if (objectRenderer != null)
        {
            if (colliding)
            {
                objectRenderer.material = collisionMaterial;
                isColliding = true;
            }
            else
            {
                objectRenderer.material = defaultMaterial;
                isColliding = false;
            }
        }
    }

    public bool IsColliding()
    {
        return isColliding;
    }
}