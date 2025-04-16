using UnityEngine;

public class AiAnimator : AiComponent
{
    private void Update()
    {
        float forwardSpeed = Vector3.Dot(ai.velocity, ai.transform.forward);
        float rightSpeed = Vector3.Dot(ai.velocity, ai.transform.right);

        

        ai.animator.SetFloat("Speed", forwardSpeed);
        ai.animator.SetFloat("StrafeSpeed", rightSpeed);
    }

}
