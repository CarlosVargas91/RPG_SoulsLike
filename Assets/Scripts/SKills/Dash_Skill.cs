using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dash_Skill : Skill
{
    [Header("Dash")]
    [SerializeField] private UI_SkillTreeSlot dashUnlockButton;
    public bool dashUnlocked {  get; private set; }

    [Header("Clone on Dash")]
    [SerializeField] private UI_SkillTreeSlot cloneOnDashUnlockButton;
    public bool cloneOnDashUnlocked { get; private set; }

    [Header("Clone on Arrival")]
    [SerializeField] private UI_SkillTreeSlot cloneOnArrivalUnlockButton;
    public bool cloneOnArrivalUnlocked { get; private set; }
    public override void useSKill()
    {
        base.useSKill();
    }


    protected override void Start()
    {
        base.Start();

        dashUnlockButton.GetComponent<Button>().onClick.AddListener(unlockDash);
        cloneOnDashUnlockButton.GetComponent<Button>().onClick.AddListener(unlockCloneOnDash);
        cloneOnArrivalUnlockButton.GetComponent<Button>().onClick.AddListener(unlockCloneOnArrival);
    }

    protected override void checkUnlock()
    {
        unlockCloneOnArrival();
        unlockCloneOnDash();
        unlockDash();
    }
    private void unlockDash()
    {
        if (dashUnlockButton.unlocked)
            dashUnlocked = true;
    }
    private void unlockCloneOnDash()
    {
        if (cloneOnDashUnlockButton.unlocked)
            cloneOnDashUnlocked = true;
    }

    private void unlockCloneOnArrival()
    {
        if (cloneOnArrivalUnlockButton.unlocked)
            cloneOnArrivalUnlocked = true;
    }

    public void cloneOnDash()
    {
        if (cloneOnDashUnlocked)
            SkillManager.instance.clone.createClone(player.transform, Vector3.zero);
    }

    public void cloneOnArrival()
    {
        if (cloneOnArrivalUnlocked)
            SkillManager.instance.clone.createClone(player.transform, Vector3.zero);
    }
}
