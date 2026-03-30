using UnityEngine;
using UnityEngine.AI;
public class AiFollowSubState : AiEngageSubState
{
    public float maxDistance = 15f;
    public float minDistance = 4f;
    public float followDuration = 0f;

    public virtual void Enter(Dog dog)
    {
        Debug.Log("Entering Follow SubState");
        dog.npc.canMove = true;
        followDuration = Random.Range(2f, 4f);
    }

    public virtual void Update(Dog dog)
    {
        GoToTarget(dog);
        followDuration -= Time.deltaTime;
        if (followDuration <= 0)
        {
            Debug.Log("Follow duration complete");
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

        if (!dog.targeting.TargetInSight)
        {
            dog.npc.canMove = true;
            dog.npc.SetDestination?.Invoke(dog.targeting.TargetPosition);
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



}
