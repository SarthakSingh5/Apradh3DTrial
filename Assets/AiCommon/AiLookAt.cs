using UnityEngine;

public class AiLookAt : AiComponent
{
    private void Update()
    {
        if(agent.targeting.HasTarget && !agent.inCover)
        {
            agent.canTurn = false;   
            agent.SetAim(true);
            agent.LookAt?.Invoke(agent.targeting.TargetPosition + Vector3.up * 1.5f);
        }
    }
    
}
