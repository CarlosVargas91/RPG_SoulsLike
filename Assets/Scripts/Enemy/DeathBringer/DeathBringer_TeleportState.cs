using UnityEngine;


public class DeathBringer_TeleportState : EnemyState
{
    private Enemy_DeathBringer enemy;
    public DeathBringer_TeleportState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_DeathBringer _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.stats.makeInvincible(true);
    }

    public override void Update()
    {
        base.Update();
        
        if (triggerCalled)
        {
            if (enemy.CanDoSpellCast())
                stateMachine.ChangeState(enemy.spellCastState);
            else
                stateMachine.ChangeState(enemy.battleState);
        }
        
        
    }

    public override void Exit()
    {
        base.Exit();

        enemy.stats.makeInvincible(false);
    }
}