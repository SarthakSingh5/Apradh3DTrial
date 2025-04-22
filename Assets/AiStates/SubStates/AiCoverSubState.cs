using UnityEngine;
public class AiCoverSubState : AiEngageSubState
{
    public void Enter(AiAgent agent)
    {
        agent.navMeshAgent.updateRotation = false; // Disable automatic rotation
        agent.coverMovement.StartHiding(agent.targeting.Target.transform);
    }

    public void Update(AiAgent agent) { }

    public void Exit(AiAgent agent)
    {
        agent.coverMovement.OnCoverBreak = null;
        agent.coverMovement?.StopHiding();
    }

    
}
