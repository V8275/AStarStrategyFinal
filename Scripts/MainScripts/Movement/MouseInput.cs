using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MouseInput : MonoBehaviour
{
    [SerializeField] private string DefaultLayer = "Default";
    [SerializeField] private string OutlineLayer = "Outline";
    [SerializeField] private bool affectOnlyObjectsWithMaterials = true;

    private List<GameObject> objectsToOutline = new List<GameObject>();
    private IUnit currentUnit;
    private AttackManager atkManager;

    private void Start()
    {
        CollectChildObjects(transform);
        currentUnit = GetComponent<IUnit>();
        atkManager = GetComponent<AttackManager>();
    }

    private void CollectChildObjects(Transform parent)
    {
        foreach (Transform child in parent)
        {
            if (!affectOnlyObjectsWithMaterials || child.GetComponent<Renderer>() != null)
            {
                objectsToOutline.Add(child.gameObject);
            }

            if (child.childCount > 0)
            {
                CollectChildObjects(child);
            }
        }
    }

    private void OnMouseOver()
    {
        SetLayerForAllObjects(LayerMask.NameToLayer(OutlineLayer));
        if (Input.GetMouseButtonDown(0)) SelectUnit();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1)) 
            AttackSelectedUnit(RayCastToTarget());
    }

    private void OnMouseExit()
    {
        SetLayerForAllObjects(LayerMask.NameToLayer(DefaultLayer));
    }

    private TeamUnit RayCastToTarget()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            var gameManager = FindFirstObjectByType<GameManager>();
            var otherTeams = gameManager.TeamController.GetOtherTeams();
            GameObject hoveredObject = hit.collider.gameObject;

            if(hoveredObject.CompareTag("Player"))
                for (int i = 0; i < otherTeams.Count; i++)
                {
                    var selectedUnit = hoveredObject.GetComponent<IUnit>();
                    if (otherTeams[i].units.Any(a=>a.unit == selectedUnit))
                    {
                        return otherTeams[i].units.FirstOrDefault(a => a.unit == currentUnit);
                    }
                }
            else return null;
        }
        return null;
    }

    private void SetLayerForAllObjects(int layerIndex)
    {
        foreach (var obj in objectsToOutline)
        {
            if (obj != null)
            {
                obj.layer = layerIndex;
            }
        }
    }

    private void AttackSelectedUnit(TeamUnit selectedUnit)
    {
        if (selectedUnit != null)
        {
            atkManager.Attack(selectedUnit);
        }
        else
        {
            Debug.LogWarning("Selected unit not found in current team units");
        }
    }

    private void SelectUnit()
    {
        var gameManager = FindFirstObjectByType<GameManager>();
        var curUnits = gameManager.TeamController.GetCurrentTeamUnits();

        TeamUnit selectedUnit = curUnits.FirstOrDefault(a => a.unit == currentUnit);

        if (selectedUnit != null)
        {
            gameManager.SelectUnit(selectedUnit, curUnits);
        }
        else
        {
            Debug.LogWarning("Selected unit not found in current team units");
        }
    }
}
