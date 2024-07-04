using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_StatToolTip : UI_ToolTip
{
    [SerializeField] private TextMeshProUGUI description;
    
    public void showStatToolTip(string _text)
    {
        description.text = _text;
        adjustPosition();

        gameObject.SetActive(true);
    }

    public void hideStatToolTip()
    {
        description.text = "";

        gameObject.SetActive(false);
    }
}
