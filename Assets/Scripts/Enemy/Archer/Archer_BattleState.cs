using UnityEngine;

public class Archer_BattleState : EnemyState
{
    private Transform player;
    private Enemy_archer enemy;
    private int moveDir;

    public Archer_BattleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_archer _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        player = PlayerManager.instance.player.transform;

        if (player.GetComponent<PlayerStats>().isDead)
            stateMachine.ChangeState(enemy.moveState);
    }
    public override void Update()
    {
        base.Update();

        if (enemy.isPlayerDetected())
        {
            stateTimer = enemy.battleTime;

            if (enemy.isPlayerDetected().distance < enemy.safeDistance)
            {
                if (canJump())
                    stateMachine.ChangeState(enemy.jumpState);
            }

            if (enemy.isPlayerDetected().distance < enemy.attackDistance)
            {
                if (canAttack())
                    stateMachine.ChangeState(enemy.attackState);
            }
        }
        else
        {
            if (stateTimer < 0 || Vector2.Distance(player.transform.position, enemy.transform.position) > 7)
                stateMachine.ChangeState(enemy.idleState);
        }

        battleStateFlipControl();
    }

    private void battleStateFlipControl()
    {
        if (player.position.x > enemy.transform.position.x && enemy.facingDir == -1) //Enemy is to the right
            enemy.Flip();
        else if (player.position.x < enemy.transform.position.x && enemy.facingDir == 1) // To the left
            enemy.Flip();
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

    private bool canJump()
    {
        if (enemy.GroundBehind() == false || enemy.WallBehind() == true)
            return false;

        if (Time.time >= enemy.lastTimeJump + enemy.jumpCoolDown)
        {
            enemy.lastTimeJump = Time.time;
            return true;
        }

        return false;
    }
}