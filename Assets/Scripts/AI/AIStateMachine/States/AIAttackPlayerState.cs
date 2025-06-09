using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAttackPlayerState : AIState
{
    private float reloadingTimer;

    private float timerBetweenShoots;
    private int shootCounter = 0;

    private bool canShoot = false;
    private bool isWalking = false;

    public void Enter(AIAgent agent)
    {
        agent.weaponIK.SetTargetTransform(agent.target);
        agent.weaponIK.weight = 0.8f;

        agent.navMeshAgent.stoppingDistance = agent.minDistance;
        agent.navMeshAgent.updatePosition = false;
        reloadingTimer = agent.config.reloadingTime;
    }

    public void Exit(AIAgent agent)
    {
        agent.navMeshAgent.updatePosition = true;
        agent.navMeshAgent.updateRotation = true;
    }

    public AIStateID GetID()
    {
        return AIStateID.AttackPlayer;
    }

    public void Update(AIAgent agent)
    {
        //if(!isWalking)
            agent.navMeshAgent.destination = agent.target.position;

        //TooCloseCheck(agent);
        Shooting(agent);
        Reloading(agent);
    }

    private void Shooting(AIAgent agent)
    {
        if (timerBetweenShoots >= agent.config.timeBetweenShoots && canShoot)
        {
            IsItPlayer(agent);

            shootCounter++;
            timerBetweenShoots = 0;

            if (shootCounter >= agent.config.shootsQuantity)
            {
                shootCounter = 0;
                reloadingTimer = 0;
                RandomBehaviour(agent);
                canShoot = false;
            }
        }
        else
        {
            timerBetweenShoots += Time.deltaTime;
        }
    }

    private static void IsItPlayer(AIAgent agent)
    {
        RaycastHit raycastHit;
        Physics.SphereCast(agent.weaponIK.aimTransform.position, 0.04f, agent.weaponIK.aimTransform.forward, out raycastHit);

        if (raycastHit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            agent.shootProjectTiles.Shoot();
        }
    }

    private void TooCloseCheck(AIAgent agent)
    {
        Vector3 targetDirection = agent.target.position - agent.transform.position;
        bool tooClose = targetDirection.magnitude < agent.config.minDistance;
        
        if (tooClose)
        {
            isWalking = true;

            agent.navMeshAgent.updateRotation = false;

            agent.navMeshAgent.destination = agent.target.position.x < agent.transform.position.x ?
                agent.target.position + Vector3.right * agent.config.minDistance :
                agent.target.position - Vector3.right * agent.config.minDistance;
        }
        else
        {
            agent.navMeshAgent.updateRotation = true;
            isWalking = false;
        }
    }

    private void Reloading(AIAgent agent)
    {
        if (reloadingTimer >= agent.config.reloadingTime)
        {
            canShoot = true;
            return;
        }
        else reloadingTimer += Time.deltaTime;   
    }

    private void RandomBehaviour(AIAgent agent)
    {
        isWalking = false;

        switch (UnityEngine.Random.Range(0, 3))
        {
            case 0:
                agent.stateMachine.ChangeState(AIStateID.ChasePlayer);
                break;
            case 1:
                agent.navMeshAgent.updatePosition = true;

                agent.navMeshAgent.destination = agent.transform.position + Vector3.right * UnityEngine.Random.Range(
                        -agent.config.minDistance, agent.config.minDistance);
                isWalking = true;
                
                break;
            case 2:
                agent.navMeshAgent.updatePosition = false;
                break;

        }
    }
}
