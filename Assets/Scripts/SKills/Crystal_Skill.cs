using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crystal_Skill : Skill
{
    [SerializeField] private GameObject crystalPrefab;
    [SerializeField] private float crystalDuration;
    private GameObject currentCrystal;

    [Header("Crystal simple")]
    [SerializeField] private UI_SkillTreeSlot crystalUnlockButton;
    public bool crystalUnlocked { get; private set; }

    [Header("Crystal mirage")]
    [SerializeField] private UI_SkillTreeSlot crystalMirageUnlockButton;
    [SerializeField] private bool cloneInsteadOfCrystal;


    [Header("Explosive crystal")]
    [SerializeField] private UI_SkillTreeSlot explosiveCrystalUnlockButton;
    [SerializeField] private float explosiveCoolDown;
    [SerializeField] private bool canExplode;

    [Header("Moving crystal")]
    [SerializeField] private UI_SkillTreeSlot movingCrystalUnlockButton;
    [SerializeField] private bool canMoveToEnemy;
    [SerializeField] private float moveSpeed;

    [Header("Multi stacking crystal")]
    [SerializeField] private UI_SkillTreeSlot multiCrystalUnlockButton;
    [SerializeField] private bool canUseMultiStacks;
    [SerializeField] private int amountOfStacks;
    [SerializeField] private float multiStackCoolDown;
    [SerializeField] private float useTimeWindow;
    [SerializeField] private List<GameObject> crystalLeft = new List<GameObject>();


    protected override void Start()
    {
        base.Start();

        crystalUnlockButton.GetComponent<Button>().onClick.AddListener(unlockCrystal);
        crystalMirageUnlockButton.GetComponent<Button>().onClick.AddListener(unlockCrystalMirage);
        explosiveCrystalUnlockButton.GetComponent<Button>().onClick.AddListener(unlockExplosiveCrystal);
        movingCrystalUnlockButton.GetComponent<Button>().onClick.AddListener(unlockMovingCrystal);
        multiCrystalUnlockButton.GetComponent<Button>().onClick.AddListener(unlockMultiStack);
    }

    //Unlock crystal skills
    #region Unlock Skill
    private void unlockCrystal()
    {
        if (crystalUnlockButton.unlocked)
            crystalUnlocked = true;
    }

    private void unlockCrystalMirage()
    {
        if (crystalMirageUnlockButton.unlocked)
            cloneInsteadOfCrystal = true;
    }

    private void unlockExplosiveCrystal()
    {
        if (explosiveCrystalUnlockButton.unlocked)
        {
            canExplode = true;
            coolDown = explosiveCoolDown;
        }
    }

    private void unlockMovingCrystal()
    {
        if (movingCrystalUnlockButton.unlocked)
            canMoveToEnemy = true;
    }

    private void unlockMultiStack()
    {
        if (multiCrystalUnlockButton.unlocked)
            canUseMultiStacks = true;
    }

    protected override void checkUnlock()
    {
        unlockCrystal();
        unlockCrystalMirage();
        unlockExplosiveCrystal();
        unlockMovingCrystal();
        unlockMultiStack();
    }
    #endregion
    public override void useSKill()
    {
        base.useSKill();

        if (canUseMultiCrystal())
            return;

        if (currentCrystal == null)
        {
            createCrystal();
        }
        else
        {
            if (canMoveToEnemy)
                return;

            Vector2 playerPos = player.transform.position;

            player.transform.position = currentCrystal.transform.position;

            currentCrystal.transform.position = playerPos;

            if (cloneInsteadOfCrystal)
            {
                SkillManager.instance.clone.createClone(currentCrystal.transform, Vector3.zero);
                Destroy(currentCrystal);
            }
            else
                currentCrystal.GetComponent<Crystal_Skill_Controller>()?/* Is null? */.finishCrystal();
        }
    }

    public void createCrystal()
    {
        currentCrystal = Instantiate(crystalPrefab, player.transform.position, Quaternion.identity);
        Crystal_Skill_Controller currentCrystalScript = currentCrystal.GetComponent<Crystal_Skill_Controller>();

        currentCrystalScript.setupCrystal(crystalDuration, canExplode, canMoveToEnemy, moveSpeed, findClosestEnemy(currentCrystal.transform), player);
    }

    public void currentCrystalChooseRandomTarget() => currentCrystal.GetComponent<Crystal_Skill_Controller>().chooseRandomEnemy();
    private bool canUseMultiCrystal()
    {
        if (canUseMultiStacks)
        {
            if (crystalLeft.Count > 0)
            {
                if (crystalLeft.Count == amountOfStacks)
                    Invoke("resetAbility", useTimeWindow);

                coolDown = 0;
                GameObject crystalToSpawn = crystalLeft[crystalLeft.Count - 1];
                GameObject newCrystal = Instantiate(crystalToSpawn, player.transform.position, Quaternion.identity);

                crystalLeft.Remove(crystalToSpawn);

                newCrystal.GetComponent<Crystal_Skill_Controller>().
                    setupCrystal(crystalDuration, canExplode, canMoveToEnemy, moveSpeed, findClosestEnemy(newCrystal.transform), player);

                if (crystalLeft.Count <= 0)
                {
                    coolDown = multiStackCoolDown;
                    refillCrystal();
                }
                return true;
            }
        }

        return false;
    }
    private void refillCrystal()
    {
        int amountToAdd = amountOfStacks - crystalLeft.Count;

        for (int i = 0; i < amountToAdd; i++)
        {
            crystalLeft.Add(crystalPrefab);
        }
    }

    private void resetAbility()
    {
        if (coolDownTimer > 0)
            return;

        coolDownTimer = multiStackCoolDown;
        refillCrystal();
    }
}
