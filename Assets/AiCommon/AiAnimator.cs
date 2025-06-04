using UnityEngine;

public class NpcAnimator : NpcComponent
{
    private void Update()
    {
        float forwardSpeed = Vector3.Dot(npc.velocity, npc.transform.forward);
        float rightSpeed = Vector3.Dot(npc.velocity, npc.transform.right);

        npc.anim.SetFloat("Speed", forwardSpeed);
        npc.anim.SetFloat("StrafeSpeed", rightSpeed);

        if (npc.carryingGun)
        {
            npc.anim.SetLayerWeight(1, 1f);

        }
        else
        {
            npc.anim.SetLayerWeight(1, 0f);
            dog.gun.gameObject.SetActive(false);
        }

        npc.anim.SetBool("Aim", npc.Aiming);

        


    }

}
