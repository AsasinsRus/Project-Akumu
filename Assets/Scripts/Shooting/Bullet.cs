using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Composites;
using UnityEngine.UIElements;

public class Bullet : MonoBehaviour
{
    private Vector3 shootDirection;

    public float moveSpeed;
    public float damageAmount = 40;
    public Transform hitEffect;

    public bool canBeDestroyed = true;

    private float canBeDestroyedTime = 0.0f;
    private float canBeDestroyedTimer = 0.0f;

    private float reflectTime = 0.05f;
    private float reflectTimer = 0.0f;

    private PlayerHealth playerHealth;
    private CombatController combatController;
    private bool playerWasHit = false;
    private bool wasReflected = false;

    private AudioSource audioSource;

    public void Setup(Vector3 shootDirection)
    {
        canBeDestroyed = true;
        this.shootDirection = shootDirection;
    }

    private void Update()
    {
        if (canBeDestroyedTimer < canBeDestroyedTime)
            canBeDestroyedTimer += Time.deltaTime;
        ReflectCheck();

        transform.position += shootDirection * moveSpeed * Time.deltaTime;
    }

    private void ReflectCheck()
    {
        if (reflectTimer < reflectTime)
            reflectTimer += Time.deltaTime;
        else if (playerHealth != null && playerWasHit)
        {
            playerHealth.TakeDamage(damageAmount);
            DestroyBullet();

            canBeDestroyed = true;
            playerWasHit = false;
        }

        if (combatController != null && !combatController.combo.IsAttackInCD() && combatController.attackContext.performed)
        {
            Reflect();

            canBeDestroyedTimer = 0;

            playerWasHit = false;
            canBeDestroyed = true;
            wasReflected = true;
        }
    }

    public void Reflect()
    {
        if(!wasReflected)
        {
            ReflectSound();
            shootDirection = -shootDirection;

            wasReflected = true;
            canBeDestroyedTimer = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (canBeDestroyedTimer < canBeDestroyedTime)
            return;

        if (other.GetComponent<NavMeshAgent>() != null)
            return;

        audioSource = other.GetComponentInParent<AudioSource>();
        EnemyHealth enemyHealth = other.GetComponentInParent<EnemyHealth>();

        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damageAmount);
        }

        playerHealth = other.GetComponentInParent<PlayerHealth>();

        if(playerHealth != null)
        {
            combatController = other.GetComponentInParent<CombatController>();

            if(!wasReflected)
            {
                reflectTimer = 0;
                canBeDestroyed = false;
                playerWasHit = true;
            }
            else playerHealth.TakeDamage(damageAmount);
        }

        if(canBeDestroyed)
        {
            DestroyBullet();
        }
    }

    private void DestroyBullet()
    {
        var hit = Instantiate(hitEffect, transform.position, hitEffect.rotation);
        
        hit.GetComponent<ParticleSystem>().Play();

        Destroy(hit.gameObject, 2);
        Destroy(gameObject);
    }

    public void ReflectSound()
    {
        var playerSoundsManager = FindFirstObjectByType<PlayerSoundsManager>();

        playerSoundsManager.audioSource.clip = playerSoundsManager.deflect;
        playerSoundsManager.audioSource.Play();
    }
}
