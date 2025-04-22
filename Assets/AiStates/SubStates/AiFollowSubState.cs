using UnityEngine;
using UnityEngine.AI;
public class AiFollowSubState : AiEngageSubState
{
    public float maxDistance = 15f;
    public float minDistance = 4f;
    public virtual void Enter(AiAgent agent)
    {
        agent.navMeshAgent.updateRotation = false; // Disable automatic rotation
        agent.navMeshAgent.isStopped = false;
    }

    public virtual void Update(AiAgent agent)
    {
        Debug.Log("Follow Update");
        if (!agent.targeting.HasTarget)
        {
           return; 
        }
        GoToTarget(agent);
        UpdateRotation(agent, agent.targeting.TargetPosition);
    }

    public virtual void Exit(AiAgent agent)
    {
    }

    protected virtual void GoToTarget(AiAgent agent)
    {
        float distance = agent.targeting.TargetDistance;
        UpdateSpeed(agent, distance);

        if (distance <= maxDistance)
        {
            agent.navMeshAgent.isStopped = true;
        }
        else
        {
            agent.navMeshAgent.isStopped = false;
            agent.navMeshAgent.SetDestination(agent.targeting.TargetPosition);
        }

        if (distance <= minDistance)
        {
            GetAwayFrom(agent, agent.targeting.TargetPosition);
        }

        if (!agent.targeting.TargetInSight)
        {
            agent.navMeshAgent.isStopped = false;
            agent.navMeshAgent.SetDestination(agent.targeting.TargetPosition);
        }
    }

    protected void UpdateSpeed(AiAgent agent, float distance)
    {
        // setting max movement speed
        float t = Mathf.InverseLerp(maxDistance, maxDistance * 1.5f, distance);
        agent.navMeshAgent.speed = Mathf.Lerp(agent.config.walkSpeed, agent.config.runSpeed, t);
    }

    public void GetAwayFrom(AiAgent agent, Vector3 point)
    {
        Vector3 direction = (agent.transform.position - point).normalized;
        Vector3 targetPosition = direction * 5f + agent.transform.position;

        if (NavMesh.SamplePosition(targetPosition, out NavMeshHit hit, 4f, NavMesh.AllAreas))
        {
            targetPosition = hit.position;
            agent.navMeshAgent.isStopped = false;
            agent.navMeshAgent.SetDestination(targetPosition);
        }
    }

    protected void UpdateRotation(AiAgent agent, Vector3 targetPosition)
    {
        Vector3 lookDir = agent.targeting.TargetPosition - agent.transform.position;
        lookDir.y = 0f;
        if (lookDir.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDir);
            agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, targetRot, Time.deltaTime * 10f);
        }

    }



}
