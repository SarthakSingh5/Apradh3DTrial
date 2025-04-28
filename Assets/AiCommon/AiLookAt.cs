using UnityEngine;

public class AiLookAt : AiComponent
{
    private void Update()
    {
        if(agent.targeting.HasTarget)
        {
            agent.canTurn = false;   
            agent.LookAt?.Invoke(agent.targeting.TargetPosition + Vector3.up * 1.5f);
        }
    }
    
}
