using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;

public class AiController : NpcController
{
    [SerializeField]
    public NavMeshAgent agent;


    public override void SetNpc(Npc npc)
    {
        if (this.npc != null)
        {
            this.npc.SetDestination -= OnSetDestination;
            this.npc.SetMaxSpeed -= OnSetMaxSpeed;
        }

        base.SetNpc(npc);

        if (npc != null)
        {
            agent = GetComponentInParent<NavMeshAgent>();
            agent.speed = npc.WalkSpeed;

            npc.SetDestination += OnSetDestination;
            npc.SetMaxSpeed += OnSetMaxSpeed;

            if (agent == null)
            {
                Debug.LogError($"NavMeshAgent not found on Npc '{npc.gameObject.name}'.");
            }
        }
    }

    void OnSetDestination(Vector3 destination)
    {
        agent.SetDestination(destination);
    }

    void OnSetMaxSpeed(float maxSpeed)
    {
        agent.speed = maxSpeed;
    }


    protected virtual void Update()
    {
        if (agent == null)
        {
            return;
        }

        npc.velocity = agent.velocity;
        agent.isStopped = !npc.canMove;
        agent.updateRotation = npc.canTurn;
    }

}
