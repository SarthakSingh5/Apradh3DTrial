using UnityEngine;

public class AiFlankSubState : AiFollowSubState
{
    public float flankAngle = 20f;
    public bool flankRight;

    public AiFlankSubState(bool flankRight)
    {
        this.flankRight = flankRight;
    }

    void MoveToFlank(AiAgent agent)
    {
        Vector3 direction = (agent.transform.position - agent.targeting.TargetPosition).normalized;
        float angle = flankAngle;
        angle *= flankRight ? 1f : -1f;
        direction = Quaternion.Euler(0f, angle, 0f) * direction;
        Vector3 destination = agent.targeting.TargetPosition + direction * (maxDistance + minDistance) * 0.5f;
        agent.navMeshAgent.isStopped = false;
        agent.navMeshAgent.SetDestination(destination);
    }

    protected override void GoToTarget(AiAgent agent)
    {
        float distance = agent.targeting.TargetDistance;
        UpdateSpeed(agent,distance);

        if (distance <= maxDistance && distance >= minDistance)
        {
            MoveToFlank(agent);
        }
        else if (distance > maxDistance)
        {
            agent.navMeshAgent.isStopped = false;
            agent.navMeshAgent.SetDestination(agent.targeting.TargetPosition);
        }
        else if (distance < minDistance)
        {
            GetAwayFrom(agent, agent.targeting.TargetPosition);
        }

        if (!agent.targeting.TargetInSight)
        {
            agent.navMeshAgent.isStopped = false;
            agent.navMeshAgent.SetDestination(agent.targeting.TargetPosition);
        }
    }


}

