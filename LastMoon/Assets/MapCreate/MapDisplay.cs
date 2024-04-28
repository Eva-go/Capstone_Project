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
        // textureRender�� �����Ǿ� �ִ��� Ȯ��
        if (textureRender != null)
        {
            // textureRender.sharedMaterial�� �����Ǿ� �ִ��� Ȯ��
            if (textureRender.sharedMaterial != null)
            {
                // textureRender.sharedMaterial.mainTexture�� �ؽ�ó�� �Ҵ�
                textureRender.sharedMaterial.mainTexture = texture;
            }
            else
            {
                // textureRender.sharedMaterial�� null�� �� ���� �޽����� ���
                Debug.LogError("Shared material of the texture renderer is not assigned.");
            }
        }
        else
        {
            // textureRender�� null�� �� ���� �޽����� ���
            Debug.LogError("Texture renderer is not assigned.");
        }
    }

    public void DrawMesh(MeshData meshData, Texture2D texture)
    {
        meshFilter.sharedMesh = meshData.CreateMesh();
        meshRenderer.sharedMaterial.mainTexture = texture;
    }
}
