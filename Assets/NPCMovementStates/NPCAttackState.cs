using UnityEngine;

public class NPCAttackState : NPCMovementBaseState
{
    public override void EnterState(NPCMovementStateManager npcMovement)
    {
        npcMovement.npcAgent.isStopped = true;
        npcMovement.npcAnim.SetBool("NPCWalking", false);
        npcMovement.npcAnim.SetBool("NPCAiming", true);

    }

    public override void UpdateState(NPCMovementStateManager npcMovement)
    {
        npcMovement.npcWeaponManager.Fire();
        if (!npcMovement.detectionManager.IsEnemyDetected())
        {
            npcMovement.SwitchState(npcMovement.NPCPatrol);
        }
        
    }
}
