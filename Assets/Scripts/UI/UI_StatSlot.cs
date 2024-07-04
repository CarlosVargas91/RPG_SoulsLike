using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_StatSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private UI ui;

    [SerializeField] private string statName;

    [SerializeField] private StatType statType;
    [SerializeField] private TextMeshProUGUI statValueText;
    [SerializeField] private TextMeshProUGUI statNameText;

    [TextArea]
    [SerializeField] private string statDescription;

    private void OnValidate()
    {
        gameObject.name = "Stat - " + statName;

        if (statNameText  != null )
            statNameText.text = statName;
    }
    // Start is called before the first frame update
    void Start()
    {
        updateStatValueUI();

        ui = GetComponentInParent<UI>();
    }

    public void updateStatValueUI()
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        if ( playerStats != null )
        {
            statValueText.text = playerStats.getStat(statType).getValue().ToString();

            if (statType == StatType.health)
                statValueText.text = playerStats.getMaxHealthValue().ToString();

            if (statType == StatType.damage)
                statValueText.text = (playerStats.damage.getValue() + playerStats.strength.getValue()).ToString();

            if (statType == StatType.critPower)
                statValueText.text = (playerStats.critPower.getValue() + playerStats.strength.getValue()).ToString() ;

            if (statType == StatType.critChance)
                statValueText.text = (playerStats.critChance.getValue() + playerStats.agility.getValue()).ToString();

            if (statType == StatType.evasion)
                statValueText.text = (playerStats.evasion.getValue() + playerStats.agility.getValue()).ToString();

            if (statType == StatType.magicRes)
                statValueText.text = (playerStats.magicResistance.getValue() + (playerStats.intelligence.getValue() * 3)).ToString();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.statToolTip.showStatToolTip(statDescription);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.statToolTip.hideStatToolTip();
    }
}
