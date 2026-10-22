using UnityEngine;
using UnityEngine.AI;
public class AiFollowSubState : AiEngageSubState
{
    public float maxDistance = 20f;
    public float minDistance = 8f;
    public float followDuration = 0f;

    protected float microStrafeTimer = 0f;
    protected float microStrafeCooldown = 3f;

    public virtual void Enter(Dog dog)
    {
        dog.npc.canMove = true;
        followDuration = Random.Range(2f, 4f);
    }
 
    public virtual void Update(Dog dog)
    {
        GoToTarget(dog);
        
        // --- NEW: Add the micromovement check! ---
        // HandleMicroStrafing(dog);
        
        followDuration -= Time.deltaTime;
        if (followDuration <= 0)
        {
            dog.npc.OnTaskComplete?.Invoke();
        }
    }

    public virtual void Exit(Dog dog)
    {
    }

    protected virtual void GoToTarget(Dog dog)
    {
        float distance = dog.targeting.TargetDistance;
        UpdateSpeed(dog, distance);

        dog.npc.isPanicking = distance <= minDistance;

        if (distance <= maxDistance)
        {
            dog.npc.canMove = false;
        }
        else
        {
            dog.npc.canMove = true;
            dog.npc.SetDestination?.Invoke(dog.targeting.TargetPosition);
        }

        if (distance <= minDistance)
        {
            GetAwayFrom(dog, dog.targeting.TargetPosition);
        }
    }

    protected void UpdateSpeed(Dog dog, float distance)
    {
        // setting max movement speed
        float t = Mathf.InverseLerp(maxDistance, maxDistance * 1.5f, distance);
        dog.agent.speed = Mathf.Lerp(dog.config.walkSpeed, dog.config.runSpeed, t);
    }

    public void GetAwayFrom(Dog dog, Vector3 point)
    {
        Vector3 direction = (dog.transform.position - point).normalized;
        Vector3 targetPosition = direction * 5f + dog.transform.position;

        if (NavMesh.SamplePosition(targetPosition, out NavMeshHit hit, 4f, NavMesh.AllAreas))
        {
            targetPosition = hit.position;
            dog.npc.canMove = true;
            dog.npc.SetDestination?.Invoke(targetPosition);
        }
    }

    protected void UpdateRotation(Dog dog, Vector3 targetPosition)
    {
        Vector3 lookDir = dog.targeting.TargetPosition - dog.transform.position;
        lookDir.y = 0f;
        if (lookDir.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDir);
            dog.transform.rotation = Quaternion.Slerp(dog.transform.rotation, targetRot, Time.deltaTime * 10f);
        }

    }


    // protected void HandleMicroStrafing(Dog dog)
    // {
    //     // Only execute if the AI has reached its destination and is basically standing still
    //     if (!dog.agent.pathPending && dog.agent.remainingDistance <= dog.agent.stoppingDistance + 0.2f)
    //     {
    //         microStrafeTimer -= Time.deltaTime;
            
    //         if (microStrafeTimer <= 0f)
    //         {
    //             // 1. Get the direction to the player
    //             Vector3 dirToPlayer = (dog.targeting.TargetPosition - dog.transform.position).normalized;
                
    //             // 2. Calculate the perpendicular "Sideways" vector (Cross product with Up)
    //             Vector3 rightVector = Vector3.Cross(Vector3.up, dirToPlayer).normalized;
                
    //             // 3. Randomly choose Left or Right, and a tiny distance (1 to 2 meters)
    //             float strafeAmount = Random.Range(1.0f, 2.0f) * (Random.value > 0.5f ? 1f : -1f);
    //             Vector3 potentialDest = dog.transform.position + rightVector * strafeAmount;

    //             // 4. Ensure the tiny step doesn't put the AI inside a wall
    //             if (NavMesh.SamplePosition(potentialDest, out NavMeshHit hit, 2f, dog.agent.areaMask))
    //             {
    //                 dog.npc.canMove = true;
    //                 // Move slightly slower than a normal walk for a highly tactical "aim walking" feel
    //                 dog.agent.speed = dog.config.walkSpeed * 0.6f; 
    //                 dog.agent.updateRotation = false; // Keep chest locked on player!
                    
    //                 dog.npc.SetDestination?.Invoke(hit.position);
                    
    //                 // Reset the timer with a random delay so it doesn't look robotic
    //                 microStrafeTimer = Random.Range(2f, 4.5f);
    //             }
    //         }
    //     }
    //     else
    //     {
    //         // If the AI is actively running somewhere else, reset the timer
    //         microStrafeTimer = 0.5f; 
    //     }
    // }



}
