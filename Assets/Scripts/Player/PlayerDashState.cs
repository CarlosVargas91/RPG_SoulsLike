using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerState
{
    public PlayerDashState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.skill.dash.cloneOnDash();

        stateTimer = player.dashDuration;

        player.stats.makeInvincible(true);

    }

    public override void Exit()
    {
        base.Exit();

        player.skill.dash.cloneOnArrival();

        player.SetVelocity(0, rb.velocity.y);

        player.stats.makeInvincible(false);
    }

    public override void Update()
    {
        base.Update();

        if (!player.isGroundDetected() && player.isWallDetected())
            stateMachine.ChangeState(player.wallSlide);

        player.SetVelocity(player.dashSpeed * player.dashDir, 0);

        if (stateTimer < 0)
            stateMachine.ChangeState(player.idleState);

        player.fx.createAfterImage();
    }
}
