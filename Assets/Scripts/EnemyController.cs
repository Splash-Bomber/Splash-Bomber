using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using UnityEngine.Tilemaps;

public class EnemyAI : MonoBehaviour
{
    public Transform player;  // 플레이어의 Transform
    public GameObject bombPrefab;  // 폭탄 프리팹
    public float detectionRange = 1.5f;  // 플레이어와의 거리
    public float explosionDelay = 3f;  // 폭발 지연 시간
    public Tilemap destructibleTiles;  // 파괴 가능한 타일맵
    public LayerMask explosionMask;  // 폭발 범위 레이어 마스크
    private NavMeshAgent agent;
    private bool isWaitingForExplosion = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent component is missing from this game object");
            enabled = false;  // 스크립트를 비활성화하여 오류 방지
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
                // 플레이어와 일정 거리 이내에 있고, 경로에 파괴 가능한 타일이 없을 때 폭탄 설치
                StartCoroutine(PlaceBomb());
            }
            else
            {
                // 플레이어 추적
                agent.SetDestination(player.position);
            }

            // NavMeshAgent 상태 디버깅
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

        // 폭탄 설치
        Vector3 bombPosition = transform.position;
        Instantiate(bombPrefab, bombPosition, Quaternion.identity);

        // 폭발 범위 밖으로 이동
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

        // 만약 모든 방향이 막혀 있다면 제자리에 대기
        yield return new WaitForSeconds(explosionDelay + 0.5f);
        isWaitingForExplosion = false;
    }
}
