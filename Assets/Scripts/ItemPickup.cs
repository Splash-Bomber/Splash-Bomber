using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public enum ItemType
    {
        ExtraWaterBomb,
        ExplosionExtension,
        SpeedUp,
    }
    
    public ItemType itemType;
    
    private void OnItemPickup(GameObject player)
    {
        switch (itemType)
        {
            case ItemType.ExtraWaterBomb:
                player.GetComponent<WaterBombController>().AddWaterBomb();
                break;
            case ItemType.ExplosionExtension:
                player.GetComponent<WaterBombController>().explosionRadius++;
                break;
            case ItemType.SpeedUp:
                player.GetComponent<MovementController>().speed++;
                break;
        }
        
        Destroy(gameObject);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            OnItemPickup(other.gameObject);
        }
    }
}
