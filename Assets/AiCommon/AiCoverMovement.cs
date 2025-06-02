using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class AiCoverMovement : MonoBehaviour
{
    public LayerMask HidableLayers;
    public EnemyLineOfSightChecker LineOfSightChecker;
    public NavMeshAgent Agent;

    [Range(-1, 1)]
    [Tooltip("Lower is a better hiding spot")]
    public float HideSensitivity = 0;

    [Range(1, 10)]
    public float MinTargetDistance = 5f;

    [Range(0, 5f)]
    public float MinObstacleHeight = 1.25f;

    [Range(0, 5f)]
    public float Updatefrequency = 5f; // seconds
    [Range(5f, 50f)]
    public float MaxTargetDistance = 15f; // if target moves beyond this, stop hiding

    private Coroutine peekCoroutine = null;
    private Coroutine movementCoroutine = null;
    private Collider[] Colliders = new Collider[10]; // more is less performant, but more options

    [Header("Peeking Settings")]
    public float PeekOffsetDistance = 1f;
    public float TimeBetweenPeeks = 3f;
    public float PeekDuration = 1f;

    private Vector3 lastHidePosition;
    public bool isPeeking = false;



    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
    }

    public void StartHiding(AiAgent agent)
    {
        StopHiding(agent);
        movementCoroutine = StartCoroutine(Hide(agent));
        peekCoroutine = StartCoroutine(PeekAtTargetRoutine(agent));

    }


    public void StopHiding(AiAgent agent)
    {
        if (movementCoroutine != null)
        {
            StopCoroutine(movementCoroutine);
            movementCoroutine = null;
        }
        if (peekCoroutine != null)
        {
            StopCoroutine(peekCoroutine);
            peekCoroutine = null;
        }
        isPeeking = false;
        agent.inCover = false;
        
    }

    public bool HasAnyCover(Vector3 TargetPosition)
    {
        int hits = Physics.OverlapSphereNonAlloc(Agent.transform.position, LineOfSightChecker.Collider.radius, Colliders, HidableLayers);

        int validHits = 0;

        for (int i = 0; i < hits; i++)
        {
            if (Vector3.Distance(Colliders[i].transform.position, TargetPosition) >= MinTargetDistance &&
                Colliders[i].bounds.size.y >= MinObstacleHeight)
            {
                validHits++;
            }
        }

        return validHits > 0;
    }


    private IEnumerator Hide(AiAgent agent)
    {
        Debug.Log("Starting Hide Routine");
        if (peekCoroutine != null)
        {
            StopCoroutine(peekCoroutine);
            peekCoroutine = null;
        }
        WaitForSeconds Wait = new WaitForSeconds(Updatefrequency);


        while (true)
        {
            Vector3 TargetPosition = agent.targeting.TargetPosition;
            for (int i = 0; i < Colliders.Length; i++)
            {
                Colliders[i] = null;
            }

            int hits = Physics.OverlapSphereNonAlloc(Agent.transform.position, LineOfSightChecker.Collider.radius, Colliders, HidableLayers);

            int hitReduction = 0;
            for (int i = 0; i < hits; i++)
            {
                if (Vector3.Distance(Colliders[i].transform.position, TargetPosition) < MinTargetDistance || Colliders[i].bounds.size.y < MinObstacleHeight)
                {
                    Colliders[i] = null;
                    hitReduction++;
                }
            }

            hits -= hitReduction;

            System.Array.Sort(Colliders, ColliderArraySortComparer);

            for (int i = 0; i < hits; i++)
            {
                if (Colliders[i].bounds.size.y >= 3)
                {
                    List<Vector3> candidatePoints = GenerateCoverPoints(Colliders[i]);

                    foreach (var point in candidatePoints)
                    {
                        if (NavMesh.SamplePosition(point, out NavMeshHit hit, 5f, Agent.areaMask))
                        {
                            if (!NavMesh.FindClosestEdge(hit.position, out hit, Agent.areaMask))
                            {
                                Debug.LogError($"Unable to find edge close to (hit.position)");
                            }
                            // Vector3 rotated = Quaternion.Euler(0, -90, 0) * hit.normal;
                            // Vector3 a = hit.normal.normalized;
                            // Vector3 b = (TargetPosition - hit.position).normalized;

                            // float sinTheta = Vector3.Cross(a, b).magnitude;
                            // float cosTheta = Vector3.Dot(a, b);
                            // float tanTheta = sinTheta / cosTheta;
                            if (Vector3.Dot(hit.normal, (TargetPosition - hit.position).normalized) < HideSensitivity)
                            {

                                agent.inCover = false;
                                lastHidePosition = hit.position;



                                Agent.SetDestination(lastHidePosition);

                                yield return new WaitUntil(() => !Agent.pathPending && Agent.remainingDistance < 0.2f);

                                agent.SetAim(false);
                                agent.inCover = true;
                                Vector3 pos = agent.transform.position;
                                pos += hit.normal * 1000.0f;
                                pos += Vector3.up * 1.5f;
                                agent.LookAt(pos);

                                break;

                            }
                        }
                        else
                        {
                            Debug.LogError($"Unable to find NavMesh near object (Colliders[{i}].name) at {Colliders[i].transform.position}");
                        }
                    }
                }
                else
                {
                    if (NavMesh.SamplePosition(Colliders[i].transform.position, out NavMeshHit hit1, 20f, Agent.areaMask))
                    {
                        if (!NavMesh.FindClosestEdge(hit1.position, out hit1, Agent.areaMask))
                        {
                            Debug.LogError($"Unable to find edge close to {hit1.position}");
                        }

                        if (Vector3.Dot(hit1.normal, (TargetPosition - hit1.position).normalized) < HideSensitivity)
                        {
                            Agent.SetDestination(hit1.position);
                            break;
                        }
                        else
                        {
                            if (NavMesh.SamplePosition(Colliders[i].transform.position - (TargetPosition - hit1.position).normalized * 2, out NavMeshHit hit2, 20f, Agent.areaMask))
                            {
                                if (!NavMesh.FindClosestEdge(hit2.position, out hit2, Agent.areaMask))
                                {
                                    Debug.LogError($"Unable to find edge close to {hit2.position} (second attempt)");
                                }

                                if (Vector3.Dot(hit2.normal, (TargetPosition - hit2.position).normalized) < HideSensitivity)
                                {
                                    Agent.SetDestination(hit2.position);
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        Debug.LogError($"Unable to find NavMesh near object {Colliders[i].name} at {Colliders[i].transform.position}");
                    }

                }

            }
            yield return Wait;

        }
    }

    private int ColliderArraySortComparer(Collider A, Collider B)
    {
        if (A == null && B != null)
        {
            return 1;
        }
        else if (A != null && B == null)
        {
            return -1;
        }
        else if (A == null && B == null)
        {
            return 0;
        }
        else
        {
            return Vector3.Distance(Agent.transform.position, A.transform.position).CompareTo(Vector3.Distance(Agent.transform.position, B.transform.position));
        }
    }

    List<Vector3> GenerateCoverPoints(Collider collider)
    {
        List<Vector3> points = new List<Vector3>();
        Bounds bounds = collider.bounds;
        Vector3 center = bounds.center;
        Vector3 extents = bounds.extents;

        float inset = 0.2f; // distance to move inward from the actual corner

        // X and Z directions
        float x = extents.x;
        float z = extents.z;

        // 8 face-near-corner points (slightly inward from corners)
        points.Add(center + new Vector3(-x + inset, 0, z)); // front-left (inward on x)
        points.Add(center + new Vector3(-x, 0, z - inset)); // front-left (inward on z)

        points.Add(center + new Vector3(x - inset, 0, z)); // front-right
        points.Add(center + new Vector3(x, 0, z - inset)); // front-right

        points.Add(center + new Vector3(-x + inset, 0, -z)); // back-left
        points.Add(center + new Vector3(-x, 0, -z + inset)); // back-left

        points.Add(center + new Vector3(x - inset, 0, -z)); // back-right
        points.Add(center + new Vector3(x, 0, -z + inset)); // back-right

        return points;
    }


    private IEnumerator PeekAtTargetRoutine(AiAgent agent)
    {
        WaitForSeconds waitBetweenPeeks = new WaitForSeconds(TimeBetweenPeeks);
        WaitForSeconds peekDuration = new WaitForSeconds(PeekDuration);

        while (true)
        {
            Vector3 TargetPosition = agent.targeting.TargetPosition;

            // Wait until agent is in cover and not moving
            if (Agent.pathPending || Vector3.Distance(transform.position, lastHidePosition) > 0.5f)
            {
                yield return null;
                continue;
            }

            // Ensure no peeking if state is about to change
            if (!agent.inCover)
            {
                yield return null;
                continue;
            }

            Debug.Log("Starting Peek Routine");

            Vector3 directionToTarget = (TargetPosition - lastHidePosition).normalized;
            Vector3 peekPosition = lastHidePosition + directionToTarget * PeekOffsetDistance;

            if (NavMesh.SamplePosition(peekPosition, out NavMeshHit peekHit, 2f, Agent.areaMask))
            {
                Agent.SetDestination(peekHit.position);
                yield return new WaitUntil(() => !Agent.pathPending && Agent.remainingDistance <= 0.2f);
                Debug.Log($"Peeking at target from {peekHit.position}");
                isPeeking = true;

                // Ensure agent stays at peek position for the full duration
                yield return peekDuration;
                isPeeking = false;

                // Return to cover position
                Agent.SetDestination(lastHidePosition);
                yield return new WaitUntil(() => !Agent.pathPending && Agent.remainingDistance <= 0.2f);
            }
            else
            {
                Debug.LogWarning($"No valid NavMesh position for peek at {peekPosition}");
            }

            // Wait before next peek
            yield return waitBetweenPeeks;
        }
    }

}
