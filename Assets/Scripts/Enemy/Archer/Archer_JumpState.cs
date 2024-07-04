using System.Collections;
using UnityEngine;

public class Archer_JumpState : EnemyState
{
    private Enemy_archer enemy;
    public Archer_JumpState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_archer _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        AudioManager.instance.playSFX(15, enemy.transform);
        rb.velocity = new Vector2(enemy.jumpVelocity.x * -enemy.facingDir, enemy.jumpVelocity.y);
    }

    public override void Update()
    {
        base.Update();

        enemy.anim.SetFloat("yVelocity", rb.velocity.y);

        if (rb.velocity.y < 0 && enemy.isGroundDetected())
            stateMachine.ChangeState(enemy.battleState);
    }
}