using UnityEngine;

public class ExplosionController : MonoBehaviour
{
    public AnimatedSpriteRenderer start;
    public AnimatedSpriteRenderer middle;
    public AnimatedSpriteRenderer end;
    
    public void SetActiveRenderer(AnimatedSpriteRenderer explosionRenderer)
    {
        start.enabled = explosionRenderer == start;
        middle.enabled = explosionRenderer == middle;
        end.enabled = explosionRenderer == end;
    }

    public void SetDirection(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x);
        transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.forward);
    }
    
    public void DestroyAfter(float sec)
    {
        Destroy(gameObject, sec);
    }
}