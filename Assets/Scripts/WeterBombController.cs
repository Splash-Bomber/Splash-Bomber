using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WaterBombController : MonoBehaviour
{
    [Header("WaterBomb")]
    public GameObject waterBombPrefab;  // ��ź ������
    public KeyCode inputKey = KeyCode.Space;  // ��ź ��ġ Ű
    public float explosionDelay = 3f; // ���� ���� �ð�
    public int waterBombAmout = 1;  // ��ź ��ġ ���� ����
    private int waterBombsRemaining;  // ���� ��ź ����
    
    [Header("Explosion")]
    public ExplosionController explosionPrefeb;
    public LayerMask explosionLayer;
    public float explosionDuration = 1f;
    public int explosionRadius = 1;
    
    public GameObject explosionEffect;
    public LayerMask explosionMask;
    public Tilemap tilemap;  // Ÿ�ϸ� ����

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
     * ��ź�� ��ġ�ϴ� �ڷ�ƾ �Լ�
     * ��ź�� ��ġ�ϰ� ���� ȿ���� �����Ѵ�.
     * ���� ȿ���� ���� ���� �ð� �Ŀ� �����ȴ�.
     * ���� ȿ���� ���� �ݰ游ŭ �����Ѵ�.
     * ���� ȿ���� �� �������� �����Ѵ�.
     * ��ź ��ġ �� ���� ��ź ������ ���ҽ�Ų��.
     * ��ź ��ġ �� ���� ȿ���� �����Ǹ� ��ź�� �����Ѵ�.
     * ��ź ��ġ �� ���� ȿ���� �����Ǹ� ���� ȿ���� �����Ѵ�.
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
     * ���� ȿ���� �����ϴ� �Լ�
     * @param position ���� ��ġ
     * @param direction ���� ����
     * @param length ���� ����
     */
    private void Explode(Vector2 position, Vector2 direction, int length)
    {
        if (length <= 0)
        {
            return;
        }

        position += direction;
        
        // ���� ȿ���� ���� �浹�� ���
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
     * ���� ȿ���� �浹�� ��� ȣ��Ǵ� �Լ�
     * ���� ȿ���� �浹�� Ÿ���� �����Ѵ�.
     * ���� ȿ���� �浹�� ������Ʈ�� �����Ѵ�.
     */
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("WaterBomb"))
        {
            other.isTrigger = false;
        }
    }
}
