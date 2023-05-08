using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OcclusionObject : MonoBehaviour
{
    [SerializeField] private Material occlusionMaterial;
    [SerializeField] private bool isVisible = true;

    private MeshRenderer[] meshRenderes;
    private SkinnedMeshRenderer[] skinnedMeshRenderes;
    private Material[] oldSkinnedMaterials;
    private Material[] oldMaterials;


    private void Awake()
    {
        meshRenderes = GetComponentsInChildren<MeshRenderer>();
        skinnedMeshRenderes = GetComponentsInChildren<SkinnedMeshRenderer>();

        if (occlusionMaterial == null)
            occlusionMaterial = Resources.Load<Material>("Materials/OcclusionMat");

        oldMaterials = new Material[meshRenderes.Length];
        oldSkinnedMaterials = new Material[skinnedMeshRenderes.Length];

        for (int i = 0; i < meshRenderes.Length; i++)
        {
            oldMaterials[i] = meshRenderes[i].material;
        }

        for (int i = 0; i < skinnedMeshRenderes.Length; i++)
            oldSkinnedMaterials[i] = skinnedMeshRenderes[i].material;

        SetObjectVisibility(isVisible);
    }

    public void ToggleVisibility() { SetObjectVisibility(!isVisible); }

    public void SetObjectVisibility(bool isVisible)
    {
        this.isVisible = isVisible;

        int i = 0;
        foreach (MeshRenderer mr in meshRenderes)
        {
            mr.material = isVisible ? oldMaterials[i++] : occlusionMaterial;
        }

        i = 0;
        foreach (SkinnedMeshRenderer mr in skinnedMeshRenderes)
        {
            mr.material = isVisible ? oldSkinnedMaterials[i++] : occlusionMaterial;
        }
    }

    public bool Visibility => isVisible;
}
