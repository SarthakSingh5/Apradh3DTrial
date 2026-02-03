using UnityEngine;
using UnityEngine.AI;

public class CampaignController : MonoBehaviour
{
    public GameObject selectedUnit;
    public GameObject selectedBuilding;
    public LayerMask clickableLayers;

    
    [Header("Grid Settings")]
    public float gridSize = 1.0f; // Set this to match your terrain grid size

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) HandleSelection();
        if (Input.GetMouseButtonDown(1) && selectedUnit != null) HandleCommand();
    }

    void HandleSelection()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, clickableLayers))
        {
            GameObject hitObject = hit.collider.gameObject;
            int hitLayer = hitObject.layer;

            if (hitLayer == LayerMask.NameToLayer("Units"))
            {
                SelectUnit(hitObject);
            }
            else if (hitLayer == LayerMask.NameToLayer("Buildings"))
            {
                SelectBuilding(hitObject);
            }
            else
            {
                DeselectAll();
            }
        }
    }

    void HandleCommand()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, clickableLayers))
        {
            GameObject hitObject = hit.collider.gameObject;
            int hitLayer = hitObject.layer;

            if (hitLayer == LayerMask.NameToLayer("Units"))
            {
                InteractUnit(hitObject);
            }
            else if( hitLayer == LayerMask.NameToLayer("Buildings"))
            {
                InteractBuilding(hitObject);
            }
            else if(hitLayer == LayerMask.NameToLayer("Terrain"))
            {
                // APPLY GRID SNAPPING HERE
                Vector3 snappedPos = SnapToGrid(hit.point);
                MoveUnit(snappedPos);
            }
            
        }
    }

    // This ensures the general moves to the center of a grid tile
    Vector3 SnapToGrid(Vector3 position)
    {
        float x = Mathf.Round(position.x / gridSize) * gridSize;
        float z = Mathf.Round(position.z / gridSize) * gridSize;
        return new Vector3(x, position.y, z);
    }

    void SelectUnit(GameObject unit)
    {
        DeselectAll();
        selectedUnit = unit;
        
        // // VISUAL FEEDBACK: Toggle a child object (like a ring/circle) on the unit
        // Transform selectionCircle = selectedUnit.transform.Find("SelectionCircle");
        // if (selectionCircle != null) selectionCircle.gameObject.SetActive(true);
        
        Debug.Log("== Selected Unit: " + unit.name + " ==");
    }

    void SelectBuilding(GameObject building)
    {
        DeselectAll();
        selectedBuilding = building;
        
        Debug.Log("== Selected Building: " + building.name + " ==");
    }

    void MoveUnit(Vector3 destination)
    {
        NavMeshAgent agent = selectedUnit.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.SetDestination(destination);
        }
    }

    void InteractUnit(GameObject targetUnit)
    {
        // if (targetUnit != selectedUnit)
        // {
        //     AttackTarget(targetUnit);
        // }
        Debug.Log("Interacted with unit: " + targetUnit.name);
    }

    void InteractBuilding(GameObject targetBuilding)
    {
        // Building interaction logic (e.g., enter, attack, etc.)
        Debug.Log("Interacted with building: " + targetBuilding.name);
    }

    void DeselectAll()
    {
        // if (selectedUnit != null)
        // {
        //     // Hide the selection circle before clearing the reference
        //     Transform selectionCircle = selectedUnit.transform.Find("SelectionCircle");
        //     if (selectionCircle != null) selectionCircle.gameObject.SetActive(false);
        // }
        selectedUnit = null;
        selectedBuilding = null;
    }

    void AttackTarget(GameObject target) { /* Attack Logic */ }
}