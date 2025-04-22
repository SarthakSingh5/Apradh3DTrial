using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiFindTargetState : AiState
{
    public AiStateId GetId() { return AiStateId.FindTarget; }

    public void Enter(AiAgent agent) {
        agent.navMeshAgent.isStopped = false; // Ensure the agent is not stopped
        agent.navMeshAgent.updateRotation = true; // Disable automatic rotation
        agent.navMeshAgent.speed = agent.config.walkSpeed; // Set the speed to walk speed
    }

    public void Update(AiAgent agent)
    {
        // Wander
        agent.navMeshAgent.SetDestination(agent.worldBounds.RandomPosition());
        
        if (agent.targeting.HasTarget)
        {
            agent.stateMachine.ChangeState(AiStateId.EngageTarget);
        }
    }

    public void Exit(AiAgent agent) { }
}
