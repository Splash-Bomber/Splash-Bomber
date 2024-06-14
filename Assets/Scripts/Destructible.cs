using UnityEngine;

public class Destructible : MonoBehaviour
{
    public float destructionTime = 0.5f;
    
    [Range(0f, 1f)]
    public float itemSpawnChange = 0.3f;
    public GameObject[] spawnItems;

    private void Start()
    {
        Destroy(gameObject, destructionTime);
    }
    
    private void OnDestroy()
    {
        if (spawnItems.Length > 0 && Random.value < itemSpawnChange)
        {
            int randomIndex = Random.Range(0, spawnItems.Length);
            Instantiate(spawnItems[randomIndex], transform.position, Quaternion.identity);
        }
    }
}
