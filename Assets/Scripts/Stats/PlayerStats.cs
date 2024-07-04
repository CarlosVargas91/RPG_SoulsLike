using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    private Player player;
    protected override void Start()
    {
        base.Start();

        player = GetComponent<Player>();
    }

    public override void takeDamage(int _damage)
    {
        base.takeDamage(_damage);
    }

    protected override void die()
    {
        base.die();
        player.die();

        GameManager.instance.lostCurrencyAmount = PlayerManager.instance.currency;
        PlayerManager.instance.currency = 0;

        GetComponent<PlayerItemDrop>()?.generateDrop();
    }

    protected override void decreaseHealthBy(int _damage)
    {
        base.decreaseHealthBy(_damage);

        if (_damage > getMaxHealthValue() * .3f)
        {
            player.setupKnockbarPower(new Vector2(10,6));
            player.fx.screenShakeFunction(player.fx.shakeHighDamage);

            int randomSound = Random.Range(34, 35);
            AudioManager.instance.playSFX(randomSound, null);
        }

        ItemData_Equipment currentArmor = Inventory.instance.getEquipment(EquipmentType.Armor);

        if (currentArmor != null)
            currentArmor.effect(player.transform);
    }

    public override void onEvasion()
    {
        player.skill.dodge.createMirageOnDodge();
    }

    public void cloneDoDamage(CharacterStats _targetStats, float _attackMultiplier)
    {
        if (targetCanAvoidAttack(_targetStats))
            return;

        int totalDamage = damage.getValue() + strength.getValue();

        if (_attackMultiplier > 0)
            totalDamage = Mathf.RoundToInt(totalDamage * _attackMultiplier);

        if (canCrit())
        {
            totalDamage = calculateCriticalDamage(totalDamage);
        }

        totalDamage = checkTargetArmor(_targetStats, totalDamage);
        _targetStats.takeDamage(totalDamage);

        doMagicalDamage(_targetStats); //remove if dont want to apply magi hit in primary attack
    }
}
