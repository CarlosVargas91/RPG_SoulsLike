using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_DetahBringerTriggers : Enemy_AnimationTriggers
{
    private Enemy_DeathBringer enemyDeathBringer => GetComponentInParent<Enemy_DeathBringer>();

    private void Relocate() => enemyDeathBringer.FindPosition();

    private void MakeInvisible() => enemyDeathBringer.fx.makeTransparent(true);
    private void MakeVisible() => enemyDeathBringer.fx.makeTransparent(false);
}
