using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBringer_BattleState : EnemyState
{
    private Enemy_DeathBringer enemy;
    private Transform player;
    private int moveDir;
    public DeathBringer_BattleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_DeathBringer _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        player = PlayerManager.instance.player.transform;

        //if (player.GetComponent<PlayerStats>().isDead)
        //stateMachine.ChangeState(enemy.moveState);
    }
    public override void Update()
    {
        base.Update();

        if (enemy.isPlayerDetected())
        {
            //stateTimer = enemy.battleTime;

            if (enemy.isPlayerDetected().distance < enemy.attackDistance)
            {
                if (canAttack())
                    stateMachine.ChangeState(enemy.attackState);
                else
                    stateMachine.ChangeState(enemy.idleState);
            }
        }

        if (player.position.x > enemy.transform.position.x) //Enemy is to the right
            moveDir = 1;
        else if (player.position.x < enemy.transform.position.x) // To the left
            moveDir = -1;

        if (enemy.isPlayerDetected() && enemy.isPlayerDetected().distance < enemy.attackDistance - .5f)
            return;

        enemy.SetVelocity(enemy.moveSpeed * moveDir, rb.velocity.y);
    }

    public override void Exit()
    {
        base.Exit();
    }

    private bool canAttack()
    {
        if (Time.time >= enemy.lastTimeAttacked + enemy.attackCooldown)
        {
            enemy.attackCooldown = Random.Range(enemy.minAttackCooldown, enemy.maxAttackCooldown);

            enemy.lastTimeAttacked = Time.time;
            return true;
        }

        return false;
    }

}
