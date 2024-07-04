using System.Collections;
using UnityEngine;


public enum StatType
{
    strength,
    agility,
    intelligence,
    vitality,
    damage,
    critChance,
    critPower,
    health,
    armor,
    evasion,
    magicRes,
    fireDamage,
    iceDamage,
    lightingDamage,
}
public class CharacterStats : MonoBehaviour
{
    private EntityFX fx;

    [Header("Major stats")]
    public Stat strength;
    public Stat agility;
    public Stat intelligence;
    public Stat vitality;

    [Header("Offensive stats")]
    public Stat damage;
    public Stat critChance;
    public Stat critPower; //default 150%

    [Header("Defensive stats")]
    public Stat maxHealth;
    public Stat armor;
    public Stat evasion;
    public Stat magicResistance;

    [Header("Magic stats")]
    public Stat fireDamage;
    public Stat iceDamage;
    public Stat lightingDamage;

    public bool isIgnited; // does damage over time
    public bool isChilled; // reduce armor by 20%
    public bool isShocked; // reduce accuracy by 20%

    [SerializeField] private float alimentsDuration = 4;

    private float ignitedTimer;
    private float chilledTimer;
    private float shockedTimer;

    private float igniteDamageCoolDown = .3f;
    private float igniteDamageTimer;
    private int igniteDamage;

    [SerializeField] private GameObject schockStrikePreFab;
    private int schockDamage;

    public int currentHealth;

    public System.Action onHealthChanged;
    public bool isDead { get; private set; }
    public bool isInvincible { get; private set; }
    private bool isVulnerable;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        critPower.setDefaultValue(150);
        currentHealth = getMaxHealthValue();

        fx = GetComponent<EntityFX>();
    }

    protected virtual void Update()
    {
        ignitedTimer -= Time.deltaTime;
        chilledTimer -= Time.deltaTime;
        shockedTimer -= Time.deltaTime;
        igniteDamageTimer -= Time.deltaTime;

        if (ignitedTimer < 0)
            isIgnited = false;

        if (chilledTimer < 0)
            isChilled = false;

        if (shockedTimer < 0)
            isShocked = false;

        if (isIgnited)
            applyIgniteDamage();
    }

    public void makeVulnerableFor(float _duration)
    {
        StartCoroutine(vulnerableCoRoutine(_duration));
    }
    private IEnumerator vulnerableCoRoutine(float _duration)
    {
        isVulnerable = true;

        yield return new WaitForSeconds(_duration);

        isVulnerable = false;
    }
    public virtual void increaseStatBy(int _modifier, float _duration, Stat _statToModify)
    {
        StartCoroutine(statModeCoroutine(_modifier, _duration, _statToModify));
    }

    IEnumerator statModeCoroutine(int _modifier, float _duration, Stat _statToModify)
    {
        _statToModify.addModifier(_modifier);

        yield return new WaitForSeconds(_duration);

        _statToModify.removeModifier(_modifier);
    }
    public virtual void doDamage(CharacterStats _targetStats)
    {
        bool criticalStrike = false;

        if (_targetStats.isInvincible)
            return;

        if (targetCanAvoidAttack(_targetStats))
            return;

        _targetStats.GetComponent<Entity>().setupKnockBackDirection(transform);

        int totalDamage = damage.getValue() + strength.getValue();

        if (canCrit())
        {
            totalDamage = calculateCriticalDamage(totalDamage);
            criticalStrike = true;
        }

        fx.createHitFX(_targetStats.transform, criticalStrike);

        totalDamage = checkTargetArmor(_targetStats, totalDamage);
        _targetStats.takeDamage(totalDamage);

        doMagicalDamage(_targetStats); //remove if dont want to apply magi hit in primary attack
    }

    #region Magical damage and aliments
    public virtual void doMagicalDamage(CharacterStats _targetStats)
    {
        int lfireDamage = fireDamage.getValue();
        int liceDamage = iceDamage.getValue();
        int llightingDamage = lightingDamage.getValue();

        int totalMagicalDamage = lfireDamage + liceDamage + llightingDamage + intelligence.getValue();

        totalMagicalDamage = checkTargetResistance(_targetStats, totalMagicalDamage);

        _targetStats.takeDamage(totalMagicalDamage);

        if (Mathf.Max(lfireDamage, liceDamage, llightingDamage) <= 0) //if all of them are zero
            return;

        attemptToApplyAliments(_targetStats, lfireDamage, liceDamage, llightingDamage);

    }

    private void attemptToApplyAliments(CharacterStats _targetStats, int lfireDamage, int liceDamage, int llightingDamage)
    {
        bool canApplyIgnite = lfireDamage > liceDamage && lfireDamage > llightingDamage;
        bool canApplyChill = liceDamage > lfireDamage && liceDamage > llightingDamage;
        bool canAppliSchock = llightingDamage > lfireDamage && llightingDamage > liceDamage;

        while (!canApplyIgnite && !canApplyChill && !canAppliSchock) //In case all of the elements
        {
            if (Random.value < .5f && lfireDamage > 0)
            {
                canApplyIgnite = true;
                _targetStats.applyAliments(canApplyIgnite, canApplyChill, canAppliSchock);
                return;
            }

            if (Random.value < .5f && liceDamage > 0)
            {
                canApplyChill = true;
                _targetStats.applyAliments(canApplyIgnite, canApplyChill, canAppliSchock);
                return;
            }

            if (Random.value < .5f && llightingDamage > 0)
            {
                canAppliSchock = true;
                _targetStats.applyAliments(canApplyIgnite, canApplyChill, canAppliSchock);
                return;
            }

        }

        if (canApplyIgnite)
            _targetStats.setupIgniteDamage(Mathf.RoundToInt(lfireDamage * .2f));

        if (canAppliSchock)
            _targetStats.setupShockStrikeDamage(Mathf.RoundToInt(llightingDamage * .1f));

        _targetStats.applyAliments(canApplyIgnite, canApplyChill, canAppliSchock);
    }

    public void setupIgniteDamage(int _damage) => igniteDamage = _damage;
    public void setupShockStrikeDamage(int _damage) => schockDamage = _damage;
    public void applyAliments(bool _ignite, bool _chill, bool _schok)
    {
        bool canApplyIgnite = !isIgnited && !isChilled && !isShocked;
        bool canApplyChill = !isIgnited && !isChilled && !isShocked;
        bool canApplySchock = !isIgnited && !isChilled;

        if (_ignite && canApplyIgnite)
        {
            isIgnited = _ignite;
            ignitedTimer = alimentsDuration;

            fx.igniteFxFor(alimentsDuration);
        }

        if (_chill && canApplyChill)
        {
            isChilled = _chill;
            chilledTimer = alimentsDuration;

            float slowPercentage = .2f;

            GetComponent<Entity>().slowEntityBy(slowPercentage, alimentsDuration);
            fx.chillFxFor(alimentsDuration);
        }

        if (_schok && canApplySchock)
        {
            if (!isShocked)
            {
                applyShock(_schok);
            }
            else
            {
                if (GetComponent<Player>() != null) //To avoid the enemy to hit himself with thunder
                    return;

                hitNearestTargetWithShockStrike();
            }
        }

    }

    public void applyShock(bool _schok)
    {
        if (isShocked)
            return;

        isShocked = _schok;
        shockedTimer = alimentsDuration;

        fx.schockFxFor(alimentsDuration);
    }

    private void hitNearestTargetWithShockStrike()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 25);

        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null && Vector2.Distance(transform.position, hit.transform.position) > 1)
            {
                float distanceToEnemy = Vector2.Distance(transform.position, hit.transform.position);

                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = hit.transform;
                }
            }

            if (closestEnemy == null)
                closestEnemy = transform;
        }

        if (closestEnemy != null)
        {
            GameObject newSchockStrike = Instantiate(schockStrikePreFab, transform.position, Quaternion.identity);

            newSchockStrike.GetComponent<ShockStrikeController>().Setup(schockDamage, closestEnemy.GetComponent<CharacterStats>());
        }
    }
    private void applyIgniteDamage()
    {
        if (igniteDamageTimer < 0)
        {
            decreaseHealthBy(igniteDamage);

            if (currentHealth < 0 && !isDead)
                die();

            igniteDamageTimer = igniteDamageCoolDown;
        }
    }

    #endregion
    public virtual void takeDamage(int _damage)
    {
        if (isInvincible)
            return;

        decreaseHealthBy(_damage);

        GetComponent<Entity>().damageImpact();
        fx.StartCoroutine("FlashFX");

        if (currentHealth < 0 && !isDead)
            die();
    }

    public virtual void incereaseHealthBy(int _amount)
    {
        currentHealth += _amount;

        if (currentHealth > getMaxHealthValue())
            currentHealth = getMaxHealthValue();

        if (onHealthChanged != null)
            onHealthChanged();
    }
    protected virtual void decreaseHealthBy(int _damage)
    {
        if (isVulnerable)
            _damage = Mathf.RoundToInt(_damage * 1.1f);

        currentHealth -= _damage;

        if (_damage > 0)
            fx.createPopUpText(_damage.ToString());

        if (onHealthChanged != null)
            onHealthChanged();
    }
    protected virtual void die()
    {
        isDead = true;
    }

    public void killEntity()
    {
        if (!isDead)
            die();
    }

    public void makeInvincible(bool _invincible) => isInvincible = _invincible;
    #region Stat calculation

    public virtual void onEvasion()
    {

    }
    protected bool targetCanAvoidAttack(CharacterStats _targetStats)
    {
        int totalEvasion = _targetStats.evasion.getValue() + _targetStats.agility.getValue();

        if (isShocked)
            totalEvasion += 20;

        if (Random.Range(0, 100) < totalEvasion)
        {
            _targetStats.onEvasion();
            return true;
        }

        return false;
    }
    private int checkTargetResistance(CharacterStats _targetStats, int totalMagicalDamage)
    {
        totalMagicalDamage -= _targetStats.magicResistance.getValue() + (_targetStats.intelligence.getValue() * 3);

        totalMagicalDamage = Mathf.Clamp(totalMagicalDamage, 0, int.MaxValue); //avoid negative value

        return totalMagicalDamage;
    }
    protected int checkTargetArmor(CharacterStats _targetStats, int totalDamage)
    {
        if (_targetStats.isChilled)
            totalDamage -= Mathf.RoundToInt(_targetStats.armor.getValue() * .8f);
        else
            totalDamage -= _targetStats.armor.getValue();

        totalDamage = Mathf.Clamp(totalDamage, 0, int.MaxValue); //to avoid problems when armor is bigger than health
        return totalDamage;
    }

    protected bool canCrit()
    {
        int totalCriticalChance = critChance.getValue() + agility.getValue();

        if (Random.Range(0, 100) <= totalCriticalChance)
        {
            return true;
        }

        return false;
    }

    protected int calculateCriticalDamage(int _damage)
    {
        float totalCritPower = (critPower.getValue() + strength.getValue()) * .01f; //Percentage

        float critDamage = _damage * totalCritPower;

        return Mathf.RoundToInt(critDamage);
    }

    public int getMaxHealthValue()
    {
        return maxHealth.getValue() + vitality.getValue() * 5;
    }

    #endregion

    public Stat getStat(StatType _statType)
    {
        if (_statType == StatType.strength) return strength;
        else if (_statType == StatType.agility) return agility;
        else if (_statType == StatType.intelligence) return intelligence;
        else if (_statType == StatType.magicRes) return magicResistance;
        else if (_statType == StatType.fireDamage) return fireDamage;
        else if (_statType == StatType.iceDamage) return iceDamage;
        else if (_statType == StatType.agility) return agility;
        else if (_statType == StatType.vitality) return vitality;
        else if (_statType == StatType.armor) return armor;
        else if (_statType == StatType.evasion) return evasion;
        else if (_statType == StatType.lightingDamage) return lightingDamage;
        else if (_statType == StatType.critChance) return critChance;
        else if (_statType == StatType.critPower) return critPower;
        else if (_statType == StatType.health) return maxHealth;
        else if (_statType == StatType.damage) return damage;

        return null;
    }
}
