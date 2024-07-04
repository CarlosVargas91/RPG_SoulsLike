using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Buff effect", menuName = "Data/Item effect/Buff effect")] //To create data item menu in unity
public class BuffEffect : ItemEffect
{
    PlayerStats stats;
    [SerializeField] private StatType buffType;
    [SerializeField] private int buffAmount;
    [SerializeField] private int buffDuration;

    public override void executeEffect(Transform _enemyPosition)
    {
        stats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        stats.increaseStatBy(buffAmount, buffDuration, stats.getStat(buffType));
    }
}
