using UnityEngine;
using UnityEditor;
using UnityEngine.AI;
using System.Collections.Generic;

public class NavMeshCoverBaker : EditorWindow
{
    [MenuItem("Tools/Generate Building Cover from NavMesh")]
    public static void GenerateCoverSplines()
    {
        Debug.Log("Starting NavMesh Cover Baking...");

        NavMeshTriangulation navData = NavMesh.CalculateTriangulation();
        Vector3[] rawVertices = navData.vertices;
        int[] rawIndices = navData.indices;

        Dictionary<Vector3, int> weldedVertices = new Dictionary<Vector3, int>();
        List<Vector3> finalVertices = new List<Vector3>();
        int[] finalIndices = new int[rawIndices.Length];

        for (int i = 0; i < rawIndices.Length; i++)
        {
            Vector3 rawV = rawVertices[rawIndices[i]];
            // Projecting firmly to the floor to prevent hovering
            Vector3 roundedV = new Vector3(
                Mathf.Round(rawV.x * 100f) / 100f,
                Mathf.Round(rawV.y * 100f) / 100f,
                Mathf.Round(rawV.z * 100f) / 100f
            );

            if (!weldedVertices.ContainsKey(roundedV))
            {
                weldedVertices[roundedV] = finalVertices.Count;
                finalVertices.Add(roundedV);
            }
            finalIndices[i] = weldedVertices[roundedV];
        }

        Dictionary<Edge, int> edgeCounts = new Dictionary<Edge, int>();
        Dictionary<Edge, Vector2Int> originalDirections = new Dictionary<Edge, Vector2Int>();

        for (int i = 0; i < finalIndices.Length; i += 3)
        {
            AddEdge(edgeCounts, originalDirections, finalIndices[i], finalIndices[i + 1]);
            AddEdge(edgeCounts, originalDirections, finalIndices[i + 1], finalIndices[i + 2]);
            AddEdge(edgeCounts, originalDirections, finalIndices[i + 2], finalIndices[i]);
        }

        // --- NEW: EXTRACT BOUNDARY EDGES ---
        List<Vector2Int> boundarySegments = new List<Vector2Int>();
        foreach (KeyValuePair<Edge, int> kvp in edgeCounts)
        {
            if (kvp.Value == 1)
            {
                boundarySegments.Add(originalDirections[kvp.Key]);
            }
        }

        // --- NEW: COLLINEAR EDGE MERGING ---
        // This stitches broken NavMesh triangle edges together into long, continuous walls
        bool merged = true;
        while (merged)
        {
            merged = false;
            for (int i = 0; i < boundarySegments.Count; i++)
            {
                for (int j = 0; j < boundarySegments.Count; j++)
                {
                    if (i == j) continue;

                    Vector2Int segA = boundarySegments[i];
                    Vector2Int segB = boundarySegments[j];

                    // If segA ends exactly where segB begins
                    if (segA.y == segB.x)
                    {
                        Vector3 dirA = (finalVertices[segA.y] - finalVertices[segA.x]).normalized;
                        Vector3 dirB = (finalVertices[segB.y] - finalVertices[segB.x]).normalized;

                        // Check if they form a straight line (Dot product > 0.95 means nearly parallel)
                        if (Vector3.Dot(dirA, dirB) > 0.95f)
                        {
                            // Merge into a single long segment
                            boundarySegments[i] = new Vector2Int(segA.x, segB.y);
                            boundarySegments.RemoveAt(j);
                            merged = true;
                            break; // Break to restart the loop safely since collection was modified
                        }
                    }
                }
                if (merged) break;
            }
        }

        GameObject oldParent = GameObject.Find("NavMesh_AutoCover_System");
        if (oldParent != null) DestroyImmediate(oldParent);

        GameObject coverParent = new GameObject("NavMesh_AutoCover_System");
        int splineCount = 0;

        foreach (Vector2Int segment in boundarySegments)
        {
            Vector3 originalPt1 = finalVertices[segment.x];
            Vector3 originalPt2 = finalVertices[segment.y];

            float rawDistance = Vector3.Distance(originalPt1, originalPt2);

            // Ignore tiny leftover segments (like doorways)
            if (rawDistance < 1.5f) continue;

            float insetAmount = 0.4f;
            Vector3 edgeDirection = (originalPt2 - originalPt1).normalized;
            Vector3 outwardNormal = -Vector3.Cross(edgeDirection, Vector3.up).normalized;

            Vector3 pt1 = originalPt1 + (edgeDirection * insetAmount);
            Vector3 pt2 = originalPt2 - (edgeDirection * insetAmount);

            GameObject splineObj = new GameObject($"Auto_CoverSpline_{splineCount}");
            splineObj.transform.SetParent(coverParent.transform);
            splineObj.transform.position = (pt1 + pt2) / 2f;

            if (outwardNormal != Vector3.zero)
            {
                splineObj.transform.rotation = Quaternion.LookRotation(outwardNormal, Vector3.up);
            }

            CoverSpline coverSpline = splineObj.AddComponent<CoverSpline>();
            coverSpline.heightType = CoverHeight.High;

            int coverLayerIndex = LayerMask.NameToLayer("Cover");
            if (coverLayerIndex != -1) splineObj.layer = coverLayerIndex;

            BoxCollider col = splineObj.AddComponent<BoxCollider>();
            col.isTrigger = true;

            float finalLength = Vector3.Distance(pt1, pt2);
            col.size = new Vector3(finalLength, 1.5f, 0.5f);

            coverSpline.localSplinePoints.Add(splineObj.transform.InverseTransformPoint(pt1));
            coverSpline.localSplinePoints.Add(splineObj.transform.InverseTransformPoint(pt2));

            splineCount++;
        }

        Debug.Log($"== Successfully generated {splineCount} optimized, stitched cover splines! ==");
    }

    private struct Edge
    {
        public int v1;
        public int v2;

        public Edge(int a, int b)
        {
            v1 = Mathf.Min(a, b);
            v2 = Mathf.Max(a, b);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Edge)) return false;
            Edge e = (Edge)obj;
            return v1 == e.v1 && v2 == e.v2;
        }

        public override int GetHashCode()
        {
            return v1.GetHashCode() ^ v2.GetHashCode();
        }
    }

    private static void AddEdge(Dictionary<Edge, int> edgeCounts, Dictionary<Edge, Vector2Int> originalDirections, int v1, int v2)
    {
        Edge edge = new Edge(v1, v2);
        if (edgeCounts.ContainsKey(edge))
        {
            edgeCounts[edge]++;
        }
        else
        {
            edgeCounts[edge] = 1;
            originalDirections[edge] = new Vector2Int(v1, v2);
        }
    }
}  