using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    public static CanvasController Instance;

    public GameObject inside;
    public GameObject inventory;
    public GameObject money;
    public GameObject Tab;

    private int keyTabCode = 2;
    private bool inventory_ck;
    private int TotalSell = 0;
    private int Amount = 0;
    // 노드
    public GameObject[] nodes;
    public Text[] nodesCount;
    public Text[] mixCount;
    public Text[] nodePrice;
    public Text[] mixPrice;
    


    public int[] count = new int[6]; // 배열 크기 초기화
    public int[] SellCount = new int[6]; // 배열 크기 초기화
    public int[] salePrice = new int[6]; // 배열 크기 초기화
    public int[] nodePriceCount = new int[6];
    public int[] mixPriceCount = new int[6];
    private PlayerController playerController;

    private bool localplayerck = false;

    public Sprite[] UI_ToolIconSprites;
    public Image[] UI_ToolIcons;
    public Color[] UI_ToolDurabilityColor;
    public Image[] UI_ToolDurabilities;

    private int selectedToolIndex = 0;


    //아이템 획득 여부
    private bool isItme = false;

    private Transform inventoryTransform;

    void Awake()
    {
        isItme = false;
        localplayerck = false;
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }

    void Start()
    {
        ToolIconUpdate();
        ToolColorUpdate();

        inside.SetActive(false);
        inventory.SetActive(false);
        Tab.SetActive(false);
        money.SetActive(true);

        inventoryTransform = inventory.transform;

        for (int i = 0; i < 6; i++)
        {
            nodesCount[i].text = "0";
            mixCount[i].text = "0";
            count[i] = 0;
            SellCount[i] = 0;
            nodePriceCount[i] +=10+ i * 10;
            mixPriceCount[i] += 100 + i * 10;
            nodePrice[i].text = nodePriceCount[i].ToString();
            mixPrice[i].text = mixPriceCount[i].ToString();
        }

        for(int i=0;i<6;i++)
        {
            GameValue.getPrice(i, nodePriceCount[i], mixPriceCount[i]);
        }
        
    }

    void Update()
    {
        ToolIconSwitching();

        UpdateInsideActive();
        UpdateInventoryActive();
        UpdateInventoryTabActive();
        UpdateMoneyActive();
        // 노드 관련 함수
        Sell();
        Die();
        //아이템 업데이트
        if (isItme)
        {
            nodeCountUpdate();
            mixCountUpdate();
        }
    }
    private void ToolIconSwitching()
    {
        int previousSelectedWeaponIndex = selectedToolIndex;

        // 무기 번호 키로 무기 교체
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedToolIndex = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedToolIndex = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            selectedToolIndex = 2;
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f)
        {
            selectedToolIndex = (selectedToolIndex + 1) % 3;
        }
        else if (scroll < 0f)
        {
            selectedToolIndex = (selectedToolIndex + 2) % 3;
        }

        // 무기 교체가 필요하면 새로운 무기를 장착
        if (previousSelectedWeaponIndex != selectedToolIndex)
        {
            UI_ToolIcons[0].sprite = UI_ToolIcons[selectedToolIndex + 1].sprite;
            ToolColorUpdate();
        }
    }

    public void ToolIconUpdate()
    {
        UI_ToolIcons[1].sprite = UI_ToolIconSprites[GameValue.Axe * 3];
        UI_ToolIcons[2].sprite = UI_ToolIconSprites[GameValue.Pickaxe * 3 + 1];
        UI_ToolIcons[3].sprite = UI_ToolIconSprites[GameValue.Shovel * 3 + 2];

        UI_ToolIcons[0].sprite = UI_ToolIcons[selectedToolIndex + 1].sprite;
    }

    public void ToolColorUpdate()
    {
        UI_ToolDurabilities[1].color = UI_ToolDurabilityColor[GameValue.Axe + 1];
        UI_ToolDurabilities[2].color = UI_ToolDurabilityColor[GameValue.Pickaxe + 1];
        UI_ToolDurabilities[3].color = UI_ToolDurabilityColor[GameValue.Shovel + 1];

        UI_ToolDurabilities[0].color = UI_ToolDurabilities[selectedToolIndex + 1].color;
        UI_ToolDurabilities[selectedToolIndex + 1].color = UI_ToolDurabilityColor[0];
    }


    public void RegisterPlayerController(PlayerController player)
    {
        if (!localplayerck)
        {
            playerController = player;
            Debug.Log("플레이어 연동 " + playerController);
            if (playerController != null)
            {
                playerController.OnInventoryChanged += nodeCountUpdate;
                nodeCountUpdate(); // 플레이어 등록 후 초기 인벤토리 업데이트
                isItme = true;
            }
            localplayerck = true;
        }

    }

    public void nodeCountUpdate()
    {
        if (playerController != null)
        {
            for (int i = 0; i < 6; i++)
            {
                Debug.Log("아이템 UI " + playerController.nodeItiems[i] + "플레이어 이름 " + playerController.name);
                count[i] = playerController.nodeItiems[i];
                nodesCount[i].text = count[i].ToString();
            }
        }
    }

    public void mixCountUpdate()
    {
        for (int i = 0; i < 6; i++)
        {
            Debug.Log("아이템 UI" + playerController.mixItiems[i]);
            count[i] = playerController.mixItiems[i];
            mixCount[i].text = count[i].ToString();
        }
    }

    private void Sell()
    {
        if (GameValue.lived)
        {
            money.SetActive(true);
            GameValue.setMoney();
            for (int i = 0; i < nodesCount.Length; i++)
            {
                Amount = SellCount[i] * salePrice[i];
                TotalSell += Amount;
            }
            GameValue.GetMomey(TotalSell);
            GameValue.lived = false;

            for (int i = 0; i < nodesCount.Length; i++)
            {
                nodesCount[i].text = "0";
                SellCount[i] = 0;
                count[i] = 0;
            }
        }
    }

    private void Die()
    {
        if (PlayerController.Hp == 0)
        {
            for (int i = 0; i < nodesCount.Length; i++)
            {
                nodesCount[i].text = "0";
                mixCount[i].text = "0";
            }
        }
    }

    void UpdateMoneyActive()
    {
        if (GameValue.lived)
        {
            money.SetActive(true);
            inventory.SetActive(false);
            Tab.SetActive(false);
            GameValue.inside = true;
        }
    }

    void UpdateInsideActive()
    {
        if (GameValue.insideUser < GameValue.MaxUser)
        {
            inside.SetActive(PlayerController.insideActive && !PlayerController.PreViewCam);

        }
    }


    void UpdateInventoryActive()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventory_ck = !inventory_ck;
            inventory.SetActive(inventory_ck);
            Tab.SetActive(inventory_ck);
        }
    }

    void UpdateInventoryTabActive()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (inventory_ck)
            {
                keyTabCode = (keyTabCode + 1) % 3;
                bool isCraftManualActive = CraftMaunal.isActivated && !CraftMaunal.isPreViewActivated;

                SetInventoryTabActive(keyTabCode);

                SetTabActive(keyTabCode);
            }

        }
    }

    void SetInventoryTabActive(int index)
    {
        for (int i = 1; i <= 3; i++)
        {
            inventoryTransform.GetChild(i).gameObject.SetActive(i == index + 1);
        }
    }

    void SetTabActive(int index)
    {
        Transform tabTransform = GameObject.Find("Tab").transform;
        for (int i = 0; i <= 2; i++)
        {
            tabTransform.GetChild(i).gameObject.SetActive(i == index);
        }
    }
}