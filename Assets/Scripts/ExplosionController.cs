using UnityEngine;

public class ExplosionController : MonoBehaviour
{
    public float explosionDuration = 0.5f;

    void Start()
    {
        Destroy(gameObject, explosionDuration);
    }
}