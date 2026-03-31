using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum AiWeaponStateId
{
    Idle,
    Fire

}

public interface AiWeaponState
{
    AiWeaponStateId GetId();
    void Enter(Dog dog);
    void Update(Dog dog);
    void Exit(Dog dog);
    
    

    
}
