using UnityEngine;

public class AiWeaponFireState : AiWeaponState
{
    public AiWeaponStateId GetId()
    {
        return AiWeaponStateId.Fire;
    }

    public void Enter(Dog dog)
    {
        Debug.Log("Fire");
        dog.npc.SetAim(true);
        dog.fireDirector.StartDirector(dog);
    }

    public void Update(Dog dog)
    {
        if (!dog.npc.canShoot)
        {
            dog.weaponMachine.ChangeState(AiWeaponStateId.Idle);
        }
    }

    public void Exit(Dog dog)
    {
        Debug.Log("Exit Fire");
        dog.npc.SetAim(false);
        dog.fireDirector.StopDirector();

    }
}
