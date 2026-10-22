using UnityEngine;

public class AiWeaponStateMachine : MonoBehaviour
{
    public AiWeaponState[] states;
    public Dog dog;
    public AiWeaponStateId currentState;

    public AiWeaponStateMachine(Dog dog)
    {
        this.dog = dog;
        int numStates = System.Enum.GetNames(typeof(AiWeaponStateId)).Length;
        states = new AiWeaponState[numStates];
    }

    public void RegisterState(AiWeaponState state)
    {
        int index = (int)state.GetId();
        states[index] = state;
    }

    public AiWeaponState GetState(AiWeaponStateId stateId)
    {
        int index = (int)stateId;
        return states[index];
    }

    public void Update()
    {
        GetState(currentState)?.Update(dog);
    }

    public void ChangeState(AiWeaponStateId newState)
    {
        GetState(currentState)?.Exit(dog);
        currentState = newState;
        GetState(currentState)?.Enter(dog);
    }
}
