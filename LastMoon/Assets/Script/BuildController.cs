using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BuildController : MonoBehaviour
{
    public GameObject cubePreviewPrefab; // cube_preview �������� ������ ����
    public GameObject cubePrefab; // cube �������� ������ ����


    private GameObject currentPreview; // ���� Ȱ��ȭ�� cube_preview ������Ʈ
    private bool isBuildingMode = false; // �ǹ� ��ġ ������� ���θ� ��Ÿ���� ����

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleBuildingMode();
            gameObject.GetComponent<CubePlacer>().enabled = false;
        }

        if (isBuildingMode)
        {
            UpdateBuildingMode();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            gameObject.GetComponent<CubePlacer>().enabled = true;
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
        }
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
}
