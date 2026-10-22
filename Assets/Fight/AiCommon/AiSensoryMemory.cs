using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiMemory
{
    public float Age{
        get{
            return Time.time-lastSeen;
        }
    }
    public GameObject gameObject;
    public Vector3 lastKnownPosition;
    public Vector3 direction;
    public float distance;
    public float angle;
    public float lastSeen;
    public float score;
}

public class AiSensoryMemory
{
    public List<AiMemory> memories = new List<AiMemory>();
    GameObject[] characters;

    public AiSensoryMemory(int maxPlayers)
    {
        characters = new GameObject[maxPlayers];
    }

    // --- NEW: Added TeamIdentifier myTeam as a parameter ---
    public void UpdateSenses(AiSensor sensor, TeamIdentifier myTeam)
    {
        int targets = sensor.Filter(characters,"Character");
        for (int i = 0; i < targets; ++i)
        {
            GameObject target = characters[i];
            
            // --- NEW: TEAM FILTER LOGIC ---
            if (myTeam != null)
            {
                // Get the team of the target we just saw
                TeamIdentifier targetTeam = target.GetComponentInParent<TeamIdentifier>();
                
                // If they have a team, and it's the exact SAME as ours, skip them!
                if (targetTeam != null && targetTeam.team == myTeam.team)
                {
                    continue; // Abort this loop iteration. Don't add to memory!
                }
            }
            
            RefreshMemory(sensor.gameObject, target);
        }
    }

    public void RefreshMemory(GameObject dog, GameObject target)
    {
        AiMemory memory = FetchMemory(target);
        memory.gameObject = target;
        memory.lastKnownPosition = target.transform.position;
        memory.direction = target.transform.position - dog.transform.position;
        memory.distance = memory.direction.magnitude;
        memory.angle = Vector3.Angle(dog.transform.forward, memory.direction);
        memory.lastSeen = Time.time;
    }

    public AiMemory FetchMemory(GameObject gameObject)
    {
        AiMemory memory = memories.Find(x => x.gameObject == gameObject);
        if (memory == null)
        {
            memory = new AiMemory();
            memories.Add(memory);
        }
        return memory;
    }

    public void ForgetMemories(float olderThan)
    {
        memories.RemoveAll(m => m.Age > olderThan);
        memories.RemoveAll(m => !m.gameObject);
    }



}
