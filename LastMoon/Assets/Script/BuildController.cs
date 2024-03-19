using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BuildController : MonoBehaviour
{
    public GameObject cubePreviewPrefab; // cube_preview 프리팹을 연결할 변수
    public GameObject cubePrefab; // cube 프리팹을 연결할 변수


    private GameObject currentPreview; // 현재 활성화된 cube_preview 오브젝트
    private bool isBuildingMode = false; // 건물 설치 모드인지 여부를 나타내는 변수

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
