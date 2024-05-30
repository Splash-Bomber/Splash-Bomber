using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using UnityEngine.Tilemaps;

public class EnemyAI : MonoBehaviour
{
    public Transform player;  // �÷��̾��� Transform
    public GameObject bombPrefab;  // ��ź ������
    public float detectionRange = 1.5f;  // �÷��̾���� �Ÿ�
    public float explosionDelay = 3f;  // ���� ���� �ð�
    public Tilemap destructibleTiles;  // �ı� ������ Ÿ�ϸ�
    public LayerMask explosionMask;  // ���� ���� ���̾� ����ũ
    private NavMeshAgent agent;
    private bool isWaitingForExplosion = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent component is missing from this game object");
            enabled = false;  // ��ũ��Ʈ�� ��Ȱ��ȭ�Ͽ� ���� ����
            return;
        }
    }

    void Update()
    {
        if (!isWaitingForExplosion && agent.isOnNavMesh)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer <= detectionRange && !IsDestructibleTileInPath())
            {
                // �÷��̾�� ���� �Ÿ� �̳��� �ְ�, ��ο� �ı� ������ Ÿ���� ���� �� ��ź ��ġ
                StartCoroutine(PlaceBomb());
            }
            else
            {
                // �÷��̾� ����
                agent.SetDestination(player.position);
            }

            // NavMeshAgent ���� �����
            Debug.Log($"Destination: {agent.destination}, Remaining Distance: {agent.remainingDistance}");
        }
    }

    bool IsDestructibleTileInPath()
    {
        Vector3[] directions = { Vector3.right, Vector3.left, Vector3.up, Vector3.down };
        foreach (var direction in directions)
        {
            Vector3 checkPosition = transform.position + direction;
            Vector3Int cellPosition = destructibleTiles.WorldToCell(checkPosition);
            TileBase tile = destructibleTiles.GetTile(cellPosition);

            if (tile != null)
            {
                return true;
            }
        }
        return false;
    }

    IEnumerator PlaceBomb()
    {
        isWaitingForExplosion = true;

        // ��ź ��ġ
        Vector3 bombPosition = transform.position;
        Instantiate(bombPrefab, bombPosition, Quaternion.identity);

        // ���� ���� ������ �̵�
        Vector3Int[] directions = new Vector3Int[]
        {
            new Vector3Int(1, 0, 0),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(0, 1, 0),
            new Vector3Int(0, -1, 0)
        };

        foreach (var direction in directions)
        {
            Vector3Int cellPosition = destructibleTiles.WorldToCell(bombPosition) + direction;
            Vector3 targetPosition = destructibleTiles.GetCellCenterWorld(cellPosition);

            if (!Physics2D.OverlapCircle(targetPosition, 0.1f, explosionMask))
            {
                agent.SetDestination(targetPosition);
                yield return new WaitForSeconds(explosionDelay + 0.5f);
                isWaitingForExplosion = false;
                yield break;
            }
        }

        // ���� ��� ������ ���� �ִٸ� ���ڸ��� ���
        yield return new WaitForSeconds(explosionDelay + 0.5f);
        isWaitingForExplosion = false;
    }
}
