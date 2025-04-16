using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Ai: MonoBehaviour
{
    public Vector3 velocity = Vector3.zero;
    

    public NavMeshAgent agent;
    public Animator animator;
    

    // Start is called before the first frame update
    private void Awake(){
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        agent.updateRotation = false; // Disable automatic rotation
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity))
        {
            transform.position = hit.point; // Snap NPC to ground
        }
        


        velocity = agent.velocity;


    }
    
}
