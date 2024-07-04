using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private ItemData itemData;

    private void setupVisuals()
    {
        if (itemData == null)
            return;

        GetComponent<SpriteRenderer>().sprite = itemData.icon;
        gameObject.name = "Item object - " + itemData.itemName;
    }

    public void setupItem(ItemData _itemData, Vector2 _velocity)
    {
        itemData = _itemData;
        rb.velocity = _velocity;

        setupVisuals();
    }
    public void pickupItem()
    {
        if (!Inventory.instance.cannAddItem() && itemData.itemType == ItemType.Equipment)
        {
            rb.velocity = new Vector2(0, 7); //Animation to move the object
            PlayerManager.instance.player.fx.createPopUpText("Inventory is full");
            return;
        }

        AudioManager.instance.playSFX(18, transform);
        Inventory.instance.addItem(itemData);
        Destroy(gameObject);
    }
}
