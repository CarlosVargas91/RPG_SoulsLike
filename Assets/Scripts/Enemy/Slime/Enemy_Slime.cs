using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public enum SlimeType { big, medium, small}

public class Enemy_Slime : Enemy
{
    [Header("Slime specific")]
    [SerializeField] private SlimeType slymeType;
    [SerializeField] private int slimesToCreate;
    [SerializeField] private GameObject slimePrefab;
    [SerializeField] private Vector2 minCreationVelocity;
    [SerializeField] private Vector2 maxCreationVelocity;
    #region States
    public SlimeIdleState idleState { get; private set; }
    public SlimeMoveState moveState { get; private set; }
    public SlimeBattleState battleState { get; private set; }
    public SlimeAttackState attackState { get; private set; }
    public SlimeStunnedState stunnedState { get; private set; }
    public SlimeDeadState deadState { get; private set; }
    #endregion

    protected override void Awake()
    {
        base.Awake();

        setupDefaultFacingDir(-1);

        idleState = new SlimeIdleState(this, stateMachine, "Idle", this);
        moveState = new SlimeMoveState(this, stateMachine, "Move", this);
        battleState = new SlimeBattleState(this, stateMachine, "Move", this);
        attackState = new SlimeAttackState(this, stateMachine, "Attack", this);
        stunnedState = new SlimeStunnedState(this, stateMachine, "Stunned", this);
        deadState = new SlimeDeadState(this, stateMachine, "Idle", this); //Does not matter the name
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

        if (slymeType == SlimeType.small)
            return;

        createSLimes(slimesToCreate, slimePrefab);
    }

    private void createSLimes(int _amount, GameObject _slimePreFab)
    {
        for (int i = 0; i < _amount; i++)
        {
            GameObject newSlime = Instantiate(_slimePreFab, transform.position, Quaternion.identity);

            newSlime.GetComponent<Enemy_Slime>().setupSlime(facingDir);
        }
    }

    public void setupSlime(int _facingDir)
    {
        if (_facingDir != facingDir)
            Flip();

        float xVelocity = Random.Range(minCreationVelocity.x, maxCreationVelocity.x);
        float yVelocity = Random.Range(minCreationVelocity.y, maxCreationVelocity.y);

        isKnocked = true;

        GetComponent<Rigidbody2D>().velocity = new Vector2(xVelocity * -facingDir, yVelocity);

        Invoke("cancelKnockback", 1.5f);
    }

    private void cancelKnockback() => isKnocked = false;
}
