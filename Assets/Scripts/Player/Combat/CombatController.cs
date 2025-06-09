using EzySlice;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class CombatController : MonoBehaviour
{
    [Header("Animations Settings")]
    [SerializeField] private Animator animator;
    [SerializeField] public Combo combo;
    [SerializeField] private VisualEffect SlashTrail;
    [SerializeField] private GameObject sword;

    private PlayerController playerController;
    private Slicing slicer;

    private float currentDamage;
    private int enemyHealthHash;

    [HideInInspector] public InputAction.CallbackContext attackContext;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        slicer = GetComponent<Slicing>();
    }

    private void Update()
    {
        if (playerController.isDead)
            return;

        if (Time.timeScale == 0)
            return;

        if (animator.GetCurrentAnimatorStateInfo(2).normalizedTime > 0.9f 
            && animator.GetCurrentAnimatorStateInfo(3).normalizedTime > 0.9f)
        {
            playerController.canMove = true;
        }
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (Time.timeScale == 0)
            return;

        attackContext = context;

        if (attackContext.performed)
        {
            playerController.canMove = false;

            if (playerController.isGrounded && !playerController.crouching && !playerController.sliding)
            {
                combo.animationLayer = 2;
            }
            else if (!playerController.climbing && !playerController.holding && !playerController.wallRunnning)
            {
                combo.animationLayer = 3;
            }
            else return;

            AttackSO currentAttack = combo.Attack();
            GetComponentInChildren<SwordDirection>().SetXonSword();
            
            if(currentAttack != null)
            {
                currentDamage = currentAttack.damage * GetCurrentAttackMultiplier();

                SlashTrail.visualEffectAsset = currentAttack.visualEffect;
                SlashTrail.Stop();

                Invoke("PlaySwiftSound", currentAttack.soundDelay);
                Invoke("TrySliceHit", currentAttack.hitDelay);
            }
        }
    }

    private float GetCurrentAttackMultiplier()
    {
        if (!playerController.isGrounded && playerController.sliding)
            return 2;

        else if (!playerController.isGrounded)
            return 1.5f;

        else if (playerController.sliding)
            return 1.5f;

        return 1;
    }

    private void TrySliceHit()
    {
        PlaySlash();

        Collider[] hits = Physics.OverlapBox(slicer.slicePlane.transform.position, new Vector3(1, 0.1f, 1),
            slicer.slicePlane.transform.rotation, slicer.whatToSlice);

        if (hits.Length <= 0)
            return;

        enemyHealthHash = 0;

        foreach (Collider hit in hits)
        {
            CheckEnemy(hit);
            CheckBullet(hit);
        }

        slicer.Slice(hits);
    }

    private void PlaySwiftSound()
    {
        var playerSoundsManager = GetComponentInChildren<PlayerSoundsManager>();

        playerSoundsManager.audioSource.clip = playerSoundsManager.swift;
        playerSoundsManager.audioSource.Play();
    }

    private static void CheckBullet(Collider hit)
    {
        var bullet = hit.GetComponent<Bullet>();

        if (bullet != null)
        {
            bullet.Reflect();
        }
    }

    private void CheckEnemy(Collider hit)
    {
        var enemyHealth = hit.
                        GetComponentInParent<EnemyHealth>();

        if (enemyHealth != null && enemyHealthHash != enemyHealth.GetInstanceID())
        {
            enemyHealth.TakeDamage(currentDamage);
            enemyHealthHash = enemyHealth.GetInstanceID();
        }
    }

    public void PlaySlash()
    {
        SlashTrail.GetComponentsInParent<Transform>()[1].position = new Vector3(
                    SlashTrail.transform.position.x,
                    sword.transform.position.y, SlashTrail.transform.position.z);
        SlashTrail.GetComponentsInParent<Transform>()[1].rotation = sword.transform.rotation;

        SlashTrail.Play();
    }
}
