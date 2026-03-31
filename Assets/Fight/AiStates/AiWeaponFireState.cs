using UnityEngine;

public class AiWeaponFireState : AiWeaponState
{
    public AiWeaponStateId GetId()
    {
        return AiWeaponStateId.Fire;
    }

    public void Enter(Dog dog)
    {
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
        dog.fireDirector.StopDirector();

    }
}
