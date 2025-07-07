using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class GPUInstancingEnable : MonoBehaviour
{
    private void Awake()
    {
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.SetPropertyBlock(mpb);
    }
}
