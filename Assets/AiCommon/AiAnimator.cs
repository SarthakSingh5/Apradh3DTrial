using UnityEngine;

public class AiAnimator : AiComponent
{
    private void Update()
    {
        float forwardSpeed = Vector3.Dot(agent.velocity, agent.transform.forward);
        float rightSpeed = Vector3.Dot(agent.velocity, agent.transform.right);

        agent.anim.SetFloat("Speed", forwardSpeed);
        agent.anim.SetFloat("StrafeSpeed", rightSpeed);

        if (agent.carryingGun)
        {
            agent.anim.SetLayerWeight(1, 1f);
            agent.gun.gameObject.SetActive(true);
        }
        else
        {
            agent.anim.SetLayerWeight(1, 0f);
            agent.gun.gameObject.SetActive(false);
        }

        agent.anim.SetBool("Aim", agent.Aiming);

        


    }

}
