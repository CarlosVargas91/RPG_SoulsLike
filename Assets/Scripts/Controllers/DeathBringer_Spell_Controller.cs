using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBringer_Spell_Controller : MonoBehaviour
{
    [SerializeField] private Transform check;
    [SerializeField] private Vector2 boxSize;
    [SerializeField] private LayerMask whatisPlayer;

    private CharacterStats myStats;

    public void SetupSpell(CharacterStats _stats) => myStats = _stats;
    private void AnimationTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(check.position, boxSize, whatisPlayer);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Player>() != null)
            {
                hit.GetComponent<Entity>().setupKnockBackDirection(transform);
                myStats.doDamage(hit.GetComponent<CharacterStats>());
            }
        }
    }

    private void OnDrawGizmos() => Gizmos.DrawWireCube(check.position, boxSize);

    private void SelfDestroy() => Destroy(gameObject);
}
