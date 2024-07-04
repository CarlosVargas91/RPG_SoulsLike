using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Freeze enemies effect", menuName = "Data/Item effect/Freeze enemies")] //To create data item menu in unity
public class FreezeEnemiesEffect : ItemEffect
{
    [SerializeField] private float duration;

    public override void executeEffect(Transform _transform)
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        if (playerStats.currentHealth > playerStats.getMaxHealthValue() * .1f) //Not working with more than 10% hp
            return;

        if (!Inventory.instance.canUseArmor())
            return;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(_transform.position, 2);

        foreach (var hit in colliders)
        {
            hit.GetComponent<Enemy>()?.freezeTimeFor(duration);
        }
    }
}
