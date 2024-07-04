using System;

[Serializable]
public class InventoryItem
{
    public ItemData data;
    public int stackSize;

    public InventoryItem(ItemData _newItemData)
    {
        data = _newItemData;
        addStack();
    }

    public void addStack() => stackSize++;
    public void removeStack() => stackSize--;
}
