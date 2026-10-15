using UnityEngine;

public class AiWeaponIdleState : AiWeaponState
{
    public AiWeaponStateId GetId()
    {
        return AiWeaponStateId.Idle;
    }

    public void Enter(Dog dog)
    {
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
    }
}
