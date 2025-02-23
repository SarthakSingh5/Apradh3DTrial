using UnityEngine;
using UnityEngine.AI;

public class NPCMovementStateManager : MonoBehaviour
{
    [HideInInspector] public float npcHzInput, npcVInput;
    [HideInInspector] public Animator npcAnim;
    private NavMeshAgent npcAgent;
    [SerializeField] public Transform target;

    NPCHealth npcHealth;
    public void Start()
    {
        npcAnim = GetComponent<Animator>();
        npcAgent = GetComponent<NavMeshAgent>();
        npcHealth = GetComponent<NPCHealth>();
    }

    public void Update() 
    {
        if(npcHealth.isDead)
        {
            npcAgent.enabled = false;
            return;
        }
        MoveNPC();
        GetNPCDirection();

        npcAnim.SetFloat("NPChzInput", npcHzInput);
        npcAnim.SetFloat("NPCvInput", npcVInput);
    }

    public void MoveNPC()
    {
        if(npcAgent!=null){
            npcAgent.SetDestination(target.position);
        }
        
    }

    public void GetNPCDirection()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(npcAgent.velocity);
        npcHzInput = localVelocity.x;
        npcVInput = localVelocity.z;
    }
}
