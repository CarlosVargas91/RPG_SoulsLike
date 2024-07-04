using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockStrikeController : MonoBehaviour
{
    [SerializeField] private CharacterStats targetStats;
    [SerializeField] private float speed;
    private int damage;

    private Animator anim;
    private bool triggered;
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }

    public void Setup(int _damage, CharacterStats _targeStats)
    {
        damage = _damage;
        targetStats = _targeStats;
    }
    // Update is called once per frame
    void Update()
    {
        if (triggered || !targetStats)
            return;

        transform.position = Vector2.MoveTowards(transform.position, targetStats.transform.position, speed * Time.deltaTime);
        transform.right = transform.position - targetStats.transform.position;

        if (Vector2.Distance(transform.position, targetStats.transform.position) < .1f)
        {
            anim.transform.localPosition = new Vector3(0, .5f);
            anim.transform.localRotation = Quaternion.identity;
            transform.localRotation = Quaternion.identity;
            transform.localScale = new Vector3(3, 3);

            Invoke("damageAnsSelfDestroy", .2f);
            triggered = true;
            anim.SetTrigger("Hit");
        }
    }

    private void damageAnsSelfDestroy()
    {
        targetStats.applyShock(true);
        targetStats.takeDamage(damage);
        Destroy(gameObject, .4f);
    }
}
