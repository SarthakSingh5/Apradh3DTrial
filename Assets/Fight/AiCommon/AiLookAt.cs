using UnityEngine;

public class AiLookAt : NpcComponent
{
    private void Update()
    {
        if (dog.targeting == null || dog.coverMovement == null) return;

        // The AI should aim if it sees the target AND is either in the open OR actively peeking
        bool isExposedAndReady = (!npc.inCover || dog.coverMovement.isPeeking);
        bool shouldAim = dog.targeting.HasTarget && dog.targeting.TargetInSight && isExposedAndReady;

        if (shouldAim)
        {
            npc.canTurn = false;   
            npc.SetAim(true);
            npc.LookAt?.Invoke(dog.targeting.TargetPosition + Vector3.up * 1.5f);
        }
        else
        {
            npc.canTurn = true;
            npc.SetAim(false);
        }
        npc.canShoot=npc.Aiming;
    }  
}