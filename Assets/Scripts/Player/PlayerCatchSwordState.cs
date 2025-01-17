using UnityEngine;

public class PlayerCatchSwordState : PlayerState
{
    private Transform sword;
    public PlayerCatchSwordState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.fx.playDustFX();
        player.fx.screenShakeFunction(player.fx.shakeSwordImpact);

        sword = player.sword.transform;

        if (player.transform.position.x > sword.position.x && player.facingDir == 1)// Is to the right
            player.Flip();
        else if (player.transform.position.x < sword.position.x && player.facingDir == -1) //to the left
            player.Flip();

        rb.velocity = new Vector2(player.swordReturnImpact * -player.facingDir, rb.velocity.y);
    }

    public override void Exit()
    {
        base.Exit();

        player.StartCoroutine("BusyFor", .1f);
    }

    public override void Update()
    {
        base.Update();

        if (triggerCalled)
            stateMachine.ChangeState(player.idleState);
    }
}
