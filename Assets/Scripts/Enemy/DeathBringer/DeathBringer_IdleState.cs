using UnityEngine;

public class DeathBringer_IdleState : EnemyState
{
    private Enemy_DeathBringer enemy;
    private Transform player;
    private bool audioPlay = false;
    public DeathBringer_IdleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_DeathBringer _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = enemy.idleTime;
        player = PlayerManager.instance.player.transform;
    }

    public override void Exit()
    {
        base.Exit();
        AudioManager.instance.playSFX(19, enemy.transform);
    }

    public override void Update()
    {
        base.Update();

        if (Vector2.Distance(player.transform.position, enemy.transform.position) < 7)
        {
            audioPlay = true;
            enemy.bossFightBegun = true;
        }

        //if (Input.GetKeyDown(KeyCode.V))
            //stateMachine.ChangeState(enemy.teleportState);

        if (audioPlay)
        {
            AudioManager.instance.playBGMFunction(3);
            audioPlay = false;
        }

        if (stateTimer < 0 && enemy.bossFightBegun)
            stateMachine.ChangeState(enemy.battleState);
    }
}