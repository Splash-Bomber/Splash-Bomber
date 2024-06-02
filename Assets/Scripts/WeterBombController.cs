using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WaterBombController : MonoBehaviour
{
    [Header("WaterBomb")]
    public GameObject waterBombPrefab; // 물폭탄 prefab
    public KeyCode inputKey = KeyCode.Space; // 물폭탄 놓는 키
    public float explosionDelay = 3f; // 폭발까지의 시간
    public int waterBombAmout = 1; // 물폭탄 갯수
    private int waterBombsRemaining; // 남은 물폭탄 갯수
    
    [Header("Explosion")]
    public ExplosionController explosionPrefeb; // 폭발 prefab
    public LayerMask explosionLayer; // 폭발 레이어
    public float explosionDuration = 1f; // 폭발 지속 시간
    public int explosionRadius = 1; // 폭발 범위
    
    [Header("Destructible")]
    public Tilemap destructibleTiles; // 파괴 가능한 타일
    public Destructible destructiblePrefab; // 파괴 가능한 prefab
    
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
    
    /*
     * 물폭탄을 놓는 함수
     * 물폭탄을 놓고 일정 시간 후 폭발하며, 폭발 범위 내의 타일을 파괴한다.
     * 폭발은 4방향으로 진행되며, 폭발 범위가 0이 될 때까지 반복한다.
     * 폭발은 폭발 prefab을 생성하여 구현한다.
     * 폭발 prefab은 폭발 시작, 중간, 끝을 구분하여 애니메이션을 재생한다.
     * 폭발 prefab은 폭발이 끝난 후 일정 시간 후 파괴된다.
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
    
    /*
     * 폭발을 구현하는 함수
     * 폭발은 폭발 prefab을 생성하여 구현한다.
     * 폭발 prefab은 폭발 시작, 중간, 끝을 구분하여 애니메이션을 재생한다.
     * 폭발 prefab은 폭발이 끝난 후 일정 시간 후 파괴된다.
     * 폭발은 폭발 범위가 0이 될 때까지 반복한다.
     * 폭발은 폭발 레이어에 있는 타일을 파괴한다.
     */
    private void Explode(Vector2 position, Vector2 direction, int length)
    {
        if (length <= 0)
        {
            return;
        }

        position += direction;
        
        // 폭발 범위 내에 파괴 가능한 타일이 있는 경우
        if (Physics2D.OverlapBox(position, Vector2.one / 2f, 0f, explosionLayer))
        {
            ClearDestructible(position);
            return;
        }
        
        ExplosionController explosion = Instantiate(explosionPrefeb, position, Quaternion.identity);
        explosion.SetActiveRenderer(length > 1 ? explosion.middle : explosion.end);
        explosion.SetDirection(direction);
        explosion.DestroyAfter(explosionDuration);
        Destroy(explosion.gameObject, explosionDuration);
        
        Explode(position, direction, length - 1);
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("WaterBomb"))
        {
            other.isTrigger = false;
        }
    }
    
    /*
     * 파괴 가능한 타일을 파괴하는 함수
     * 파괴 가능한 타일을 파괴하고 파괴 가능한 prefab을 생성한다.
     * 파괴 가능한 타일은 destructibleTiles에 저장되어 있다.
     * 파괴 가능한 prefab은 destructiblePrefab을 사용한다.
     * 파괴 가능한 타일은 destructibleTiles에서 제거한다.
     * 파괴 가능한 prefab은 position 위치에 생성한다.
     * 파괴 가능한 prefab은 파괴 가능한 타일의 크기와 같다.
     * 파괴 가능한 prefab은 파괴 가능한 타일의 위치에 생성된다.
     */
    private void ClearDestructible(Vector2 position)
    {
        Vector3Int cell = destructibleTiles.WorldToCell(position);
        TileBase tile = destructibleTiles.GetTile(cell);

        if (tile != null)
        {
            Instantiate(destructiblePrefab, position, Quaternion.identity);
            destructibleTiles.SetTile(cell, null);
        }
    }

    public void AddWaterBomb()
    {
        waterBombAmout++;
        waterBombsRemaining++;
    }
}
