using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    public Renderer textureRender;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    public void DrawTexture(Texture2D texture)
    {
        textureRender.sharedMaterial.mainTexture = texture;
        textureRender.transform.localScale = new Vector3(texture.width, 1, texture.height);
    }

    public void DrawMesh(MeshData meshData, Texture2D texture)
    {
        meshFilter.sharedMesh = meshData.CreateMesh();
        meshRenderer.sharedMaterial.mainTexture = texture;

        // Mesh ��ü�� MeshCollider �߰�
        AddMeshColliderToObject(meshFilter.gameObject, meshFilter.sharedMesh);
    }
    private void AddMeshColliderToObject(GameObject obj, Mesh mesh)
    {
        MeshCollider meshCollider = obj.GetComponent<MeshCollider>();
        if (meshCollider == null)
        {
            meshCollider = obj.AddComponent<MeshCollider>();
        }

        meshCollider.sharedMesh = mesh;
    }
}
