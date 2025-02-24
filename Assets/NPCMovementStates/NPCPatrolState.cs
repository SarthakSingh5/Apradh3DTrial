using UnityEngine;

public class NPCPatrolState : NPCMovementBaseState
{
    public override void EnterState(NPCMovementStateManager npcMovement)
    {
        npcMovement.npcAgent.isStopped = false;
        npcMovement.npcAnim.SetBool("NPCWalking", true);
        npcMovement.npcAnim.SetBool("NPCAiming", false);
    
        SetNewPatrolPoint(npcMovement);
    }

    public override void UpdateState(NPCMovementStateManager npcMovement)
    {
        if (npcMovement.npcAgent.remainingDistance < 0.5f && !npcMovement.npcAgent.pathPending) 
        {
            SetNewPatrolPoint(npcMovement);
        }

        if(npcMovement.detectionManager.PlayerSeen())
        {
            npcMovement.SwitchState(npcMovement.NPCAttack);
        }
    }

    private void SetNewPatrolPoint(NPCMovementStateManager npcMovement)
    {
        Vector3 randomPoint = npcMovement.RandomNavMeshPosition();
        npcMovement.npcAgent.SetDestination(randomPoint);
    }
}
