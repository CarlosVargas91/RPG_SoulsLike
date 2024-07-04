using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dodge_Skill : Skill
{
    [Header("Dodge")]
    [SerializeField] private UI_SkillTreeSlot unlockDodgeButton;
    [SerializeField] private int evasionAmount;
    public bool dodgeUnlocked;

    [Header("Mirage dodge")]
    [SerializeField] private UI_SkillTreeSlot unlockMirageDodgeButton;
    public bool dodgeMirageUnlocked;

    protected override void Start()
    {
        base.Start();

        unlockDodgeButton.GetComponent<Button>().onClick.AddListener(unlockDodge);
        unlockMirageDodgeButton.GetComponent<Button>().onClick.AddListener(unlockMirageDodge);
    }
    protected override void checkUnlock()
    {
        unlockDodge();
        unlockMirageDodge();
    }
    private void unlockDodge()
    {
        if (unlockDodgeButton.unlocked && !dodgeUnlocked)
        {
            player.stats.evasion.addModifier(evasionAmount);
            Inventory.instance.updateStatsUI();
            dodgeUnlocked = true;
        }
    }

    private void unlockMirageDodge()
    {
        if (unlockMirageDodgeButton.unlocked)
            dodgeMirageUnlocked = true;
    }

    public void createMirageOnDodge()
    {
        if (dodgeMirageUnlocked)
            SkillManager.instance.clone.createClone(player.transform, new Vector3(2 * player.facingDir,0));
    }
}
