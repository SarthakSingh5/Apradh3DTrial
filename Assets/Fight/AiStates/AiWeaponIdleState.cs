using UnityEngine;

public class AiWeaponIdleState : AiWeaponState
{
    public AiWeaponStateId GetId()
    {
        return AiWeaponStateId.Idle;
    }

    public void Enter(Dog dog)
    {
        dog.npc.SetAim(false);
    }

    public void Update(Dog dog)
    {
        if(dog.npc.canShoot)
        {
            dog.weaponMachine.ChangeState(AiWeaponStateId.Fire);
        }
    }

    public void Exit(Dog dog)
    {
        // do nothing
    }
}
