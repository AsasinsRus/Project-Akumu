using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIIdleState : AIState
{
    public void Enter(AIAgent agent)
    {
        
    }

    public void Exit(AIAgent agent)
    {
        
    }

    public AIStateID GetID()
    {
        return AIStateID.Idle;
    }

    public void Update(AIAgent agent)
    {
        Vector3 targetDirection = agent.target.position - agent.transform.position;

        if(targetDirection.magnitude < agent.config.distanceToFind)
        {
            agent.stateMachine.ChangeState(AIStateID.AttackPlayer);
        }
    }
}
