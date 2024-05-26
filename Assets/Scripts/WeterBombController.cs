using UnityEngine;
using UnityEngine.Tilemaps;

public class Bomb : MonoBehaviour
{
    public float explosionDelay = 2f;
    public GameObject explosionEffect;
    public LayerMask explosionMask;
    public Tilemap tilemap;  // Ÿ�ϸ� ����

    void Start()
    {
        Invoke("Explode", explosionDelay);
    }

    void Explode()
    {
        Vector3 bombPosition = transform.position;

        // Ÿ�ϸ� �׸����� �� ��ǥ�� ��ȯ
        Vector3Int cellPosition = tilemap.WorldToCell(bombPosition);

        // �����¿� 4ĭ�� ���� ����Ʈ ����
        CreateExplosion(cellPosition + new Vector3Int(1, 0, 0), Quaternion.Euler(0, 0, 0));
        CreateExplosion(cellPosition + new Vector3Int(-1, 0, 0), Quaternion.Euler(0, 0, 180));
        CreateExplosion(cellPosition + new Vector3Int(0, 1, 0), Quaternion.Euler(0, 0, 90));
        CreateExplosion(cellPosition + new Vector3Int(0, -1, 0), Quaternion.Euler(0, 0, -90));

        // ��ź ������Ʈ ����
        Destroy(gameObject);
    }

    void CreateExplosion(Vector3Int cellPosition, Quaternion rotation)
    {
        // Ÿ�ϸ��� �� ��ǥ�� ���� ��ǥ ��ȯ
        Vector3 explosionPosition = tilemap.GetCellCenterWorld(cellPosition);

        // ���� ����Ʈ�� ��ġ���� üũ
        if (Physics2D.OverlapCircle(explosionPosition, 0.1f, explosionMask) == null)
        {
            GameObject explosion = Instantiate(explosionEffect, explosionPosition, rotation);

            // Ÿ�� ũ�⸦ �������� ���� ����Ʈ ũ�� ����
            Vector3 tileSize = tilemap.cellSize;

            // ���� ����Ʈ �������� ���� ũ�⸦ ������ Ÿ�� ũ�⿡ �°� ������ ����
            SpriteRenderer spriteRenderer = explosion.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                Vector3 spriteSize = spriteRenderer.bounds.size;
                explosion.transform.localScale = new Vector3(tileSize.x / spriteSize.x, tileSize.y / spriteSize.y, 1);
            }
        }
    }
}
