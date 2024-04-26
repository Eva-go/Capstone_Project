using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BuildSystem : MonoBehaviour
{
    private Grid grid; // �׸���

    public GameObject cubePreviewPrefab; // cube_preview �������� ������ ����
    public GameObject cubePrefab; // cube �������� ������ ����
    private GameObject cubePreview; // cube �̸����� ��ü�� ������ ����
    private bool isBuildingMode = false; // �ǹ� ��ġ ������� ���θ� ��Ÿ���� ����
    private bool checkToClick = false;
    private enum BuildMode { FREE, GRID, EXIT };
    private BuildMode build;

    int gridnum = 0;
    int freenum = 0;

    public Button bt;


    private void Awake()
    {
        grid = FindObjectOfType<Grid>();
       
        build = BuildMode.EXIT;

    }
    void Start()
    {
        bt = GetComponent<Button>();
        bt.onClick.AddListener(ButtonClick);
    }

    void Update()
    {
        if(checkToClick)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                switch (build)
                {
                    case BuildMode.FREE:
                        ToggleBuildingMode();
                        break;
                    case BuildMode.GRID:
                        HideCubePreview();
                        break;
                    case BuildMode.EXIT:

                        //ToggleBuildingMode();
                        build = BuildMode.FREE;

                        break;
                    default:
                        break;
                }
            }
            
        }
        if (build == BuildMode.FREE)
        {
            UpdateBuildingMode();
        }
        if (build == BuildMode.GRID)
        {
            GridBuild();
        }

    }
    public void ButtonClick() //��ư Ŭ�� �̺�Ʈ�� ���� �Լ��� ����� �ش�.
    {
        checkToClick=true;
        build = BuildMode.FREE;
        ToggleBuildingMode();

    }

    // Grid mode build
    private void GridBuild()
    {
        if (build == BuildMode.GRID && Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hitInfo))
            {
                Vector3 clickPoint = grid.GetNearestPointOnGrid(hitInfo.point);
                PlaceCubeNear(clickPoint);
            }
            checkToClick = false;
            build = BuildMode.EXIT;
        }
        else if (build == BuildMode.GRID)
        {
            RaycastHit hitInfo;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hitInfo))
            {
                Vector3 previewPosition = grid.GetNearestPointOnGrid(hitInfo.point);
                ShowCubePreview(previewPosition);
            }
        }

    }

    private void ShowCubePreview(Vector3 position)
    {
        if (cubePreview == null)
        {
            cubePreview = Instantiate(cubePreviewPrefab, position, Quaternion.identity); // �̸����� ��ġ�� cube �̸����� ������ ����
        }
        else
        {
            cubePreview.transform.position = position; // �̸����� ��ġ�� ������Ʈ
        }
    }

    private void HideCubePreview()
    {
        if (cubePreview != null)
        {
            Destroy(cubePreview); // ������ cube �̸����⸦ ����
            cubePreview = null;
        }
    }

    private void PlaceCubeNear(Vector3 position)
    {
        Instantiate(cubePrefab, position, Quaternion.identity); // Ŭ���� ��ġ�� cube ��ġ
    }

    void ToggleBuildingMode()
    {
        
        isBuildingMode = !isBuildingMode;

        if (isBuildingMode)
        {
            if (cubePreview == null)
            {
                cubePreview = Instantiate(cubePreviewPrefab);
            }
        }
        else
        {
            Destroy(cubePreview);
            cubePreview = null;
        }

    }

    void UpdateBuildingMode()
    {
        if (isBuildingMode && cubePreview != null)
        {
            Vector3 mousePosition = GetMousePositionOnGround();
            cubePreview.transform.position = mousePosition;

            if (Input.GetMouseButtonDown(0))
            {
                Destroy(cubePreview);
                cubePreview = null;

                GameObject cube = Instantiate(cubePrefab, mousePosition, Quaternion.identity);
                checkToClick = false;
                build = BuildMode.GRID;
            }
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