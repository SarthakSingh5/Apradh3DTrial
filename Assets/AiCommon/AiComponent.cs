using UnityEngine;

public class AiComponent : MonoBehaviour
{
    protected AiAgent agent;

    private void Awake()
    {
        agent = GetComponentInParent<AiAgent>();
    }
}
