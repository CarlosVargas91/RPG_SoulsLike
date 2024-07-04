using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_archer : Enemy
{
    [Header("Archer info")]
    [SerializeField] private GameObject arrowPreFab;
    [SerializeField] private float arrowSpeed;
    [SerializeField] public Vector2 jumpVelocity;
    public float jumpCoolDown;
    public float safeDistance;
    [HideInInspector] public float lastTimeJump;

    [Header("Aditional collision check")]
    [SerializeField] private Transform groundBehindCheck;
    [SerializeField] private Vector2 groundBehindCheckSize;

    #region States
    public Archer_IdleState idleState { get; private set; }
    public Archer_MoveState moveState { get; private set; }
    public Archer_BattleState battleState { get; private set; }
    public Archer_AttackState attackState { get; private set; }
    public Archer_StunnedState stunnedState { get; private set; }
    public Archer_DeadState deadState { get; private set; }
    public Archer_JumpState jumpState { get; private set; }
    #endregion
    protected override void Awake()
    {
        base.Awake();

        idleState = new Archer_IdleState(this, stateMachine, "Idle", this);
        moveState = new Archer_MoveState(this, stateMachine, "Move", this);
        battleState = new Archer_BattleState(this, stateMachine, "Idle", this);
        attackState = new Archer_AttackState(this, stateMachine, "Attack", this);
        stunnedState = new Archer_StunnedState(this, stateMachine, "Stunned", this);
        deadState = new Archer_DeadState(this, stateMachine, "Idle", this); //Does not matter the name
        jumpState = new Archer_JumpState(this, stateMachine, "Jump", this);
    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(idleState);
    }

    public override bool canBeStunnedFunction()
    {
        if (base.canBeStunnedFunction())
        {
            stateMachine.ChangeState(stunnedState);
            return true;
        }

        return false;
    }

    public override void die()
    {
        base.die();

        stateMachine.ChangeState(deadState);
    }

    public override void animationSpecialAttackTrigger()
    {
        GameObject newArrow = Instantiate(arrowPreFab, attackCheck.position, Quaternion.identity);

        newArrow.GetComponent<Arrow_Controller>().setupArrow(arrowSpeed * facingDir, stats);
    }

    public bool GroundBehind() => Physics2D.BoxCast(groundBehindCheck.position, groundBehindCheckSize, 0, Vector2.zero,0, whatIsGround);
    public bool WallBehind() => Physics2D.Raycast(wallCheck.position, Vector2.right * -facingDir, wallCheckDistance + 2, whatIsGround);

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.DrawWireCube(groundBehindCheck.position, groundBehindCheckSize);
    }
}
