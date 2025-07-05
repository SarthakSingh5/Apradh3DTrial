using UnityEngine;

public class AiFlankSubState : AiFollowSubState
{
    public float flankAngle = 20f;
    public bool flankRight;

    public AiFlankSubState(bool flankRight)
    {
        this.flankRight = flankRight;
    }

    void MoveToFlank(Dog dog)
    {
        Vector3 direction = (dog.transform.position - dog.targeting.TargetPosition).normalized;
        float angle = flankAngle;
        angle *= flankRight ? 1f : -1f;
        direction = Quaternion.Euler(0f, angle, 0f) * direction;
        Vector3 destination = dog.targeting.TargetPosition + direction * (maxDistance + minDistance) * 0.5f;
        dog.npc.canMove = true;
        dog.npc.SetDestination?.Invoke(destination);
    }

    protected override void GoToTarget(Dog dog)
    {
        float distance = dog.targeting.TargetDistance;
        UpdateSpeed(dog,distance);

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

