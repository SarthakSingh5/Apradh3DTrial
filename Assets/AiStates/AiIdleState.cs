using UnityEngine;

public class AiIdleState : AiState
{
    public AiStateId GetId()
    {
        return AiStateId.Idle;
    }

    public void Enter(Dog dog)
    {

    }

    public void Update(Dog dog)
    {
        Vector3 playerDirection = dog.playerTransform.position - dog.transform.position;
        if (playerDirection.magnitude > dog.config.maxSightDistance)
        {
            return;
        }

        Vector3 dogDirection = dog.transform.forward;
        playerDirection.Normalize();

        float dotProduct = Vector3.Dot(playerDirection, dogDirection);
        if (dotProduct > 0.0f)
        {
            dog.stateMachine.ChangeState(AiStateId.ChasePlayer);
        }

    }

    public void Exit(Dog dog)
    {

    }


}
