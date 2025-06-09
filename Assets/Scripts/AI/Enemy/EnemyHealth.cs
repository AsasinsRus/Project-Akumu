using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : Health
{
    private Ragdoll ragdoll;
    private Animator colorAnimator;
    private Transform target;
    private AIAgent agent;

    [Header("Explosion force")]
    public float explosionForce;

    protected override void OnStart()
    {
        ragdoll = GetComponent<Ragdoll>();
        colorAnimator = GetComponentsInChildren<Animator>()[1];
        agent = GetComponent<AIAgent>();

        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public override void TakeDamage(float amount)
    {
        colorAnimator.Play("Hit");

        GetComponentInChildren<UIHealthBar>().UpdateHealth(currentHealth / 100);

        base.TakeDamage(amount);
    }

    protected override void Die()
    {
        base.Die();

        agent.stateMachine.ChangeState(AIStateID.Death);

        ragdoll.ActivateRigidbody();
        ragdoll?.AddForce((transform.position - target.transform.position).normalized * explosionForce, ForceMode.Impulse);

        Invoke("PlayRedissolve", 0.3f);
        Destroy(gameObject, 1.3f);

        FindFirstObjectByType<EnemySpawnManager>().currentEnemyQuantity--;
        FindFirstObjectByType<UIStatisticManager>().AddScore(100);
        FindFirstObjectByType<UIStatisticManager>().AddEnemiesLeft(-1);
    }

    private void PlayRedissolve()
    {
        var colorAnimators = GetComponentsInChildren<Animator>();

        for (int i = 0; i < colorAnimators.Length; i++)
        {
            colorAnimators[i].Play("Redissolve");
        }
    }

    protected override void OnUpdateHealth()
    {
        GetComponentInChildren<UIHealthBar>().UpdateHealth(currentHealth / 100);
    }
}
