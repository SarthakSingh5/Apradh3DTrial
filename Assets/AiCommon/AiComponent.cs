using UnityEngine;

public class AiComponent : MonoBehaviour
{
    protected Ai ai;

    private void Awake()
    {
        ai = GetComponentInParent<Ai>();
    }
}
