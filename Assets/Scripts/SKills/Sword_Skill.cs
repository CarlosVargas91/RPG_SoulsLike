using System;
using UnityEngine;
using UnityEngine.UI;

public enum SwordType
{
    Regular,
    Bounce,
    Pierce,
    Spin
}

public class Sword_Skill : Skill
{
    public SwordType swordType = SwordType.Regular;

    [Header("Bounce info")]
    [SerializeField] private UI_SkillTreeSlot bounceUnlockButton;
    [SerializeField] private int bounceAmount;
    [SerializeField] private float bounceGravity;
    [SerializeField] private float bounceSpeed;

    [Header("Pierce info")]
    [SerializeField] private UI_SkillTreeSlot pierceUnlockButton;
    [SerializeField] private int pierceAmount;
    [SerializeField] private float pierceGravity;

    [Header("Spin info")]
    [SerializeField] private UI_SkillTreeSlot spinUnlockButton;
    [SerializeField] private float hitCoolDown = .35f;
    [SerializeField] private float maxTravelDistance = 7;
    [SerializeField] private float spinDuration =2;
    [SerializeField] private float spinGravity = 1;

    [Header("Skill info")]
    [SerializeField] private UI_SkillTreeSlot swordUnlockButton;
    public bool swordUnlocked { get; private set; }
    [SerializeField] private GameObject swordPrefab;
    [SerializeField] private Vector2 launchForce;
    [SerializeField] private float swordGravity;
    [SerializeField] private float freezeTimeDuration;
    [SerializeField] private float returnSpeed;

    [Header("Passive skilss")]
    [SerializeField] private UI_SkillTreeSlot timeStopUnlockButton;
    public bool timeStopUnlocked { get; private set; }
    [SerializeField] private UI_SkillTreeSlot vulnerableUnlockButton;
    public bool vulnerableUnlocked {  get; private set; }

    private Vector2 finalDir;

    [Header("Aim dots")]
    [SerializeField] private int numberOfDots;
    [SerializeField] private float spaceBetweenDots;
    [SerializeField] private GameObject dotPrefab;
    [SerializeField] private Transform dotsParent;


    private GameObject[] dots;

    protected override void Start()
    {
        base.Start();

        generateDots();

        setupGravity();

        swordUnlockButton.GetComponent<Button>().onClick.AddListener(unlockSword);
        timeStopUnlockButton.GetComponent<Button>().onClick.AddListener(unlockTimeStop);
        vulnerableUnlockButton.GetComponent<Button>().onClick.AddListener(unlockVulnerable);
        bounceUnlockButton.GetComponent<Button>().onClick.AddListener(unlockBounceSword);
        pierceUnlockButton.GetComponent<Button>().onClick.AddListener(unlockPierceSword);
        spinUnlockButton.GetComponent<Button>().onClick.AddListener(unlockSpinSword);
    }
    protected override void Update()
    {
        if (Input.GetKeyUp(KeyCode.Mouse1))
            finalDir = new Vector2(aimDirection().normalized.x * launchForce.x, aimDirection().normalized.y * launchForce.y);

        if (Input.GetKey(KeyCode.Mouse1))
        {
            for (int i = 0; i < dots.Length; i++)
            {
                dots[i].transform.position = dotsPosition(i * spaceBetweenDots);
            }
        }
    }

    private void setupGravity()
    {
        if (swordType == SwordType.Bounce)
            swordGravity = bounceGravity;
        else if (swordType == SwordType.Pierce)
            swordGravity = pierceGravity;
        else if (swordType == SwordType.Spin)
            swordGravity = spinGravity;
    }

    public void createSword()
    {
        GameObject newSword = Instantiate(swordPrefab, player.transform.position, transform.rotation);
        Sword_Skill_Controller newSwordScript = newSword.GetComponent<Sword_Skill_Controller>();

        if (swordType == SwordType.Bounce)
            newSwordScript.setupBounce(true, bounceAmount, bounceSpeed);
        else if (swordType == SwordType.Pierce)
            newSwordScript.setupPierce(pierceAmount);
        else if (swordType == SwordType.Spin)
            newSwordScript.setupSpin(true, maxTravelDistance, spinDuration, hitCoolDown);

        newSwordScript.setupSword(finalDir, swordGravity, player, freezeTimeDuration, returnSpeed);

        player.assignNewSword(newSword);

        dotsActive(false);
    }

    #region Unlock
    private void unlockTimeStop()
    {
        if (timeStopUnlockButton.unlocked)
            timeStopUnlocked = true;
    }

    private void unlockVulnerable()
    {
        if (vulnerableUnlockButton.unlocked)
            vulnerableUnlocked = true;
    }

    private void unlockSword()
    {
        if (swordUnlockButton.unlocked)
        {
            swordType = SwordType.Regular;
            swordUnlocked = true;
        }
    }

    private void unlockBounceSword()
    {
        if (bounceUnlockButton.unlocked)
            swordType = SwordType.Bounce;
    }

    private void unlockPierceSword()
    {
        if (pierceUnlockButton.unlocked)
            swordType = SwordType.Pierce;
    }

    private void unlockSpinSword()
    {
        if (spinUnlockButton.unlocked)
            swordType = SwordType.Spin;
    }

    protected override void checkUnlock()
    {
        unlockSword();
        unlockSpinSword();
        unlockPierceSword();
        unlockBounceSword();
        unlockTimeStop();
        unlockVulnerable();
    }
    #endregion
    #region Aim
    public Vector2 aimDirection()
    {
        Vector2 playerPosition = player.transform.position;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePosition - playerPosition;

        return direction;
    }

    public void dotsActive(bool _isActive)
    {
        for (int i = 0; i < dots.Length; i++)
        {
            dots[i].SetActive(_isActive);
        }
    }
    private void generateDots()
    {
        dots = new GameObject[numberOfDots];

        for (int i = 0; i < numberOfDots; i++)
        {
            dots[i] = Instantiate(dotPrefab, player.transform.position, Quaternion.identity, dotsParent);
            dots[i].SetActive(false);
        }
    }

    private Vector2 dotsPosition(float t)
    {
        Vector2 position = (Vector2)player.transform.position + new Vector2(
            aimDirection().normalized.x * launchForce.x,
            aimDirection().normalized.y * launchForce.y) * t + .5f * (Physics2D.gravity * swordGravity) * (t * t);

        return position;
    }
    #endregion
}
