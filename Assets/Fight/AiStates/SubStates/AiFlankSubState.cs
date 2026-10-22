using UnityEngine;

public class AiFlankSubState : AiFollowSubState
{
    public float flankAngle = 20f;
    public bool flankRight;
    private bool hasSetInitialDestination = false;
    public AiFlankSubState(bool flankRight)
    {
        this.flankRight = flankRight;
    }

    public override void Enter(Dog dog)
    {
        base.Enter(dog);
        hasSetInitialDestination = false; // Reset when we start flanking
    }

    public override void Update(Dog dog)
    {
        if (!hasSetInitialDestination)
        {
            GoToTarget(dog);
            hasSetInitialDestination = true;
        }
        // RULE: Flank is 'Done' as soon as we arrive at the side
        if (!dog.agent.pathPending && dog.agent.remainingDistance <= dog.agent.stoppingDistance+0.5f)
        {
            dog.npc.OnTaskComplete?.Invoke();
        }
    }

    // void MoveToFlank(Dog dog)
    // {
    //     Vector3 direction = (dog.transform.position - dog.targeting.TargetPosition).normalized;
    //     float angle = flankAngle;
    //     angle *= flankRight ? 1f : -1f;
    //     direction = Quaternion.Euler(0f, angle, 0f) * direction;
    //     Vector3 destination = dog.targeting.TargetPosition + direction * (maxDistance + minDistance) * 0.5f;
    //     dog.npc.canMove = true;
    //     dog.npc.SetDestination?.Invoke(destination);
    // }

    void MoveToFlank(Dog dog)
    {
        Vector3 direction = (dog.transform.position - dog.targeting.TargetPosition).normalized;

        float randomAngle = Random.Range(15f, 45f) * (flankRight ? 1f : -1f);
        float randomRadius = Random.Range(minDistance, maxDistance);

        direction = Quaternion.Euler(0f, randomAngle, 0f) * direction;
        Vector3 destination = dog.targeting.TargetPosition + direction * randomRadius;

        // Ensure the random point is actually walkable
        if (UnityEngine.AI.NavMesh.SamplePosition(destination, out UnityEngine.AI.NavMeshHit hit, 4f, dog.agent.areaMask))
        {
            dog.npc.canMove = true;
            dog.npc.SetDestination?.Invoke(hit.position);
        }
    }

    protected override void GoToTarget(Dog dog)
    {
        float distanceToPlayer = dog.targeting.TargetDistance;
        
        // --- DANGER ZONE SYSTEM ---
        float dangerZoneRadius = 6.0f; // 6 meters lethal range
        
        if (distanceToPlayer <= dangerZoneRadius)
        {
            // INSIDE DANGER ZONE: Strafe and suppress!
            dog.agent.speed = dog.config.walkSpeed;
            dog.agent.updateRotation = false; // Locks rotation so AiLookAt can aim at player
        }
        else
        {
            // OUTSIDE DANGER ZONE: Turn back and sprint!
            dog.agent.speed = dog.config.runSpeed;
            dog.agent.updateRotation = true; // NavMesh takes over rotation to look where it's running
        }
        // --------------------------

        dog.npc.isPanicking = distanceToPlayer <= minDistance;

        if (distanceToPlayer <= maxDistance && distanceToPlayer >= minDistance)
        {
            MoveToFlank(dog); // Uses your existing MoveToFlank logic
        }
        else if (distanceToPlayer > maxDistance)
        {
            dog.npc.canMove = true;
            dog.npc.SetDestination?.Invoke(dog.targeting.TargetPosition);
        }
        else if (distanceToPlayer < minDistance)
        {
            GetAwayFrom(dog, dog.targeting.TargetPosition);
        }
    }


}

