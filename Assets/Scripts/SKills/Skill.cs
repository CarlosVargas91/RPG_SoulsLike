using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    public float coolDown;
    public float coolDownTimer;

    protected Player player;

    protected virtual void Start()
    {
        player = PlayerManager.instance.player;

        //checkUnlock();
        StartCoroutine(checkUnlockAfter());
    }

    private IEnumerator checkUnlockAfter()
    {
        yield return new WaitForSeconds(.6f);
        checkUnlock();
    }
    protected virtual void Update()
    {
        coolDownTimer -= Time.deltaTime;
    }

    protected virtual void checkUnlock()
    {

    }
    public virtual bool canUseSKill()
    {
        if (coolDownTimer < 0)
        {
            //Use skill
            useSKill();
            coolDownTimer = coolDown;

            return true;
        }

        player.fx.createPopUpText("Cooldown");
        return false;
    }

    public virtual void useSKill()
    {

    }

    protected virtual Transform findClosestEnemy(Transform _checkTransform)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(_checkTransform.position, 25);

        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                float distanceToEnemy = Vector2.Distance(_checkTransform.position, hit.transform.position);

                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = hit.transform;
                }
            }
        }

        return closestEnemy;
    }
}
