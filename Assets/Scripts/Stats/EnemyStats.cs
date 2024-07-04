using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : CharacterStats
{
    private Enemy enemy;
    private ItemDrop myDropSystem;
    public Stat soulsDropAmount;

    [Header("Level details")]
    [SerializeField] private int level = 1;

    [Range(0f, 1f)]
    [SerializeField] private float percentageModifier = .4f;
    protected override void Start()
    {
        soulsDropAmount.setDefaultValue(100);
        applyLevelModifier();

        base.Start();

        enemy = GetComponent<Enemy>();
        myDropSystem = GetComponent<ItemDrop>();

    }

    private void applyLevelModifier()
    {
        modify(strength);
        modify(agility);
        modify(intelligence);
        modify(vitality);

        modify(damage);
        modify(critChance);
        modify(critPower);

        modify(maxHealth);
        modify(armor);
        modify(evasion);
        modify(magicResistance);

        modify(fireDamage);
        modify(iceDamage);
        modify(lightingDamage);

        modify(soulsDropAmount);
    }

    private void modify(Stat _stat)
    {
        for (int i = 1; i < level; i++)
        {
            float modifier = _stat.getValue() * percentageModifier;

            _stat.addModifier(Mathf.RoundToInt(modifier));
        }
    }
    public override void takeDamage(int _damage)
    {
        base.takeDamage(_damage);
    }

    protected override void die()
    {
        base.die();
        enemy.die();

        PlayerManager.instance.currency += soulsDropAmount.getValue();
        myDropSystem.generateDrop();

        Destroy(gameObject, 5f);
    }
}
