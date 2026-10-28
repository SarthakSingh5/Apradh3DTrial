using UnityEngine;
using UnityEngine.AI;

public class AiInvestigateState : AiState
{
    private Vector3 investigateCenter;
    private int searchPointsLeft = 3; // Total spots to walk to before giving up
    private float searchRadius = 30f;  // How far from the center they will wander
    
    // FAIL-SAFE: Prevents the AI from getting stuck on geometry forever
    private float stateTimeout = 15f; 

    public AiStateId GetId() { return AiStateId.Investigate; }

    public void Enter(Dog dog)
    {
        dog.npc.canMove = true;
        dog.npc.currentAimMode = AimMode.ForceAim; // Gun UP
        
        dog.agent.speed = dog.config.walkSpeed; // walk
        dog.agent.updateRotation = true; 

        investigateCenter = dog.targeting.TargetPosition;
        searchPointsLeft = 3;
        stateTimeout = 15f; // Max time allowed in this state

        // Walk to the exact Last Known Position first
        dog.npc.SetDestination?.Invoke(investigateCenter);
    }

    public void Update(Dog dog)
    {
        stateTimeout -= Time.deltaTime;

        // 1. INSTANT OVERRIDE: Re-acquired target visual!
        if (dog.targeting.HasTarget && dog.targeting.TargetInSight)
        {
            dog.stateMachine.ChangeState(AiStateId.EngageTarget);
            return;
        }

        // 2. TIMEOUT FAIL-SAFE: If stuck on a wall for 15 seconds, give up cleanly
        if (stateTimeout <= 0f)
        {
            dog.stateMachine.ChangeState(AiStateId.FindTarget);
            return;
        }

        // 3. CONTINUOUS AIM: Keep the gun pointed ahead of movement
        Vector3 lookAheadPoint = dog.transform.position + (dog.transform.forward * 5f) + (Vector3.up * 1.5f);
        dog.npc.LookAt?.Invoke(lookAheadPoint);

        // 4. WANDER LOGIC: Check if arrived at current wander point
        // Using +0.5f threshold prevents infinite looping if the NavMesh point is slightly unreachable
        if (!dog.agent.pathPending && dog.agent.remainingDistance <= dog.agent.stoppingDistance + 0.5f)
        {
            searchPointsLeft--;

            if (searchPointsLeft > 0)
            {
                // Pick a new random point within the search radius
                Vector3 randomDir = Random.insideUnitSphere * searchRadius;
                randomDir.y = 0; 
                Vector3 nextSpot = investigateCenter + randomDir;

                // Validate point on NavMesh
                NavMeshHit hit;
                if (NavMesh.SamplePosition(nextSpot, out hit, searchRadius, NavMesh.AllAreas))
                {
                    dog.npc.SetDestination?.Invoke(hit.position);
                }
            }
            else
            {
                // Finished checking the area. Transition to normal, lowered-gun patrol.
                dog.stateMachine.ChangeState(AiStateId.FindTarget);
            }
        }
    }

    public void Exit(Dog dog)
    {
        // Lower the gun and resume normal walk speed
        dog.npc.currentAimMode = AimMode.AutoCombat;
        dog.agent.speed = dog.config.walkSpeed;
    }
}