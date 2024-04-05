using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BuildSystem : MonoBehaviour
{
    private Grid grid; // 그리드

    public GameObject cubePreviewPrefab; // cube_preview 프리팹을 연결할 변수
    public GameObject cubePrefab; // cube 프리팹을 연결할 변수
    private GameObject cubePreview; // cube 미리보기 객체를 저장할 변수
    private bool isBuildingMode = false; // 건물 설치 모드인지 여부를 나타내는 변수
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
    public void ButtonClick() //버튼 클릭 이벤트에 대한 함수를 만들어 준다.
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
            cubePreview = Instantiate(cubePreviewPrefab, position, Quaternion.identity); // 미리보기 위치에 cube 미리보기 프리팹 생성
        }
        else
        {
            cubePreview.transform.position = position; // 미리보기 위치를 업데이트
        }
    }

    private void HideCubePreview()
    {
        if (cubePreview != null)
        {
            Destroy(cubePreview); // 생성된 cube 미리보기를 제거
            cubePreview = null;
        }
    }

    private void PlaceCubeNear(Vector3 position)
    {
        Instantiate(cubePrefab, position, Quaternion.identity); // 클릭한 위치에 cube 배치
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