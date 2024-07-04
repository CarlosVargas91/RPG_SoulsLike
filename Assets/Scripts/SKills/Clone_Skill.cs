using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Clone_Skill : Skill
{
    [Header("Clone info")]
    [SerializeField] private float attackMultiplier;
    [SerializeField] private GameObject clonePrefab;
    [SerializeField] private float cloneDuration;
    [Space]

    [Header("Clone attack")]
    [SerializeField] private UI_SkillTreeSlot cloneAttackUnlockButton;
    [SerializeField] private float cloneAttackMultiplier;
    [SerializeField] private bool canAttack;

    [Header("Aggresive clone")]
    [SerializeField] private UI_SkillTreeSlot aggressiveUnlockButton;
    [SerializeField] private float aggresiveCloneAttackMultiplier;
    public bool canApplyOnHitEffect {  get; private set; }

    [Header("Multiple clone")]
    [SerializeField] private UI_SkillTreeSlot multipleUnlockButton;
    [SerializeField] private bool canDuplicateClone;
    [SerializeField] private float multiCloneAttackMultiplier;
    [SerializeField] private float chanceToDuplicate;

    [Header("Crystal instead of clone")]
    [SerializeField] private UI_SkillTreeSlot crystalInsteadUnlockButton;
    public bool crystalInsteadOfClone;

    protected override void Start()
    {
        base.Start();

        cloneAttackUnlockButton.GetComponent<Button>().onClick.AddListener(unlockCloneAttack);
        aggressiveUnlockButton.GetComponent<Button>().onClick.AddListener(unlockAggresiveClone);
        multipleUnlockButton.GetComponent<Button>().onClick.AddListener(unlockMultiClone);
        crystalInsteadUnlockButton.GetComponent<Button>().onClick.AddListener(unlockCrystalInstead);
    }

    #region Unlock
    private void unlockCloneAttack()
    {
        if (cloneAttackUnlockButton.unlocked)
        {
            canAttack = true;
            attackMultiplier = cloneAttackMultiplier;
        }
    }

    private void unlockAggresiveClone()
    {
        if (aggressiveUnlockButton.unlocked)
        {
            canApplyOnHitEffect = true;
            attackMultiplier = aggresiveCloneAttackMultiplier;
        }
    }

    private void unlockMultiClone()
    {
        if (multipleUnlockButton.unlocked)
        {
            canDuplicateClone = true;
            attackMultiplier = multiCloneAttackMultiplier;
        }
    }

    private void unlockCrystalInstead()
    {
        if (crystalInsteadUnlockButton.unlocked)
        {
            crystalInsteadOfClone = true;
        }
    }

    protected override void checkUnlock()
    {
        unlockCloneAttack();
        unlockCrystalInstead();
        unlockAggresiveClone();
        unlockMultiClone();
    }
    #endregion
    public void createClone(Transform _clonePosition, Vector3 _offset)
    {
        if (crystalInsteadOfClone)
        {
            SkillManager.instance.crystal.createCrystal();
            return;
        }

        GameObject newClone = Instantiate(clonePrefab); //build a prefab

        newClone.GetComponent<Clone_Skill_Controller>().
            setupClone(_clonePosition, cloneDuration, canAttack, _offset, findClosestEnemy(newClone.transform), canDuplicateClone, chanceToDuplicate, player, attackMultiplier);
    }

    public void createCloneWithDelay(Transform _enemyTransform)
    {
        StartCoroutine(cloneDelayCoroutine(_enemyTransform, new Vector3(2 * player.facingDir, 0)));
    }

    private IEnumerator cloneDelayCoroutine(Transform _transform, Vector3 _offset)
    {
        yield return new WaitForSeconds(.4f);
            createClone(_transform, _offset);
    }
}
