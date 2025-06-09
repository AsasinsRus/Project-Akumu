using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] public float maxHealth;
    [SerializeField] public float currentHealth;

    [Header("Shield Settings")]
    [SerializeField] private float shieldCD;
    [SerializeField] private float shieldAcceleration;

    private float shieldTimer;
    private bool regenerateShield = false;
    public bool startedRegenerateTimer = false;

    public bool isDead = false;
    public bool canRechargeShieldSoundPlay = false;

    private void Start()
    {
        OnStart();
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if(currentHealth < maxHealth && !isDead)
        {
            shieldTimer += Time.deltaTime;

            if(shieldTimer > shieldCD && regenerateShield)
            {
                startedRegenerateTimer = false;
                currentHealth += Time.deltaTime * shieldAcceleration;

                if(canRechargeShieldSoundPlay)
                {
                    ShieldRechargeSoundPlay();
                    canRechargeShieldSoundPlay = false;
                }

                if(currentHealth >= maxHealth)
                {
                    currentHealth = maxHealth;
                    regenerateShield = false;
                }
            }

            OnUpdateHealth();
        }
    }

    private void ShieldRechargeSoundPlay()
    {
        var sound = FindFirstObjectByType<GeneralSoundsManager>().shieldRecharge;
        var audioSource = GetComponentInChildren<AudioSource>();

        audioSource.clip = sound;
        audioSource.Play();
    }

    public virtual void TakeDamage(float amount)
    {
        if(!isDead)
        {
            if (regenerateShield)
            {
                regenerateShield = false;

                if (currentHealth != 0.0f)
                    startedRegenerateTimer = false;
            }

            currentHealth -= amount;

            if (currentHealth <= 0.0f)
            {
                if (startedRegenerateTimer)
                {
                    Die();
                }
                else
                {
                    ShieldHitSound();
                    currentHealth = 0.0f;
                }
            }
            else
            {
                ShieldHitSound();
            }

            StartShieldTimer();
        }
    }

    private void StartShieldTimer()
    {
        startedRegenerateTimer = true;
        regenerateShield = true;
        canRechargeShieldSoundPlay = true;
        shieldTimer = 0;
    }

    protected virtual void Die()
    {
        regenerateShield = false;
        isDead = true;
    }

    public void ShieldHitSound()
    {
        var generalSoundsManager = FindFirstObjectByType<GeneralSoundsManager>();
        var audioSource = GetComponentInChildren<AudioSource>();

        audioSource.clip = generalSoundsManager.shieldHit;
        audioSource.Play();
    }

    protected virtual void OnStart()
    {

    }

    protected virtual void OnUpdateHealth()
    {

    }
}
