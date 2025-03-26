using UnityEngine;
using UnityEngine.AI;

public class NPCMovementStateManager : MonoBehaviour
{
    [HideInInspector] public float npcHzInput, npcVInput;
    [HideInInspector] public Animator npcAnim;
    public NavMeshAgent npcAgent;

    NPCHealth npcHealth;
    public DetectionStateManager detectionManager;

    public NPCWeaponManager npcWeaponManager;
    public NPCSoundEmitter npcSoundEmitter;

    public NPCMovementBaseState currentState;
    public NPCPatrolState NPCPatrol = new NPCPatrolState();
    public NPCAttackState NPCAttack = new NPCAttackState();


    public void Start()
    {
        npcAnim = GetComponent<Animator>();
        npcAgent = GetComponent<NavMeshAgent>();
        npcHealth = GetComponent<NPCHealth>();
        detectionManager = GetComponent<DetectionStateManager>();
        npcWeaponManager = GetComponentInChildren<NPCWeaponManager>();
        npcSoundEmitter = GetComponent<NPCSoundEmitter>();

        SwitchState(NPCPatrol);
    }

    public void Update() 
    {
        if(npcHealth.isDead)
        {
            npcAgent.enabled = false;
            return;
        }

        currentState.UpdateState(this);
    }


    public void SwitchState(NPCMovementBaseState npcState)
    {
        currentState = npcState;
        currentState.EnterState(this);
    }

    public Vector3 RandomNavMeshPosition()
    {
        Vector3 randomDirection = transform.position + Random.insideUnitSphere * 10f;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, 10f, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return transform.position;
    }
}
