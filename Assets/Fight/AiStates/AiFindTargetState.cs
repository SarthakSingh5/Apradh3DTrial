using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiFindTargetState : AiState
{
    public AiStateId GetId() { return AiStateId.FindTarget; }

    public void Enter(Dog dog) {
        dog.npc.canMove = true; // Enable movement
        dog.agent.speed = dog.config.walkSpeed; // Set the speed to walk speed
    }

    public Vector3 GetRandomWalkablePoint(Vector3 centerPoint, float searchRadius)
    {
        // 1. Get a completely random point inside a 3D sphere
        Vector3 randomDirection = Random.insideUnitSphere * searchRadius;
        
        // 2. Move that sphere to our center point
        randomDirection += centerPoint;

        UnityEngine.AI.NavMeshHit hit;
        if (UnityEngine.AI.NavMesh.SamplePosition(randomDirection, out hit, searchRadius, UnityEngine.AI.NavMesh.AllAreas))
        {
            // Success! We found a perfectly safe, walkable spot.
            return hit.position;
        }

        // Fallback just in case something goes mathematically wrong
        return centerPoint; 
    }

    public void Update(Dog dog)
    {
        if (!dog.agent.pathPending && dog.agent.remainingDistance <= dog.agent.stoppingDistance + 0.5f)
        {
            // Search within 15 meters of where the AI currently is!
            Vector3 searchSpot = GetRandomWalkablePoint(dog.transform.position, 15f);
            dog.npc.SetDestination?.Invoke(searchSpot);
        }
        
        if (dog.targeting.HasTarget)
        {
            dog.stateMachine.ChangeState(AiStateId.EngageTarget);
        }
    }

    public void Exit(Dog dog) { }
}
