using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    public Renderer textureRender;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    public Material[] materials; // �� ������ �ش��ϴ� ���׸��� �迭

    public void DrawTexture(Texture2D texture)
    {
        // ������ ���� ǥ���� �������� �ؽ�ó ����
        textureRender.sharedMaterial.mainTexture = texture;
        textureRender.transform.localScale = new Vector3(texture.width, 1, texture.height);
    }

    public void DrawMesh(MeshData meshData, Texture2D texture)
    {
        // �޽� ����
        Mesh mesh = meshData.CreateMesh();
        // �޽� ���Ϳ� �޽� ����
        meshFilter.sharedMesh = mesh;
        // �޽� �������� �ؽ�ó ����
        meshRenderer.sharedMaterial.mainTexture = texture;

        // �÷��ʿ� ���� ���׸��� ����
        ApplyMaterialFromColourMap(texture);

    }

    // �÷��ʿ� ���� ���׸��� ����
    private void ApplyMaterialFromColourMap(Texture2D texture)
    {
        Color[] pixels = texture.GetPixels();
        int width = texture.width;
        int height = texture.height;

        // �� �ȼ����� �ش��ϴ� ���׸��� ����
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Color pixelColor = pixels[y * width + x];
                // �ȼ��� ������ �������� �ش��ϴ� ���׸����� ã��
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
