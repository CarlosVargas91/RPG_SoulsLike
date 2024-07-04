using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_DeathBringer : Enemy
{
    public bool bossFightBegun;

    [Header("Spellcast details")]
    [SerializeField] private GameObject spellPrefab;
    public int amountOfSpells;
    public float spellCoolDown;

    public float lastTimeCast;
    [SerializeField] private float spellStateCoolDown;
    [SerializeField] private Vector2 spellOffset;

    [Header("Teleport details")]
    [SerializeField] private BoxCollider2D arena;
    [SerializeField] private Vector2 surroindingCheckSize;
    public float chanceToTeleport;
    public float defaultChanceToTeleport = 25;
    #region States
    public DeathBringer_IdleState idleState { get; private set; }
    public DeathBringer_BattleState battleState { get; private set; }
    public DeathBringer_AttackState attackState { get; private set; }
    public DeathBringer_DeadState deadState { get; private set; }
    public DeathBringer_SpellCastState spellCastState { get; private set; }
    public DeathBringer_TeleportState teleportState { get; private set; }
    #endregion

    protected override void Awake()
    {
        base.Awake();

        setupDefaultFacingDir(-1);

        idleState = new DeathBringer_IdleState(this, stateMachine, "Idle", this);

        battleState = new DeathBringer_BattleState(this, stateMachine, "Move", this);
        attackState = new DeathBringer_AttackState(this, stateMachine, "Attack", this);

        deadState = new DeathBringer_DeadState(this, stateMachine, "Idle", this);
        spellCastState = new DeathBringer_SpellCastState(this, stateMachine, "SpellCast", this);
        teleportState = new DeathBringer_TeleportState(this, stateMachine, "Teleport", this);
    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(idleState);
    }

    public override void die()
    {
        base.die();

        stateMachine.ChangeState(deadState);
    }

    public void CastSpell()
    {
        Player player = PlayerManager.instance.player;

        float xOffset = 0;

        if (player.rb.velocity.x != 0)
            xOffset = player.facingDir * spellOffset.x;

        Vector3 spellPosition = new Vector3(player.transform.position.x + xOffset, player.transform.position.y + spellOffset.y);

        GameObject newSpell = Instantiate(spellPrefab, spellPosition, Quaternion.identity);
        newSpell.GetComponent<DeathBringer_Spell_Controller>().SetupSpell(stats);
    }
    public void FindPosition()
    {
        float x = Random.Range(arena.bounds.min.x + 3, arena.bounds.max.x - 3);
        float y = Random.Range(arena.bounds.min.y + 3, arena.bounds.max.y - 3);

        transform.position = new Vector3(x, y);
        transform.position = new Vector3(transform.position.x, transform.position.y - GroundBelow().distance + (cd.size.y / 2));

        if (!GroundBelow() || SomethingIsAround())
        {
            Debug.Log("Looking for new position");
            //FindPosition(); //Creates a problem became ciclyc maybe a place with no big obstacles
        }
    }

    private RaycastHit2D GroundBelow() => Physics2D.Raycast(transform.position, Vector2.down, 100, whatIsGround);
    private bool SomethingIsAround() => Physics2D.BoxCast(transform.position, surroindingCheckSize, 0, Vector2.zero, 0, whatIsGround);
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - GroundBelow().distance));
        Gizmos.DrawWireCube(transform.position, surroindingCheckSize);
    }

    public bool CanTeleport()
    {
        if (Random.Range(0, 100) <= chanceToTeleport)
        {
            chanceToTeleport = defaultChanceToTeleport;
            return true;
        }

        return false;
    }

    public bool CanDoSpellCast()
    {
        if (Time.time >= lastTimeCast + spellStateCoolDown)
        {
            return true;
        }

        return false;
    }
}
