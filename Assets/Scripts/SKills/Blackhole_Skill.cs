using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Blackhole_Skill : Skill
{
    [SerializeField] private UI_SkillTreeSlot blackholeUnlockButton;
    public bool blackholeUnlocked {  get; private set; }
    [SerializeField] private int amountOfAttacks;
    [SerializeField] private float cloneCooldown;
    [SerializeField] private float blackholeDuration;

    [SerializeField] private GameObject blackHolePrefab;
    [SerializeField] private float maxSize;
    [SerializeField] private float growSpeed;
    [SerializeField] private float shrinkSpeed;

    Blackhole_Skill_Controller currentBlackhole;

    private void unlockBlackHole()
    {
        if (blackholeUnlockButton.unlocked)
            blackholeUnlocked = true;
    }
    public override bool canUseSKill()
    {
        return base.canUseSKill();
    }

    public override void useSKill()
    {
        base.useSKill();

        GameObject newBlackHole = Instantiate(blackHolePrefab, player.transform.position, Quaternion.identity);

        currentBlackhole = newBlackHole.GetComponent<Blackhole_Skill_Controller>();

        currentBlackhole.setupBlackHole(maxSize, growSpeed, shrinkSpeed, amountOfAttacks, cloneCooldown, blackholeDuration);

        AudioManager.instance.playSFX(6, player.transform);
        AudioManager.instance.playSFX(3, player.transform);
    }

    protected override void Start()
    {
        base.Start();

        blackholeUnlockButton.GetComponent<Button>().onClick.AddListener(unlockBlackHole);
    }

    protected override void Update()
    {
        base.Update();
    }

    public bool skillCompleted()
    {
        if (!currentBlackhole)
            return false;
        
        if (currentBlackhole.playerCanExitState)
        {
            currentBlackhole = null;
            return true;
        }

        return false;
    }

    public float getBlackHoleRadius()
    {
        return maxSize / 2;
    }

    protected override void checkUnlock()
    {
        unlockBlackHole();
    }
}
