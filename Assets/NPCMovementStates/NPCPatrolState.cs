using UnityEngine;

public class NPCPatrolState : NPCMovementBaseState
{
    public override void EnterState(NPCMovementStateManager npcMovement)
    {
        npcMovement.npcAnim.SetBool("NPCWalking", true);
    
        SetNewPatrolPoint(npcMovement);
    }

    public override void UpdateState(NPCMovementStateManager npcMovement)
    {
        
        if (npcMovement.npcAgent.remainingDistance < 0.5f && !npcMovement.npcAgent.pathPending) 
        {
            SetNewPatrolPoint(npcMovement);
        }

        
    }

    private void SetNewPatrolPoint(NPCMovementStateManager npcMovement)
    {
        Vector3 randomPoint = npcMovement.RandomNavMeshPosition();
        npcMovement.npcAgent.SetDestination(randomPoint);
    }
}
