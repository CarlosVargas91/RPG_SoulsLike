using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFX : EntityFX
{
    [Header("After Image")]
    [SerializeField] private GameObject afterImagePreFab;
    [SerializeField] private float colorLooseRate;
    [SerializeField] private float afterImageCoolDown;
    private float afterImageCoolDownTimer;

    [Header("Screen shake")]
    private CinemachineImpulseSource screenShake;
    [SerializeField] private float shakeMultiplier;
    public Vector3 shakeSwordImpact;
    public Vector3 shakeHighDamage;

    [Space]
    [SerializeField] private ParticleSystem dustFX;

    private void Update()
    {
        afterImageCoolDownTimer -= Time.deltaTime;
    }

    protected override void Start()
    {
        base.Start();
        screenShake = GetComponent<CinemachineImpulseSource>();
    }
    public void createAfterImage()
    {

        if (afterImageCoolDownTimer < 0)
        {
            afterImageCoolDownTimer = afterImageCoolDown;

            GameObject newAfterImage = Instantiate(afterImagePreFab, transform.position, transform.rotation);

            newAfterImage.GetComponent<AfterImageFX>().setupAfterImage(colorLooseRate, sr.sprite);
        }

    }

    public void screenShakeFunction(Vector3 _shakePower)
    {
        screenShake.m_DefaultVelocity = new Vector3(_shakePower.x * player.facingDir, _shakePower.y) * shakeMultiplier;
        screenShake.GenerateImpulse();
    }

    public void playDustFX()
    {
        if (dustFX != null)
            dustFX.Play();
    }
}
