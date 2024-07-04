using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_SkillToolTip : UI_ToolTip
{
    [SerializeField] private TextMeshProUGUI skillText;
    [SerializeField] private TextMeshProUGUI skillName;
    [SerializeField] private TextMeshProUGUI skillCost;
    [SerializeField] private float defaultNameFontSize;
    public void showToolTip(string _skillDescription, string _skillName, int _price)
    {
        skillName.text = _skillName;
        skillText.text = _skillDescription;
        skillCost.text = "Cost: " + _price;

        adjustPosition();

        adjustFontSize(skillName);

        gameObject.SetActive(true);
    }

    public void hideToolTip()
    {
        skillName.fontSize = defaultNameFontSize;
        gameObject.SetActive(false);
    }
}
