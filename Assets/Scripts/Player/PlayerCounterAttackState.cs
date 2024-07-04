using UnityEngine;

public class PlayerCounterAttackState : PlayerState
{
    private bool canCreateClone;
    public PlayerCounterAttackState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        canCreateClone = true;
        stateTimer = player.counterAttackDuration;
        player.anim.SetBool("SuccesfulCounterAttack", false);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        player.SetZeroVelocity();

        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Arrow_Controller>() != null)
            {
                hit.GetComponent<Arrow_Controller>().flipArrow();
                successfuleCounterAttack();
            }

            if (hit.GetComponent<Enemy>() != null)
            {

                if (hit.GetComponent<Enemy>().canBeStunnedFunction())
                {
                    successfuleCounterAttack();

                    player.skill.parry.useSKill(); //to restore health on parry

                    if (canCreateClone)
                    {
                        canCreateClone = false;
                        player.skill.parry.makeMirageOnParry(hit.transform);
                    }
                }
            }
        }

        if (stateTimer < 0 || triggerCalled)
            stateMachine.ChangeState(player.idleState);
    }

    private void successfuleCounterAttack()
    {
        stateTimer = 10; //any value bigger than 1
        player.anim.SetBool("SuccesfulCounterAttack", true);
    }
}
