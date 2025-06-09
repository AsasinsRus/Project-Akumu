using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public class AIChasePlayerState : AIState
{
    private float randomTimer = 0;
    private float randomTimeMax = 2;
    private float randomTimeMin = 0.5f;

    private float timer = 0;

    private bool isWalking = false;

    public void Enter(AIAgent agent)
    {
        //agent.navMeshAgent.stoppingDistance = agent.minDistance;
        agent.weaponIK.weight = 0;
    }

    public void Exit(AIAgent agent)
    {
        agent.navMeshAgent.updateRotation = true;
    }

    public AIStateID GetID()
    {
        return AIStateID.ChasePlayer;
    }

    public void Update(AIAgent agent)
    {
        if (agent.GetComponent<EnemyHealth>().currentHealth <= 0)
        {
            return;
        }

        Vector3 targetDirection = agent.target.position - agent.transform.position;
        bool tooClose = targetDirection.magnitude < agent.config.minDistance;

        /*if (tooClose && !Physics.Raycast(agent.weaponIK.aimTransform.position, agent.weaponIK.aimTransform.forward,
                        1000, 9))
        {
            agent.transform.localEulerAngles += Vector3.up * Time.deltaTime * 100;
        }*/

        timer -= Time.deltaTime;
        randomTimer -= Time.deltaTime;

        /*if(!isWalking)
            MoveAgent(agent, tooClose);*/

        RandomBehavior(agent);
    }

    private void RandomBehavior(AIAgent agent)
    {
        if (randomTimer < 0)
        {
            isWalking = false;

            switch (UnityEngine.Random.Range(0, 3))
            {
                case 0:
                    agent.stateMachine.ChangeState(AIStateID.AttackPlayer);
                    break;
                case 1:
                    agent.navMeshAgent.destination = agent.transform.position + Vector3.right * UnityEngine.Random.Range(
                        -agent.config.minDistance, agent.config.minDistance);
                    isWalking = true;

                    break;
            }

            randomTimer = UnityEngine.Random.Range(randomTimeMin, randomTimeMax);
        }
    }

    private void MoveAgent(AIAgent agent, bool tooClose)
    {
        if (timer < 0)
        {
            float sqrDistance = (agent.target.position - agent.navMeshAgent.destination).sqrMagnitude;

            if (sqrDistance > agent.config.maxDistance * agent.config.maxDistance
                || tooClose)
            {
                if (tooClose)
                {
                    agent.navMeshAgent.updateRotation = false;
                }
                else agent.navMeshAgent.updateRotation = true;

                agent.navMeshAgent.destination = agent.target.position.x < agent.transform.position.x ?
                    agent.target.position + Vector3.right * agent.config.minDistance :
                    agent.target.position - Vector3.right * agent.config.minDistance;
            }


            timer = agent.config.maxTime;
        }
    }
}
