using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private Animator anim;
    public string Id;
    public bool activationStatus;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    [ContextMenu("Generate checkpoint ID")]
    private void generateId()
    {
        Id = System.Guid.NewGuid().ToString();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            activateCheckpoint();
        }
    }

    public void activateCheckpoint()
    {
        if (activationStatus == false)
            AudioManager.instance.playSFX(5, transform);

        activationStatus = true;
        anim.SetBool("active", true);
    }
}
