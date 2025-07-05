using UnityEngine;

public class AiLookAt : NpcComponent
{
    private void Update()
    {
        if(dog.targeting.HasTarget && !npc.inCover)
        {
            npc.canTurn = false;   
            npc.SetAim(true);
            npc.LookAt?.Invoke(dog.targeting.TargetPosition + Vector3.up * 1.5f);
        }
    }
    
}
