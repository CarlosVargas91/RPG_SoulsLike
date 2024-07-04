using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_InGame : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Slider slider;

    [SerializeField] private Image dashImage;
    [SerializeField] private Image parryImage;
    [SerializeField] private Image crystalImage;
    [SerializeField] private Image swordImage;
    [SerializeField] private Image blakcHoleImage;
    [SerializeField] private Image flaskImage;

    private SkillManager skills;

    [Header("Souls info")]
    [SerializeField] private TextMeshProUGUI currentSouls;
    [SerializeField] private float soulsAmount;
    [SerializeField] private float increaseRate = 100;

    // Start is called before the first frame update
    void Start()
    {
        if (playerStats != null)
            playerStats.onHealthChanged += updateHealthUI;

        skills = SkillManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        updateSoulsUI();

        if (Input.GetKeyDown(KeyCode.LeftShift) && skills.dash.dashUnlocked)
            setCoolDownOf(dashImage);

        if (Input.GetKeyDown(KeyCode.Q) && skills.parry.parryUnlocked)
            setCoolDownOf(parryImage);

        if (Input.GetKeyDown(KeyCode.F) && skills.crystal.crystalUnlocked)
            setCoolDownOf(crystalImage);

        if (Input.GetKeyDown(KeyCode.Mouse1) && skills.sword.swordUnlocked)
            setCoolDownOf(swordImage);

        if (Input.GetKeyDown(KeyCode.R) && skills.blackhole.blackholeUnlocked)
            setCoolDownOf(blakcHoleImage);

        if (Input.GetKeyDown(KeyCode.Alpha1) && Inventory.instance.getEquipment(EquipmentType.Flask) != null)
            setCoolDownOf(flaskImage);

        checkCoolDownOf(dashImage, skills.dash.coolDown);
        checkCoolDownOf(parryImage, skills.parry.coolDown);
        checkCoolDownOf(crystalImage, skills.crystal.coolDown);
        checkCoolDownOf(swordImage, skills.sword.coolDown);
        checkCoolDownOf(blakcHoleImage, skills.blackhole.coolDown);
        checkCoolDownOf(flaskImage, Inventory.instance.flaskCooldown);
    }

    private void updateSoulsUI()
    {
        if (soulsAmount < PlayerManager.instance.getCurrentCurrency())
            soulsAmount += Time.deltaTime * increaseRate;
        else
            soulsAmount = PlayerManager.instance.getCurrentCurrency();



        //currentSouls.text = PlayerManager.instance.getCurrentCurrency().ToString("#,#"); //to format style
        currentSouls.text = ((int)soulsAmount).ToString();
    }

    private void updateHealthUI()
    {
        slider.maxValue = playerStats.getMaxHealthValue();
        slider.value = playerStats.currentHealth;
    }

    private void setCoolDownOf(Image _image)
    {
        if (_image.fillAmount <= 0)
            _image.fillAmount = 1;
    }

    private void checkCoolDownOf(Image _image, float _cooldown)
    {
        if (_image.fillAmount > 0)
            _image.fillAmount -= 1 / _cooldown * Time.deltaTime;
    }
}
