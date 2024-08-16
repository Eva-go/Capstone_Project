using UnityEngine;

public class EndPoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PooledObject")) // Ensure this tag is used for pooled objects
        {
            GameObject obj = other.gameObject;
            ObjectPooler.ReturnToPool(obj); // Return object to pool
        }
    }
}