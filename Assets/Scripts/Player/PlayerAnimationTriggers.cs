using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationTriggers : MonoBehaviour
{
    private Player player => /*return*/ GetComponentInParent<Player>();
    private void AnimationTrigger()
    {
        player.AnimationTrigger();
    }

    private void AttackTrigger()
    {
        AudioManager.instance.playSFX(2, null); //audio of attack
        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);

        foreach(var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                EnemyStats _target = hit.GetComponent<EnemyStats>();

                if(_target != null)
                    player.stats.doDamage(_target);

                // inventory get weapom call item effect
                ItemData_Equipment weaponData = Inventory.instance.getEquipment(EquipmentType.Weapon);

                if (weaponData != null)
                    weaponData.effect(_target.transform);
            }
        }
    }
    private void throwSword()
    {
        SkillManager.instance.sword.createSword();
    }
}
