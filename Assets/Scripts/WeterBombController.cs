using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WaterBombController : MonoBehaviour
{
    [Header("WaterBomb")]
    public GameObject waterBombPrefab;  // 폭탄 프리팹
    public KeyCode inputKey = KeyCode.Space;  // 폭탄 설치 키
    public float explosionDelay = 3f; // 폭발 지연 시간
    public int waterBombAmout = 1;  // 폭탄 설치 가능 개수
    private int waterBombsRemaining;  // 남은 폭탄 개수
    
    [Header("Explosion")]
    public ExplosionController explosionPrefeb;
    public LayerMask explosionLayer;
    public float explosionDuration = 1f;
    public int explosionRadius = 1;
    
    public GameObject explosionEffect;
    public LayerMask explosionMask;
    public Tilemap tilemap;  // 타일맵 참조

    private void OnEnable()
    {
        waterBombsRemaining = waterBombAmout;
    }

    private void Update()
    {
        if (waterBombsRemaining > 0 && Input.GetKeyDown(inputKey))
        {
            StartCoroutine(PlaceWaterBomb());
        }
    }
    
    /**
     * 폭탄을 설치하는 코루틴 함수
     * 폭탄을 설치하고 폭발 효과를 생성한다.
     * 폭발 효과는 폭발 지연 시간 후에 생성된다.
     * 폭발 효과는 폭발 반경만큼 폭발한다.
     * 폭발 효과는 네 방향으로 폭발한다.
     * 폭탄 설치 후 남은 폭탄 개수를 감소시킨다.
     * 폭탄 설치 후 폭발 효과가 생성되면 폭탄을 제거한다.
     * 폭탄 설치 후 폭발 효과가 생성되면 폭발 효과를 제거한다.
     */
    private IEnumerator PlaceWaterBomb()
    {
        Vector2 position = transform.position;
        
        position.x = Mathf.Round(position.x);
        position.y = Mathf.Round(position.y);

        GameObject waterBomb = Instantiate(waterBombPrefab, position, Quaternion.identity);
        waterBombsRemaining--;
        
        yield return new WaitForSeconds(explosionDelay);
        
        position = waterBomb.transform.position;
        position.x=  Mathf.Round(position.x);
        position.y = Mathf.Round(position.y);
        
        ExplosionController explosion = Instantiate(explosionPrefeb, position, Quaternion.identity);
        explosion.SetActiveRenderer(explosion.start);
        explosion.DestroyAfter(explosionDuration);
        Destroy(explosion.gameObject, explosionDuration);
        
        Explode(position, Vector2.up, explosionRadius);
        Explode(position, Vector2.down, explosionRadius);
        Explode(position, Vector2.left, explosionRadius);
        Explode(position, Vector2.right, explosionRadius);
        
        Destroy(waterBomb);
        waterBombsRemaining++;
    }

    /**
     * 폭발 효과를 생성하는 함수
     * @param position 폭발 위치
     * @param direction 폭발 방향
     * @param length 폭발 길이
     */
    private void Explode(Vector2 position, Vector2 direction, int length)
    {
        if (length <= 0)
        {
            return;
        }

        position += direction;
        
        // 폭발 효과와 벽이 충돌한 경우
        if (Physics2D.OverlapBox(position, Vector2.one / 2f, 0f, explosionLayer))
        {
            return;
        }
        
        ExplosionController explosion = Instantiate(explosionPrefeb, position, Quaternion.identity);
        explosion.SetActiveRenderer(length > 1 ? explosion.middle : explosion.end);
        explosion.SetDirection(direction);
        explosion.DestroyAfter(explosionDuration);
        Destroy(explosion.gameObject, explosionDuration);
        
        Explode(position, direction, length - 1);
    }

    /**
     * 폭발 효과와 충돌한 경우 호출되는 함수
     * 폭발 효과와 충돌한 타일을 제거한다.
     * 폭발 효과와 충돌한 오브젝트를 제거한다.
     */
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("WaterBomb"))
        {
            other.isTrigger = false;
        }
    }
}
