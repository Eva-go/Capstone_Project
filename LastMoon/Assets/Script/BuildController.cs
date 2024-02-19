using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildController : MonoBehaviour
{
    public GameObject cubePreviewPrefab; // cube_preview �������� ������ ����
    public GameObject cubePrefab; // cube �������� ������ ����
    public Material gridMaterial; // �׸����� ��Ƽ����

    private GameObject currentPreview; // ���� Ȱ��ȭ�� cube_preview ������Ʈ
    private GameObject currentGrid; // ���� Ȱ��ȭ�� grid ������Ʈ
    private bool isBuildingMode = false; // �ǹ� ��ġ ������� ���θ� ��Ÿ���� ����
    private bool isMagneticMode = false; // �ڼ� �ǹ� ��ġ ������� ���θ� ��Ÿ���� ����

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleBuildingMode();
        }

        if (isBuildingMode)
        {
            UpdateBuildingMode();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            ToggleMagneticMode();
        }

        if (isMagneticMode)
        {
            UpdateMagneticMode();
        }
    }

    void ToggleBuildingMode()
    {
        isBuildingMode = !isBuildingMode;

        if (isBuildingMode)
        {
            currentPreview = Instantiate(cubePreviewPrefab);
        }
        else
        {
            Destroy(currentPreview);
            currentPreview = null;
        }
    }

    void UpdateBuildingMode()
    {
        if (currentPreview == null)
            return;

        Vector3 mousePosition = GetMousePositionOnGround();
        currentPreview.transform.position = mousePosition;

        if (Input.GetMouseButtonDown(0))
        {
            Destroy(currentPreview);
            currentPreview = null;

            GameObject cube = Instantiate(cubePrefab, mousePosition, Quaternion.identity);
            // �߰����� ������ �ʿ��� ��� ���⿡ �ڵ� �߰�
        }
    }

    void ToggleMagneticMode()
    {
        isMagneticMode = !isMagneticMode;

        if (isMagneticMode)
        {
            CreateGrid();
        }
        else
        {
            Destroy(currentGrid);
        }
    }

    void UpdateMagneticMode()
    {
        if (currentPreview == null)
            return;

        Vector3 mousePosition = GetMousePositionOnGrid();
        currentPreview.transform.position = mousePosition;

        if (Input.GetMouseButtonDown(0))
        {
            Destroy(currentPreview);
            currentPreview = null;

            GameObject cube = Instantiate(cubePrefab, mousePosition, Quaternion.identity);
            // �߰����� ������ �ʿ��� ��� ���⿡ �ڵ� �߰�
        }
    }

    void CreateGrid()
    {
        currentGrid = new GameObject("Grid");
        currentGrid.transform.position = Vector3.zero;

        int gridSizeX = 10; // x�� �׸��� ����
        int gridSizeZ = 10; // z�� �׸��� ����
        float cellSize = 2.0f; // �� ���� ũ��

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int z = 0; z < gridSizeZ; z++)
            {
                Vector3 position = new Vector3(x * cellSize - (gridSizeX * 0.5f * cellSize), 0, z * cellSize - (gridSizeZ * 0.5f * cellSize));
                CreateGridCell(position, cellSize);
            }
        }
    }

    void CreateGridCell(Vector3 position, float cellSize)
    {
        GameObject gridCell = new GameObject("GridCell");
        gridCell.transform.position = position;

        LineRenderer lineRenderer = gridCell.AddComponent<LineRenderer>();
        lineRenderer.material = gridMaterial;
        lineRenderer.startColor = Color.gray;
        lineRenderer.endColor = Color.gray;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;

        // ���� ��
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, new Vector3(-cellSize * 0.5f, 0, 0));
        lineRenderer.SetPosition(1, new Vector3(cellSize * 0.5f, 0, 0));

        // ���� ��
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, new Vector3(0, 0, -cellSize * 0.5f));
        lineRenderer.SetPosition(1, new Vector3(0, 0, cellSize * 0.5f));
    }

    Vector3 GetMousePositionOnGround()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            return hit.point;
        }

        return Vector3.zero;
    }

    Vector3 GetMousePositionOnGrid()
    {
        return GetMousePositionOnGround();
    }
}
