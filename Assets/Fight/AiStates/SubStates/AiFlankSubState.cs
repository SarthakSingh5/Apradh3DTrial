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

        dog.npc.canMove = true;
        dog.npc.SetDestination?.Invoke(destination);
    }

    protected override void GoToTarget(Dog dog)
    {
        float distance = dog.targeting.TargetDistance;
        UpdateSpeed(dog, distance);

        dog.npc.isPanicking = distance <= minDistance;

        if (distance <= maxDistance && distance >= minDistance)
        {
            MoveToFlank(dog);
        }
        else if (distance > maxDistance)
        {
            dog.npc.canMove = true;
            dog.npc.SetDestination?.Invoke(dog.targeting.TargetPosition);
        }
        else if (distance < minDistance)
        {
            GetAwayFrom(dog, dog.targeting.TargetPosition);
        }

        if (!dog.targeting.TargetInSight)
        {
            dog.npc.canMove = true;
            dog.npc.SetDestination?.Invoke(dog.targeting.TargetPosition);
        }
    }


}

