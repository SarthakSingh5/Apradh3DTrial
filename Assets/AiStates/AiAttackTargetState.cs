using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiAttackTargetState : AiState
{
    public AiStateId GetId() { return AiStateId.AttackTarget; }

    public void Enter(AiAgent agent)
    {
        agent.navMeshAgent.stoppingDistance = agent.config.attackStoppingDistance;
        agent.navMeshAgent.speed = agent.config.attackSpeed;
    }

    public void Update(AiAgent agent)
    {
        if (!agent.targeting.HasTarget)
        {
            agent.anim.SetBool("isAiming", false);
            agent.aimRig.weight = 0.0f;
            agent.stateMachine.ChangeState(AiStateId.FindTarget);
            return;
        }
        agent.anim.SetBool("isAiming", true);
        agent.aimTarget.position = agent.targeting.Target.transform.position;
        agent.aimRig.weight = 1.0f;
        agent.weaponManager.aimTarget = agent.targeting.Target.transform;
        agent.navMeshAgent.destination = agent.targeting.TargetPosition;

        UpdateFiring(agent);
    }

    private void UpdateFiring(AiAgent agent)
    {
        if (agent.targeting.TargetInSight)
        {
            agent.weaponManager.SetFiring(true);
        }
        else
        {
            agent.weaponManager.SetFiring(false);
        }
    }

    public void Exit(AiAgent agent)
    {
        agent.navMeshAgent.stoppingDistance = 0.0f;
    }

    

    

    
}