using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Inventory : MonoBehaviour, ISaveManager
{
    public static Inventory instance;

    public List<ItemData> startingItems;

    public List<InventoryItem> equipment;
    public Dictionary<ItemData_Equipment, InventoryItem> equipmentDictionary;

    public List<InventoryItem> inventory;
    public Dictionary<ItemData, InventoryItem> inventoryDictionary;

    public List<InventoryItem> stash;
    public Dictionary<ItemData, InventoryItem> stashDictionary;

    [Header("Inventory UI")]
    [SerializeField] private Transform inventorySlotParent;
    [SerializeField] private Transform stashSlotParent;
    [SerializeField] private Transform equipmentSlotParent;
    [SerializeField] private Transform statSlotParent;

    private UI_ItemSlot[] inventoryItemSlot;
    private UI_ItemSlot[] stashItemSlot;
    private UI_EquipmentSlot[] equipmentSlot;
    private UI_StatSlot[] statSlot;

    [Header("Item coolsown")]
    private float lastTimeUsedFlask;
    private float lastTimeUsedArmor;

    public float flaskCooldown {  get; private set; }
    private float armorCooldwon;

    [Header("Data base")]
    public List<ItemData> itemDataBase;
    public List<InventoryItem> loadedItems;
    public List<ItemData_Equipment> loadedEquipment;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        inventory = new List<InventoryItem>();
        inventoryDictionary = new Dictionary<ItemData, InventoryItem>();

        stash = new List<InventoryItem>();
        stashDictionary = new Dictionary<ItemData, InventoryItem>();

        equipment = new List<InventoryItem>();
        equipmentDictionary = new Dictionary<ItemData_Equipment, InventoryItem>();

        inventoryItemSlot = inventorySlotParent.GetComponentsInChildren<UI_ItemSlot>();
        stashItemSlot = stashSlotParent.GetComponentsInChildren<UI_ItemSlot>();
        equipmentSlot = equipmentSlotParent.GetComponentsInChildren<UI_EquipmentSlot>();
        statSlot = statSlotParent.GetComponentsInChildren<UI_StatSlot>();

        addStartingItems();
    }

    private void addStartingItems()
    {
        foreach (ItemData_Equipment item in loadedEquipment)
        {
            equipItem(item);
        }

        if (loadedItems.Count > 0)
        {
            foreach (InventoryItem item in loadedItems)
            {
                for (int i = 0; i < item.stackSize; i++)
                {
                    addItem(item.data);
                }
            }

            return;
        }

        for (int i = 0; i < startingItems.Count; i++)
        {
            if (startingItems[i] != null)
                addItem(startingItems[i]);
        }
    }

    public void equipItem(ItemData _item)
    {
        ItemData_Equipment newEquipment = _item as ItemData_Equipment; //Casting? 
        InventoryItem newItem = new InventoryItem(newEquipment);

        ItemData_Equipment oldEquipment = null;

        foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionary)
        {
            if (item.Key.equipmentType == newEquipment.equipmentType)
                oldEquipment = item.Key;
        }

        if (oldEquipment != null)
        {
            unequipItem(oldEquipment);
            addItem(oldEquipment);
        }

        equipment.Add(newItem);
        equipmentDictionary.Add(newEquipment, newItem);
        newEquipment.addModifiers();

        removeItem(_item);

        updateSlotUI();
    }

    public void unequipItem(ItemData_Equipment _itemToRemove)
    {
        if (equipmentDictionary.TryGetValue(_itemToRemove, out InventoryItem value))
        {
            equipment.Remove(value);
            equipmentDictionary.Remove(_itemToRemove);
            _itemToRemove.removeModifiers();
        }
    }

    private void updateSlotUI()
    {
        for (int i = 0; i < equipmentSlot.Length; i++)
        {
            foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionary)
            {
                if (item.Key.equipmentType == equipmentSlot[i].slotType)
                    equipmentSlot[i].updateSlot(item.Value);
            }
        }

        for (int i = 0; i < inventoryItemSlot.Length; i++)
        {
            inventoryItemSlot[i].cleanUpSlot();
        }

        for (int i = 0; i < stashItemSlot.Length; i++)
        {
            stashItemSlot[i].cleanUpSlot();
        }


        for (int i = 0; i < inventory.Count; i++)
        {
            inventoryItemSlot[i].updateSlot(inventory[i]);
        }

        for (int i = 0; i < stash.Count; i++)
        {
            stashItemSlot[i].updateSlot(stash[i]);
        }

        updateStatsUI();
    }

    public void updateStatsUI()
    {
        for (int i = 0; i < statSlot.Length; i++) //Update info on UI
        {
            statSlot[i].updateStatValueUI();
        }
    }

    public void addItem(ItemData _item)
    {
        if (_item.itemType == ItemType.Equipment && cannAddItem())
            addToInventory(_item);
        else if (_item.itemType == ItemType.Material)
            addToStash(_item);

        updateSlotUI();
    }

    private void addToStash(ItemData _item)
    {
        if (stashDictionary.TryGetValue(_item, out InventoryItem value))
        {
            value.addStack();
        }
        else
        {
            InventoryItem newItem = new InventoryItem(_item);
            stash.Add(newItem);
            stashDictionary.Add(_item, newItem);
        }
    }

    private void addToInventory(ItemData _item)
    {
        if (inventoryDictionary.TryGetValue(_item, out InventoryItem value))
        {
            value.addStack();
        }
        else
        {
            InventoryItem newItem = new InventoryItem(_item);
            inventory.Add(newItem);
            inventoryDictionary.Add(_item, newItem);
        }
    }

    public void removeItem(ItemData _item)
    {
        if (inventoryDictionary.TryGetValue(_item, out InventoryItem value))
        {
            if (value.stackSize <= 1)
            {
                inventory.Remove(value);
                inventoryDictionary.Remove(_item);
            }
            else
                value.removeStack();
        }

        if (stashDictionary.TryGetValue(_item, out InventoryItem stashValue))
        {
            if (stashValue.stackSize <= 1)
            {
                stash.Remove(stashValue);
                stashDictionary.Remove(_item);
            }
            else
                stashValue.removeStack();
        }
        updateSlotUI();
    }

    public bool cannAddItem()
    {
        if (inventory.Count >= inventoryItemSlot.Length)
            return false;

        return true;
    }
    public bool canCraft(ItemData_Equipment _itemToCraft, List<InventoryItem> _requiredMaterials)
    {
        List<InventoryItem> materialsToRemove = new List<InventoryItem>();

        for (int i = 0; i < _requiredMaterials.Count; i++)
        {
            if (stashDictionary.TryGetValue(_requiredMaterials[i].data, out InventoryItem stashValue))
            {
                if (stashValue.stackSize < _requiredMaterials[i].stackSize)
                {
                    return false;
                }
                else
                {
                    materialsToRemove.Add(stashValue);
                }
            }
            else
            {
                return false;
            }
        }

        for (int i = 0; i < materialsToRemove.Count; i++)
        {
            removeItem(materialsToRemove[i].data);
        }

        addItem(_itemToCraft);

        return true;
    }

    public List<InventoryItem> getEquipmentList() => equipment;
    public List<InventoryItem> getStashList() => stash;

    public ItemData_Equipment getEquipment(EquipmentType _type)
    {
        ItemData_Equipment equipItem = null;

        foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionary)
        {
            if (item.Key.equipmentType == _type)
                equipItem = item.Key;
        }

        return equipItem;
    }

    public void useFlask()
    {
        ItemData_Equipment currentFlask = getEquipment(EquipmentType.Flask);

        if (currentFlask == null)
            return;

        bool canUseFlask = Time.time > lastTimeUsedFlask + flaskCooldown;

        if(canUseFlask)
        {
            flaskCooldown = currentFlask.itemCoolDown;
            currentFlask.effect(null);
            lastTimeUsedFlask = Time.time;
        }
    }

    public bool canUseArmor()
    {
        ItemData_Equipment currentArmor = getEquipment(EquipmentType.Armor);

        if (Time.time > lastTimeUsedArmor + armorCooldwon)
        {
            armorCooldwon = currentArmor.itemCoolDown;
            lastTimeUsedArmor = Time.time;
            return true;
        }

        return false;
    }

    public void loadData(GameData _data)
    {
        foreach (KeyValuePair<string, int> pair in _data.inventory)
        {
            foreach (var item in itemDataBase)
            {
                if (item != null && item.itemId == pair.Key)
                {
                    InventoryItem itemToLoad = new InventoryItem(item);
                    itemToLoad.stackSize = pair.Value;

                    loadedItems.Add(itemToLoad);
                }
            }
        }

        foreach (string loadedItemId in _data.equipmentId)
        {
            foreach (var item in itemDataBase)
            {
                if (item != null && loadedItemId == item.itemId) 
                {
                    loadedEquipment.Add(item as ItemData_Equipment);
                }
            }
        }
    }

    public void saveData(ref GameData _data)
    {
        _data.inventory.Clear();
        _data.equipmentId.Clear();

        foreach (KeyValuePair<ItemData, InventoryItem> pair in inventoryDictionary)
        {
            _data.inventory.Add(pair.Key.itemId, pair.Value.stackSize);
        }

        foreach (KeyValuePair<ItemData, InventoryItem> pair in stashDictionary)
        {
            _data.inventory.Add(pair.Key.itemId, pair.Value.stackSize);
        }

        foreach (KeyValuePair<ItemData_Equipment, InventoryItem> pair in equipmentDictionary)
        {
            _data.equipmentId.Add(pair.Key.itemId);
        }
    }

#if UNITY_EDITOR //To avoid build errors
    [ContextMenu("Fill up item database")]
    private void fillupItemDatabase() => itemDataBase = new List<ItemData>(getItemDataBase());

    /* 
     * breakdown of what it does:
     * It initializes itemDataBase as a new list of ItemData objects.
     * It uses the AssetDatabase.FindAssets method to find all assets in the “Assets/Data/Equipment” directory. The names of these assets are stored in the assetNames array.
     * It then iterates over each asset name in assetNames.
     * For each asset name, it uses AssetDatabase.GUIDToAssetPath to convert the asset’s GUID to a path.
     * It then uses AssetDatabase.LoadAssetAtPath to load the asset at the given path as an ItemData object.
     * This ItemData object is then added to the itemDataBase list.
     * Finally, it returns the itemDataBase list.
     * In summary, this function is used to load all ItemData objects from the “Assets/Data/Equipment” directory into a list. This could be useful, for example, 
     * in a game where you need to load all equipment data at runtime. Please note that this function is specific to Unity and its asset management system. 
     * It won’t work in other contexts or programming environments. */

    private List<ItemData> getItemDataBase()
    {
        List<ItemData> itemDataBase = new List<ItemData>();
        string [] assetNames = AssetDatabase.FindAssets("", new[] { "Assets/Data/Items" });

        foreach (string SOName in assetNames)
        {
            var SOpath = AssetDatabase.GUIDToAssetPath(SOName);
            var itemData = AssetDatabase.LoadAssetAtPath<ItemData>(SOpath);
            itemDataBase.Add(itemData);
        }

        return itemDataBase;
    }
#endif
}
