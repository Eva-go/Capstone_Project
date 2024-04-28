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
        // textureRender가 설정되어 있는지 확인
        if (textureRender != null)
        {
            // textureRender.sharedMaterial이 설정되어 있는지 확인
            if (textureRender.sharedMaterial != null)
            {
                // textureRender.sharedMaterial.mainTexture에 텍스처를 할당
                textureRender.sharedMaterial.mainTexture = texture;
            }
            else
            {
                // textureRender.sharedMaterial이 null일 때 오류 메시지를 출력
                Debug.LogError("Shared material of the texture renderer is not assigned.");
            }
        }
        else
        {
            // textureRender가 null일 때 오류 메시지를 출력
            Debug.LogError("Texture renderer is not assigned.");
        }
    }

    public void DrawMesh(MeshData meshData, Texture2D texture)
    {
        meshFilter.sharedMesh = meshData.CreateMesh();
        meshRenderer.sharedMaterial.mainTexture = texture;
    }
}
