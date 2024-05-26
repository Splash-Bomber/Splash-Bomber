using UnityEngine;
using UnityEngine.Tilemaps;

public class Bomb : MonoBehaviour
{
    public float explosionDelay = 2f;
    public GameObject explosionEffect;
    public LayerMask explosionMask;
    public Tilemap tilemap;  // 타일맵 참조

    void Start()
    {
        Invoke("Explode", explosionDelay);
    }

    void Explode()
    {
        Vector3 bombPosition = transform.position;

        // 타일맵 그리드의 셀 좌표로 변환
        Vector3Int cellPosition = tilemap.WorldToCell(bombPosition);

        // 상하좌우 4칸에 폭발 이펙트 생성
        CreateExplosion(cellPosition + new Vector3Int(1, 0, 0), Quaternion.Euler(0, 0, 0));
        CreateExplosion(cellPosition + new Vector3Int(-1, 0, 0), Quaternion.Euler(0, 0, 180));
        CreateExplosion(cellPosition + new Vector3Int(0, 1, 0), Quaternion.Euler(0, 0, 90));
        CreateExplosion(cellPosition + new Vector3Int(0, -1, 0), Quaternion.Euler(0, 0, -90));

        // 폭탄 오브젝트 삭제
        Destroy(gameObject);
    }

    void CreateExplosion(Vector3Int cellPosition, Quaternion rotation)
    {
        // 타일맵의 셀 좌표로 월드 좌표 변환
        Vector3 explosionPosition = tilemap.GetCellCenterWorld(cellPosition);

        // 폭발 이펙트가 겹치는지 체크
        if (Physics2D.OverlapCircle(explosionPosition, 0.1f, explosionMask) == null)
        {
            GameObject explosion = Instantiate(explosionEffect, explosionPosition, rotation);

            // 타일 크기를 기준으로 폭발 이펙트 크기 조정
            Vector3 tileSize = tilemap.cellSize;

            // 폭발 이펙트 프리팹의 원본 크기를 가져와 타일 크기에 맞게 스케일 조정
            SpriteRenderer spriteRenderer = explosion.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                Vector3 spriteSize = spriteRenderer.bounds.size;
                explosion.transform.localScale = new Vector3(tileSize.x / spriteSize.x, tileSize.y / spriteSize.y, 1);
            }
        }
    }
}
