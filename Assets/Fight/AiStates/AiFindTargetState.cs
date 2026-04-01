using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiFindTargetState : AiState
{
    public AiStateId GetId() { return AiStateId.FindTarget; }

    public void Enter(Dog dog) {
        dog.npc.canMove = true; // Enable movement
        dog.npc.canTurn = true; // Enable turning
        dog.agent.speed = dog.config.walkSpeed; // Set the speed to walk speed
        dog.npc.canShoot = false;
    }

    public void Update(Dog dog)
    {
        // Wander
        dog.npc.SetDestination(dog.worldBounds.RandomPosition());
        
        if (dog.targeting.HasTarget)
        {
            dog.stateMachine.ChangeState(AiStateId.EngageTarget);
        }
    }

    public void Exit(Dog dog) { }
}
