using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Item : MonoBehaviour
{
    [SerializeField] public ItemSo itemSos;
    public int num;
    [SerializeField] private InventoryManager inventoryManager;

    private void Awake()
    {
        if (inventoryManager == null)
            inventoryManager = FindFirstObjectByType<InventoryManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            inventoryManager.AddItem(itemSos ,num);
            Destroy(gameObject);
        }
    }
}
