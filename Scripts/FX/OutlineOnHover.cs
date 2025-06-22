using System.Collections.Generic;
using UnityEngine;

public class OutlineOnHover : MonoBehaviour
{
    [SerializeField] private string DefaultLayer = "Default";
    [SerializeField] private string OutlineLayer = "Outline";
    [SerializeField] private bool affectOnlyObjectsWithMaterials = true;

    private List<GameObject> objectsToOutline = new List<GameObject>();

    private void Start()
    {
        CollectChildObjects(transform);
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
    }

    private void OnMouseExit()
    {
        SetLayerForAllObjects(LayerMask.NameToLayer(DefaultLayer));
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
}
