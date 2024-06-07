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
    public Tilemap groundTiles; // 바닥 타일맵
    public TileBase newTile; // 새로운 타일

    [Header("Audio")]
    public AudioClip placeSound; // 설치 사운드
    public AudioClip explosionSound; // 폭발 사운드
    private AudioSource audioSource;

    public delegate void TileChanged(TileBase newTile, Vector3Int position);
    public static event TileChanged OnTileChanged;

    private void OnEnable()
    {
        waterBombsRemaining = waterBombAmout;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Update()
    {
        if (waterBombsRemaining > 0 && Input.GetKeyDown(inputKey))
        {
            StartCoroutine(PlaceWaterBomb());
        }
    }

    private IEnumerator PlaceWaterBomb()
    {
        Vector2 position = transform.position;

        position.x = Mathf.Round(position.x);
        position.y = Mathf.Round(position.y);

        GameObject waterBomb = Instantiate(waterBombPrefab, position, Quaternion.identity);
        waterBombsRemaining--;

        // 설치 사운드 재생
        PlaySound(placeSound);

        yield return new WaitForSeconds(explosionDelay);

        position = waterBomb.transform.position;
        position.x = Mathf.Round(position.x);
        position.y = Mathf.Round(position.y);

        // 폭탄이 설치된 자리의 타일 색 변경
        ChangeGroundTile(position);

        ExplosionController explosion = Instantiate(explosionPrefeb, position, Quaternion.identity);
        explosion.SetActiveRenderer(explosion.start);
        explosion.DestroyAfter(explosionDuration);

        // 폭발 시 사운드 재생
        PlaySound(explosionSound);

        Explode(position, Vector2.up, explosionRadius);
        Explode(position, Vector2.down, explosionRadius);
        Explode(position, Vector2.left, explosionRadius);
        Explode(position, Vector2.right, explosionRadius);

        Destroy(explosion.gameObject, explosionDuration);
        Destroy(waterBomb);
        waterBombsRemaining++;
    }

    private void Explode(Vector2 startPosition, Vector2 direction, int length)
    {
        Vector2 position = startPosition;
        for (int i = 0; i < length; i++)
        {
            position += direction;

            // 폭발 효과 생성
            ExplosionController explosion = Instantiate(explosionPrefeb, position, Quaternion.identity);
            explosion.SetActiveRenderer(i < length - 1 ? explosion.middle : explosion.end);
            explosion.SetDirection(direction);
            explosion.DestroyAfter(explosionDuration);
            Destroy(explosion.gameObject, explosionDuration);

            // 타일을 새로운 타일로 교체
            ChangeGroundTile(position);

            // 충돌 레이어 확인하여 폭발 중단
            if (Physics2D.OverlapBox(position, Vector2.one / 2f, 0f, explosionLayer))
            {
                ClearDestructible(position);
                break;
            }
        }
    }

    private void ChangeGroundTile(Vector2 position)
    {
        Vector3Int cell = groundTiles.WorldToCell(position);
        groundTiles.SetTile(cell, newTile);
        OnTileChanged?.Invoke(newTile, cell);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("WaterBomb"))
        {
            other.isTrigger = false;
        }
    }

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

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
