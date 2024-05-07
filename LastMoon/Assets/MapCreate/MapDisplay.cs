using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    public Renderer textureRender;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    public Material[] materials; // 각 구간에 해당하는 머테리얼 배열

    public void DrawTexture(Texture2D texture)
    {
        // 노이즈 맵을 표시할 렌더러에 텍스처 적용
        textureRender.sharedMaterial.mainTexture = texture;
        textureRender.transform.localScale = new Vector3(texture.width, 1, texture.height);
    }

    public void DrawMesh(MeshData meshData, Texture2D texture)
    {
        // 메쉬 생성
        Mesh mesh = meshData.CreateMesh();
        // 메쉬 필터에 메쉬 적용
        meshFilter.sharedMesh = mesh;
        // 메쉬 렌더러에 텍스처 적용
        meshRenderer.sharedMaterial.mainTexture = texture;

        // 컬러맵에 따라 머테리얼 적용
        ApplyMaterialFromColourMap(texture);

    }

    // 컬러맵에 따라 머테리얼 적용
    private void ApplyMaterialFromColourMap(Texture2D texture)
    {
        Color[] pixels = texture.GetPixels();
        int width = texture.width;
        int height = texture.height;

        // 각 픽셀마다 해당하는 머테리얼 적용
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Color pixelColor = pixels[y * width + x];
                // 픽셀의 색상을 기준으로 해당하는 머테리얼을 찾음
                for (int i = 0; i < materials.Length; i++)
                {
                    if (pixelColor == materials[i].color)
                    {
                        meshRenderer.sharedMaterial = materials[i];
                        break;
                    }
                }
            }
        }
    }

}
