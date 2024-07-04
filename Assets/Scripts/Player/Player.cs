using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Player : Entity
{
    [Header("Attack details")]
    public Vector2[] attackMovement;
    public float counterAttackDuration = .2f;
    public bool isBusy {  get; private set; }

    [Header("Move info")]
    public float moveSpeed = 12f;
    public float jumpForce;
    public float swordReturnImpact;
    private float defaultMoveSPeed;
    private float defaultJumpForce;
    private float defaultDashSPeed;

    [Header("Dash info")]
    public float dashSpeed;
    public float dashDuration;
    public float dashDir {  get; private set; }

    public SkillManager skill {  get; private set; }
    public GameObject sword { get; private set; }
    public PlayerFX fx { get; private set; }
    #region States
    public PlayerStateMachine stateMachine {  get; private set; }
    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerAirState airState { get; private set; }
    public PlayerWallSlideState wallSlide { get; private set; }
    public PlayerDashState dashState { get; private set; }
    public PlayerWallJumpState wallJump { get; private set; }
    public PlayerPrimaryAttackState primaryAttack { get; private set; }
    public PlayerCounterAttackState counterAttack { get; private set; }
    public PlayerAimSwordState aimSword { get; private set; }
    public PlayerCatchSwordState catchSword { get; private set; }
    public PlayerBlackHoleState blackHole { get; private set; }
    public PlayerDeadState deadState { get; private set; }

    #endregion
    protected override void Awake()
    {
        base.Awake();

        stateMachine = new PlayerStateMachine();

        idleState = new PlayerIdleState(this, stateMachine, "Idle");
        moveState = new PlayerMoveState(this, stateMachine, "Move");
        jumpState = new PlayerJumpState(this, stateMachine, "Jump");
        airState  = new PlayerAirState(this, stateMachine, "Jump");
        dashState = new PlayerDashState(this, stateMachine, "Dash");
        wallSlide = new PlayerWallSlideState(this, stateMachine, "WallSlide");
        wallJump  = new PlayerWallJumpState(this, stateMachine, "Jump");

        primaryAttack = new PlayerPrimaryAttackState(this, stateMachine, "Attack");
        counterAttack = new PlayerCounterAttackState(this, stateMachine, "CounterAttack");

        aimSword = new PlayerAimSwordState(this, stateMachine, "AimSword");
        catchSword = new PlayerCatchSwordState(this, stateMachine, "CatchSword");
        blackHole = new PlayerBlackHoleState(this, stateMachine, "Jump");

        deadState = new PlayerDeadState(this, stateMachine, "Die");
    }

    protected override void Start()
    {
        base.Start();

        fx = GetComponent<PlayerFX>();
        skill = SkillManager.instance;

        stateMachine.Initialize(idleState); //First state

        defaultMoveSPeed = moveSpeed;
        defaultJumpForce = jumpForce;
        defaultDashSPeed = dashSpeed;
    }

    protected override void Update()
    {
        if (Time.timeScale == 0)
            return;

        base.Update();

        stateMachine.currentState.Update();

        CheckForDashInput();

        if (Input.GetKeyDown(KeyCode.F) && skill.crystal.crystalUnlocked)
            skill.crystal.canUseSKill();

        if (Input.GetKeyDown(KeyCode.Alpha1))
            Inventory.instance.useFlask();
    }

    public override void slowEntityBy(float _slowPercentage, float _slowDuration)
    {
        moveSpeed = moveSpeed * (1 - _slowPercentage); //to get perecentage 
        jumpForce = jumpForce * (1 - _slowPercentage);
        dashSpeed = dashSpeed * (1 - _slowPercentage);

        anim.speed = anim.speed * (1 - _slowPercentage);

        Invoke("returnDefaultSpeed", _slowDuration);
    }

    protected override void returnDefaultSpeed()
    {
        base.returnDefaultSpeed();

        moveSpeed = defaultMoveSPeed;
        jumpForce = defaultJumpForce;
        dashSpeed = defaultDashSPeed;
    }
    public void assignNewSword(GameObject _newSword)
    {
        sword = _newSword;
    }

    public void catchTheSword()
    {
        stateMachine.ChangeState(catchSword);
        Destroy(sword);
    }
    public IEnumerator BusyFor(float _seconds)
    {
        isBusy = true;

        yield return new WaitForSeconds(_seconds);

        isBusy = false;
    }

    public void AnimationTrigger() => stateMachine.currentState.AnimationTrigger(); // Call of animation trigger
    private void CheckForDashInput()
    {
        if (isWallDetected())
            return; // not able to dash in the wall

        if (skill.dash.dashUnlocked == false)
            return;

        if (Input.GetKeyDown(KeyCode.LeftShift) &&  SkillManager.instance.dash.canUseSKill())
        {

            dashDir = Input.GetAxisRaw("Horizontal");

            if (dashDir == 0)
                dashDir = facingDir;

            stateMachine.ChangeState(dashState);
        }
    }

    public override void die()
    {
        base.die();

        AudioManager.instance.playSFX(11, null);
        stateMachine.ChangeState(deadState);
    }

    protected override void setupZeroKnockbackPower()
    {
        knockbackPower = new Vector2(0, 0);
    }
}
