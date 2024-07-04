using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CraftWIndow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itenName;
    [SerializeField] private TextMeshProUGUI itenDescription;
    [SerializeField] private Image itemIcon;
    [SerializeField] private Button craftButton;

    [SerializeField] private Image[] materialImage;

    public void setupCraftWindow(ItemData_Equipment _data)
    {
        craftButton.onClick.RemoveAllListeners();

        for (int i = 0; i < materialImage.Length; i++)
        {
            materialImage[i].color = Color.clear;
            materialImage[i].GetComponentInChildren<TextMeshProUGUI>().color = Color.clear;
        }

        for (int i = 0; i < _data.craftingMaterial.Count; i++)
        {
            if (_data.craftingMaterial.Count > materialImage.Length)
                Debug.LogWarning("You have more material than slots in window");

            materialImage[i].sprite = _data.craftingMaterial[i].data.icon;
            materialImage[i].color = Color.white;

            TextMeshProUGUI materialSlotText = materialImage[i].GetComponentInChildren<TextMeshProUGUI>();

            materialSlotText.text = _data.craftingMaterial[i].stackSize.ToString();
            materialSlotText.color = Color.white;
        }

        itemIcon.sprite = _data.icon;
        itenName.text = _data.itemName;
        itenDescription.text = _data.getDescription();

        craftButton.onClick.AddListener(() => Inventory.instance.canCraft(_data, _data.craftingMaterial));
    }
}
