using System.Collections.Generic;
using UnityEngine;

public enum CoverHeight { Low, High }

public class CoverSpline : MonoBehaviour
{
    [Header("Cover Settings")]
    public CoverHeight heightType = CoverHeight.High;

    [Header("Spline Data (Local Space)")]
    [Tooltip("Points are local to the object so the prefab can be rotated and moved seamlessly.")]
    public List<Vector3> localSplinePoints = new List<Vector3>();

    // Quick properties for the AI to find the absolute world-space edges (The Peek Points)
    public Vector3 LeftCornerWorld => transform.TransformPoint(localSplinePoints[0]);
    public Vector3 RightCornerWorld => transform.TransformPoint(localSplinePoints[localSplinePoints.Count - 1]);

    // Converts a specific local node into a world position for the AI to path to
    public Vector3 GetWorldPoint(int index)
    {
        if (localSplinePoints.Count == 0) return transform.position;
        return transform.TransformPoint(localSplinePoints[index]);
    }

    // Calculates the perpendicular wall normal in world space (tells the AI which way to face)
    public Vector3 GetSplineNormalWorld(int index)
    {
        if (localSplinePoints.Count < 2) return transform.forward;
        if (index >= localSplinePoints.Count - 1) index = localSplinePoints.Count - 2;

        // Find the direction of the track segment, then convert it to world space
        Vector3 localTangent = (localSplinePoints[index + 1] - localSplinePoints[index]).normalized;
        Vector3 worldTangent = transform.TransformDirection(localTangent);
        
        // Cross product with 'Up' gives us the outward-facing perpendicular normal
        return Vector3.Cross(worldTangent, Vector3.up).normalized;
    }

    // Draws the invisible track in the Unity Editor so you can manually place the points
    private void OnDrawGizmos()
    {
        if (localSplinePoints == null || localSplinePoints.Count < 2) return;

        // Blue for High Cover, Yellow for Low Cover
        Gizmos.color = heightType == CoverHeight.High ? Color.blue : Color.yellow;

        for (int i = 0; i < localSplinePoints.Count; i++)
        {
            Vector3 worldPt = GetWorldPoint(i);
            
            // Draw the track node
            Gizmos.DrawSphere(worldPt, 0.1f);

            if (i < localSplinePoints.Count - 1)
            {
                Vector3 nextWorldPt = GetWorldPoint(i + 1);
                
                // Draw the rail connecting the nodes
                Gizmos.DrawLine(worldPt, nextWorldPt);

                // Draw a Red Ray indicating exactly which way the NPC's back will face
                Vector3 midPoint = Vector3.Lerp(worldPt, nextWorldPt, 0.5f);
                Gizmos.color = Color.red;
                Gizmos.DrawRay(midPoint, GetSplineNormalWorld(i) * 0.5f);
                
                // Reset color for the next rail segment
                Gizmos.color = heightType == CoverHeight.High ? Color.blue : Color.yellow;
            }
        }
    }
}