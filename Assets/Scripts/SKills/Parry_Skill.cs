using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Parry_Skill : Skill
{
    [Header("Parry")]
    [SerializeField] private UI_SkillTreeSlot parryUnlockButton;
    public bool parryUnlocked {  get; private set; }

    [Header("Parry restore")]
    [SerializeField] private UI_SkillTreeSlot restoreUnlockButton;
    [Range(0f, 1f)]
    [SerializeField] private float restoreHealthPercentage;
    public bool restoreUnlocked { get; private set; }

    [Header("Parry with a mirage")]
    [SerializeField] private UI_SkillTreeSlot parryWithMirageUnlockButton;
    public bool parryWithMirageUnlocked { get; private set; }

    public override void useSKill()
    {
        base.useSKill();

        if (restoreUnlocked)
        {
            int restoreAmount = Mathf.RoundToInt(player.stats.getMaxHealthValue() * restoreHealthPercentage);
            player.stats.incereaseHealthBy(restoreAmount);
        }
    }

    protected override void Start()
    {
        base.Start();

        parryUnlockButton.GetComponent<Button>().onClick.AddListener(unlockParry);
        restoreUnlockButton.GetComponent<Button>().onClick.AddListener(unlockParryRestore);
        parryWithMirageUnlockButton.GetComponent<Button>().onClick.AddListener(unlockParryWithMirage);
    }

    protected override void checkUnlock()
    {
        unlockParry();
        unlockParryRestore();
        unlockParryWithMirage();
    }
    private void unlockParry()
    {
        if (parryUnlockButton.unlocked)
            parryUnlocked = true;
    }

    private void unlockParryRestore()
    {
        if (restoreUnlockButton.unlocked)
            parryUnlocked = true;
    }

    private void unlockParryWithMirage()
    {
        if (parryWithMirageUnlockButton.unlocked)
            parryWithMirageUnlocked = true;
    }

    public void makeMirageOnParry(Transform _respawnTransform)
    {
        if (parryWithMirageUnlocked)
            SkillManager.instance.clone.createCloneWithDelay(_respawnTransform);
    }
}
