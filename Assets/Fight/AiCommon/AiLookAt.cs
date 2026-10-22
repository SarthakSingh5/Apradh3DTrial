using UnityEngine;
public class AiLookAt : NpcComponent
{
    private void Update()
    {
        if (dog.targeting == null || dog.coverMovement == null) return;

        // 1. OVERRIDE: FORCE LOWER (Used by CoverMovement when ducking)
        if (dog.npc.currentAimMode == AimMode.ForceLower) 
        {
            npc.SetAim(false);
            npc.canShoot = false;
            npc.canTurn = true;
            return; // Stop running any other aim logic
        }

        // 2. OVERRIDE: FORCE AIM (Used by InvestigateState)
        if (dog.npc.currentAimMode == AimMode.ForceAim)
        {
            npc.SetAim(true);
            npc.canShoot = false; // Usually don't shoot while investigating
            npc.canTurn = true;
            // (Your sweep look-at logic goes here)
            return;
        }

        // 3. AUTO COMBAT MODE (Standard fighting & peeking)
        bool isExposedAndReady = (!npc.inCover || dog.coverMovement.isPeeking);
        bool shouldAim = dog.targeting.HasTarget && dog.targeting.TargetInSight && isExposedAndReady;

        npc.SetAim(shouldAim);
        npc.canShoot = shouldAim;
        npc.canTurn = !shouldAim;

        if (shouldAim)
        {
            npc.LookAt?.Invoke(dog.targeting.TargetPosition + Vector3.up * 1.5f);
        }
    }
}