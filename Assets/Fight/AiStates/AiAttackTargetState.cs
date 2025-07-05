using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiAttackTargetState : AiState
{
    public AiStateId GetId() { return AiStateId.AttackTarget; }

    public void Enter(Dog dog)
    {
        dog.agent.stoppingDistance = dog.config.attackStoppingDistance;
        dog.agent.speed = dog.config.attackSpeed;
    }

    public void Update(Dog dog)
    {
        if (!dog.targeting.HasTarget)
        {
            dog.npc.anim.SetBool("isAiming", false);
            dog.aimRig.weight = 0.0f;
            dog.stateMachine.ChangeState(AiStateId.FindTarget);
            return;
        }
        dog.npc.anim.SetBool("isAiming", true);
        dog.aimTarget.position = dog.targeting.Target.transform.position;
        dog.aimRig.weight = 1.0f;
        dog.weaponManager.aimTarget = dog.targeting.Target.transform;
        dog.agent.destination = dog.targeting.TargetPosition;

        UpdateFiring(dog);
    }

    private void UpdateFiring(Dog dog)
    {
        if (dog.targeting.TargetInSight)
        {
            dog.weaponManager.SetFiring(true);
        }
        else
        {
            dog.weaponManager.SetFiring(false);
        }
    }

    public void Exit(Dog dog)
    {
        dog.agent.stoppingDistance = 0.0f;
    }

    

    

    
}