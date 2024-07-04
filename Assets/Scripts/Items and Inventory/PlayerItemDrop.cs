using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemDrop : ItemDrop
{
    [Header("Players drop")]
    [SerializeField] private float chanceToLooseItems;
    [SerializeField] private float chanceToLooseMaterials;

    public override void generateDrop()
    {
        Inventory inventory = Inventory.instance;

        List<InventoryItem> itemsToUnequip = new List<InventoryItem>();
        List<InventoryItem> materialsToloose = new List<InventoryItem>();

        foreach (InventoryItem item in inventory.getEquipmentList())
        {
            if (Random.Range(0, 100) <= chanceToLooseItems)
            {
                dropItem(item.data);
                itemsToUnequip.Add(item);
            }
        }
        for (int i = 0; i < itemsToUnequip.Count; i++)
        {
            inventory.unequipItem(itemsToUnequip[i].data as ItemData_Equipment);
        }

        foreach (InventoryItem item in inventory.getStashList())
        {
            if (Random.Range(0, 100) <= chanceToLooseMaterials)
            {
                dropItem(item.data);
                materialsToloose.Add(item);
            }
        }
        for (int i = 0; i < materialsToloose.Count; i++)
        {
            inventory.removeItem(materialsToloose[i].data as ItemData_Equipment);
        }

    }
}
