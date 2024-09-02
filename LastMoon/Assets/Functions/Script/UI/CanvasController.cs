using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class CanvasController : MonoBehaviourPunCallbacks
{
    public static CanvasController Instance;

    public GameObject Respawn_Text;

    public GameObject inside;
    public GameObject inventory;
    public GameObject money;
    public GameObject Tab;
    public GameObject Poi;
    public GameObject Shop;
    //public GameObject Respawn;
    public GameObject PoiPopup;
    public GameObject Alive;


    public GameObject StationUI;

    public Color EmptyColor;
    public Color IsColor;
    public Transform inventory_Tab;
    private Transform ItemTab;
    private Transform ItemSlot;
    private Transform ItemScroll;

    private Transform ItemTab_Consumable;
    private Transform ItemSlot_Consumable;
    private Transform ItemScroll_Consumable;

    private Transform ItemTab_Construction;
    private Transform ItemSlot_Construction;
    private Transform ItemScroll_Construction;

    private Inventory UIinventory;

    public Transform Recipe_Info;
    public Transform Recipe_Tab;
    private Transform Recipe_Slot;
    public Transform Recipe_Scroll;

    private int keyTabCode = 2;
    private bool inventory_ck;
    private int TotalSell = 0;
    private int Amount = 0;
    public bool SetPoi = false;

    // 노드
    //public GameObject[] nodes;
    //public Text[] nodesCount;
    public Text[] mixCount;
    //public Text[] nodePrice;
    public Text[] mixPrice;

    public int[] count = new int[6]; // 배열 크기 초기화
    public int[] SellCount = new int[6]; // 배열 크기 초기화
    public int[] salePrice = new int[6]; // 배열 크기 초기화
    public int[] nodePriceCount = new int[6];
    public int[] mixPriceCount = new int[6];
    private PlayerController playerController;

    private bool localplayerck = false;
    private bool pricesInitialized = false; // 랜덤 가격이 초기화되었는지 여부

    public Sprite[] UI_ToolIconSprites;
    public Image[] UI_ToolIcons;
    public Color[] UI_ToolDurabilityColor;
    public Image[] UI_ToolDurabilities;

    private int selectedToolIndex = 0;

    private bool StationUIOpened = false;

    //건설 오브젝트
    public ScriptableObject_Item[] ConItem;
    public ScriptableObject_Item[] AuxItem;
    public ScriptableObject_Item[] FixItem;

    public GameObject ConInfoTab;


    //아이템 획득 여부
    private bool isItme = false;

    private Transform inventoryTransform;

    //인벤토리
    public Inventory PlayerInventory;


    //유령모드
    public GameObject hpbar;
    public GameObject toolbar;

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

        ItemTab = inventory_Tab.Find("Misc_Tab");
        ItemScroll = ItemTab.GetChild(0).GetChild(0);
        ItemSlot = ItemScroll.GetChild(0);

        ItemTab_Consumable = inventory_Tab.Find("Food_Tab");
        ItemScroll_Consumable = ItemTab_Consumable.GetChild(0).GetChild(0);
        ItemSlot_Consumable = ItemScroll_Consumable.GetChild(0);

        ItemTab_Construction = inventory_Tab.Find("Mix_Tab");
        ItemScroll_Construction = ItemTab_Construction.GetChild(0).GetChild(0);
        ItemSlot_Construction = ItemScroll_Construction.GetChild(0);


        Recipe_Scroll = Recipe_Tab.GetChild(0).GetChild(0);
        Recipe_Slot = Recipe_Scroll.GetChild(0);

    }

    void Start()
    {
        ToolIconUpdate();
        ToolColorUpdate();
        
        Respawn_Text.SetActive(false);
        Alive.SetActive(false);
        inventory.SetActive(true);
        inside.SetActive(false);
        Tab.SetActive(false);
        money.SetActive(true);
        Poi.SetActive(false);
        inventory.SetActive(false);
        PoiPopup.SetActive(false);
        inventoryTransform = inventory.transform;

        // 마스터 클라이언트인지 확인 후 랜덤 값 초기화 요청
        if (PhotonNetwork.IsMasterClient)
        {
            InitializeAndSendRandomPrices();
        }
        else
        {
            // Non-master clients will request random prices from master
            RequestRandomPricesFromMaster();
        }
        RefreshInventory();
    }
    void Update()
    {
        ToolIconSwitching();
        UpdateInsideActive();
        UpdateInventoryActive();
        UpdateInventoryTabActive();
        UpdateMoneyActive();
        PoiActive();
        PoiPopupActive();
        // 노드 관련 함수
        Die();
        Shoping();
        Station_Manageing();
        RespawnSet();
        Respawn_T();
        //유령시
        Ghost();
        //아이템 업데이트
        if (isItme)
        {
            nodeCountUpdate();
            mixCountUpdate();
        }
        if (Input.GetKeyDown(KeyCode.F9))
        {
            foreach (Item item in UIinventory.GetItems())
            {
                Debug.Log("CanvasController" + item.ItemType + " : " + item.Count);
            }

        }
        // 랜덤 가격이 아직 초기화되지 않았다면, 요청
        RequestRandomPricesFromMaster();

        if(GameValue.is_Winner)
        {
            Alive.SetActive(true);
            Cursor.lockState = CursorLockMode.Confined;
        }
    }


    public void Ghost()
    {
        if(playerController.GhostRespawn)
        {
            hpbar.SetActive(false);
            toolbar.SetActive(false);
            money.SetActive(false);
        }
    }
    [PunRPC]
    void GoToAliveScene()
    {
        PhotonNetwork.LoadLevel("Alive");
    }
    public void Alive_BT()
    {
        PhotonView photonView = PhotonView.Get(this);
        photonView.RPC("GoToAliveScene", RpcTarget.All); // 모든 클라이언트에 씬 전환 명령
    }                                                                       

    public void Respawn_T()
    {
        Respawn_Text.SetActive(playerController.isRespawn);
    }


    public void SetInventory(Inventory inventory)
    {
        UIinventory = inventory;
    }
    public void RefreshInventory()
    {
        foreach (Transform child in ItemScroll)
        {
            if (child == ItemSlot) continue;
            Destroy(child.gameObject);
        }
        foreach (Transform child in ItemScroll_Consumable)
        {
            if (child == ItemSlot_Consumable) continue;
            Destroy(child.gameObject);
        }
        foreach (Transform child in ItemScroll_Construction)
        {
            if (child == ItemSlot_Construction) continue;
            Destroy(child.gameObject);
        }

        if (playerController != null) SetInventory(playerController.PlayerInventory);
        int x = 0;
        int y = 19;
        float itemSlotSize = 150f;
        int x_Construction = 0;
        int y_Construction = 19;
        int x_Consumable = 0;
        int y_Consumable = 19;

        if (UIinventory != null)
        {
            foreach (Item item in UIinventory.GetItems())
            {
                RectTransform itemRectTransform = Instantiate(ItemSlot, ItemScroll).GetComponent<RectTransform>();
                itemRectTransform.gameObject.SetActive(true);

                itemRectTransform.anchoredPosition = new Vector2(x * itemSlotSize, y * itemSlotSize);

                Image image = itemRectTransform.Find("Icon").GetComponent<Image>();
                image.sprite = item.ItemType.ItemSprite;

                Text text = itemRectTransform.Find("Count").GetComponent<Text>();
                text.text = item.Count.ToString();

                text = itemRectTransform.Find("Price").GetComponent<Text>();
                text.text = item.ItemType.Price.ToString();

                Button_Sell sellbutton = itemRectTransform.Find("Button_Sell").GetComponent<Button_Sell>();
                sellbutton.RegisterPlayerController(playerController);
                sellbutton.ItemType = item.ItemType;

                y--;
                if (item.ItemType.ItemType == 1)
                {
                    itemRectTransform = Instantiate(ItemSlot_Construction, ItemScroll_Construction).GetComponent<RectTransform>();
                    itemRectTransform.gameObject.SetActive(true);

                    itemRectTransform.anchoredPosition = new Vector2(x_Construction * itemSlotSize, y_Construction * itemSlotSize);

                    image = itemRectTransform.Find("Icon").GetComponent<Image>();
                    image.sprite = item.ItemType.ItemSprite;

                    text = itemRectTransform.Find("Count").GetComponent<Text>();
                    text.text = item.Count.ToString();

                    text = itemRectTransform.Find("Price").GetComponent<Text>();
                    text.text = item.ItemType.Price.ToString();

                    sellbutton = itemRectTransform.Find("Button_Sell").GetComponent<Button_Sell>();
                    sellbutton.RegisterPlayerController(playerController);
                    sellbutton.ItemType = item.ItemType;

                    y_Construction--;
                }
                else if (item.ItemType.ItemType == 2)
                {
                    itemRectTransform = Instantiate(ItemSlot_Consumable, ItemScroll_Consumable).GetComponent<RectTransform>();
                    itemRectTransform.gameObject.SetActive(true);

                    itemRectTransform.anchoredPosition = new Vector2(x_Consumable * itemSlotSize, y_Consumable * itemSlotSize);

                    image = itemRectTransform.Find("Icon").GetComponent<Image>();
                    image.sprite = item.ItemType.ItemSprite;

                    text = itemRectTransform.Find("Count").GetComponent<Text>();
                    text.text = item.Count.ToString();

                    text = itemRectTransform.Find("Price").GetComponent<Text>();
                    text.text = item.ItemType.Price.ToString();

                    sellbutton = itemRectTransform.Find("Button_Sell").GetComponent<Button_Sell>();
                    sellbutton.RegisterPlayerController(playerController);
                    sellbutton.ItemType = item.ItemType;

                    Button_Use Usebutton = itemRectTransform.Find("Button_Use").GetComponent<Button_Use>();
                    Usebutton.RegisterPlayerController(playerController);
                    Usebutton.ItemType = item.ItemType;

                    y_Consumable--;
                }
            }
        }
    }

    public void ConMatUpdate()
    {
        if (playerController != null) SetInventory(playerController.PlayerInventory);
        if (UIinventory != null)
        {
            for (int i = 0; i < AuxItem.Length; i++)
            {
                ConCountUpdate(AuxItem[i], 1, i + 1);
            }
            for (int i = 0; i < FixItem.Length; i++)
            {
                ConCountUpdate(FixItem[i], 2, i + 1);
            }
            for (int i = 0; i < ConItem.Length; i++)
            {
                ConCountUpdate(ConItem[i], 3, i);
            }
        }
    }
    public void ConCountUpdate(ScriptableObject_Item ItemType, int SlotType, int SlotNum)
    {
        Button button;
        Text text;

        button = ConInfoTab.transform.GetChild(SlotType).GetChild(SlotNum).GetComponent<Button>();
        text = ConInfoTab.transform.GetChild(SlotType).GetChild(SlotNum).GetChild(1).GetComponent<Text>();
        int ConCount = 0;
        foreach (Item item in UIinventory.GetItems())
        {
            if (item.ItemType == ItemType) ConCount = item.Count;
        }
        foreach (Item item in playerController.ConstInventory.GetItems())
        {
            if (item.ItemType == ItemType) ConCount -= item.Count;
        }
        if (ConCount > 0)
        {
            button.interactable = true;
            text.text = ConCount.ToString();
        }
        else
        {
            button.interactable = false;
            text.text = "";
        }
    }

    public void PoiPopupActive()
    {
        //PoiPopup.SetActive(playerController.PoiPopUp);
    }
     
    public void PoiPopupActive_BT()
    {
        playerController.PoiPopUp = false;
        PoiPopup.SetActive(playerController.PoiPopUp);
    }

    public void Extract_BT()
    {
        playerController.Extract = !playerController.Extract;
    }


    public void RecipeSelect()
    {
        ScriptableObject_Recipe[] SelectableRecipes = playerController.UISelectedPOIController.SelectableRecipes;

        foreach (Transform child in Recipe_Scroll)
        {
            if (child == Recipe_Slot) continue;
            Destroy(child.gameObject);
        }

        int x = -1;
        int y = 1;
        float itemSlotSize = 300f;
        for (int i = 0; i < SelectableRecipes.Length; i++)
        {
            RectTransform RecipeRectTransform = Instantiate(Recipe_Slot, Recipe_Scroll).GetComponent<RectTransform>();
            RecipeRectTransform.gameObject.SetActive(true);

            RecipeRectTransform.anchoredPosition = new Vector2(x * itemSlotSize, y * itemSlotSize + 535f);

            Image image = RecipeRectTransform.Find("Icon").GetComponent<Image>();
            image.sprite = SelectableRecipes[i].Output[0].ItemSprite;

            Text text = RecipeRectTransform.Find("Text").GetComponent<Text>();
            if (SelectableRecipes[i].OutputCount > 1) text.text = ("+" + SelectableRecipes[i].OutputCount.ToString());
            else text.text = "";

            SeletRecipe Recipebutton = RecipeRectTransform.GetComponent<SeletRecipe>();
            Recipebutton.RegisterStationController(playerController.UISelectedPOIController);
            Recipebutton.SelectableRecipe = SelectableRecipes[i];

            Button button = RecipeRectTransform.GetComponent<Button>();
            if (playerController.UISelectedPOIController.TempertureLimit < SelectableRecipes[i].Temperture)
            {
                button.interactable = false;
            }

            if (SelectableRecipes[i].RequireUnlock)
            {
                bool unlockedRecipe = false;
                for (int j = 0; j < playerController.UnlockableRecipes.Length; j++)
                {
                    if (playerController.UnlockableRecipes[j] == SelectableRecipes[i])
                    {
                        unlockedRecipe = playerController.UnlockedRecipe[j];
                    }
                }
                button.interactable = unlockedRecipe;
            }

            x++;
            if (x > 1)
            {
                x = -1;
                y--;
            }
        }
    }
    private void SelectedRecipeInfo(ScriptableObject_Recipe SelectedRecipe)
    {
        Text text = Recipe_Info.Find("Recipe_Temperture").GetChild(0).GetComponent<Text>();
        text.text = "(" + SelectedRecipe.Temperture.ToString() + ")";
        text = Recipe_Info.Find("Recipe_Temperture").GetChild(1).GetComponent<Text>();
        text.text = playerController.UISelectedPOIController.StationTemperture.ToString();

        text = Recipe_Info.Find("Recipe_ProcessTime").GetChild(0).GetComponent<Text>();
        text.text = "(" + SelectedRecipe.ProgressTime.ToString() + ")";
        text = Recipe_Info.Find("Recipe_ProcessTime").GetChild(1).GetComponent<Text>();
        text.text = (playerController.UISelectedPOIController.StationProgress / SelectedRecipe.ProgressTime * 100f).ToString("F0") + "%";

        text = Recipe_Info.Find("Recipe_Coolent").GetChild(0).GetComponent<Text>();
        text.text = "(" + SelectedRecipe.Coolent.ToString() + ")";
        text = Recipe_Info.Find("Recipe_Coolent").GetChild(1).GetComponent<Text>();
        if (playerController.UISelectedPOIController.Inv_Coolent != null)
            text.text = playerController.UISelectedPOIController.Inv_Coolent.Count.ToString();
        else
            text.text = "0";

        Image image;

        switch (SelectedRecipe.InputCount)
        {
            case 1:
                image = Recipe_Info.Find("Recipe_Input001").GetChild(0).GetComponent<Image>();
                text = Recipe_Info.Find("Recipe_Input001").GetChild(1).GetComponent<Text>();
                image.sprite = SelectedRecipe.Input[0].ItemSprite;
                image.color = IsColor;
                if (playerController.UISelectedPOIController.Inv_Input[0] != null)
                    text.text = playerController.UISelectedPOIController.Inv_Input[0].Count.ToString();
                else
                    text.text = "0";

                image = Recipe_Info.Find("Recipe_Input002").GetChild(0).GetComponent<Image>();
                text = Recipe_Info.Find("Recipe_Input002").GetChild(1).GetComponent<Text>();
                image.color = EmptyColor;
                text.text = "";

                image = Recipe_Info.Find("Recipe_Input003").GetChild(0).GetComponent<Image>();
                text = Recipe_Info.Find("Recipe_Input003").GetChild(1).GetComponent<Text>();
                image.color = EmptyColor;
                text.text = "";

                break;
            case 2:
                image = Recipe_Info.Find("Recipe_Input001").GetChild(0).GetComponent<Image>();
                text = Recipe_Info.Find("Recipe_Input001").GetChild(1).GetComponent<Text>();
                image.sprite = SelectedRecipe.Input[0].ItemSprite;
                image.color = IsColor;
                if (playerController.UISelectedPOIController.Inv_Input[0] != null)
                    text.text = playerController.UISelectedPOIController.Inv_Input[0].Count.ToString();
                else
                    text.text = "0";

                image = Recipe_Info.Find("Recipe_Input002").GetChild(0).GetComponent<Image>();
                text = Recipe_Info.Find("Recipe_Input002").GetChild(1).GetComponent<Text>();
                image.sprite = SelectedRecipe.Input[1].ItemSprite;
                image.color = IsColor;
                if (playerController.UISelectedPOIController.Inv_Input[1] != null)
                    text.text = playerController.UISelectedPOIController.Inv_Input[1].Count.ToString();
                else
                    text.text = "0";

                image = Recipe_Info.Find("Recipe_Input003").GetChild(0).GetComponent<Image>();
                text = Recipe_Info.Find("Recipe_Input003").GetChild(1).GetComponent<Text>();
                image.color = EmptyColor;
                text.text = "";

                break;
            case 3:
                image = Recipe_Info.Find("Recipe_Input001").GetChild(0).GetComponent<Image>();
                text = Recipe_Info.Find("Recipe_Input001").GetChild(1).GetComponent<Text>();
                image.sprite = SelectedRecipe.Input[0].ItemSprite;
                image.color = IsColor;
                if (playerController.UISelectedPOIController.Inv_Input[0] != null)
                    text.text = playerController.UISelectedPOIController.Inv_Input[0].Count.ToString();
                else
                    text.text = "0";

                image = Recipe_Info.Find("Recipe_Input002").GetChild(0).GetComponent<Image>();
                text = Recipe_Info.Find("Recipe_Input002").GetChild(1).GetComponent<Text>();
                image.sprite = SelectedRecipe.Input[1].ItemSprite;
                image.color = IsColor;
                if (playerController.UISelectedPOIController.Inv_Input[1] != null)
                    text.text = playerController.UISelectedPOIController.Inv_Input[1].Count.ToString();
                else
                    text.text = "0";

                image = Recipe_Info.Find("Recipe_Input003").GetChild(0).GetComponent<Image>();
                text = Recipe_Info.Find("Recipe_Input003").GetChild(1).GetComponent<Text>();
                image.sprite = SelectedRecipe.Input[2].ItemSprite;
                image.color = IsColor;
                if (playerController.UISelectedPOIController.Inv_Input[2] != null)
                    text.text = playerController.UISelectedPOIController.Inv_Input[2].Count.ToString();
                else
                    text.text = "0";

                break;
        }
        switch (SelectedRecipe.OutputCount)
        {
            case 1:
                image = Recipe_Info.Find("Recipe_Output001").GetChild(0).GetComponent<Image>();
                text = Recipe_Info.Find("Recipe_Output001").GetChild(1).GetComponent<Text>();
                image.sprite = SelectedRecipe.Output[0].ItemSprite;
                image.color = IsColor;
                if (playerController.UISelectedPOIController.Inv_Output[0] != null)
                    text.text = playerController.UISelectedPOIController.Inv_Output[0].Count.ToString();
                else
                    text.text = "0";

                image = Recipe_Info.Find("Recipe_Output002").GetChild(0).GetComponent<Image>();
                text = Recipe_Info.Find("Recipe_Output002").GetChild(1).GetComponent<Text>();
                image.color = EmptyColor;
                text.text = "";

                image = Recipe_Info.Find("Recipe_Output003").GetChild(0).GetComponent<Image>();
                text = Recipe_Info.Find("Recipe_Output003").GetChild(1).GetComponent<Text>();
                image.color = EmptyColor;
                text.text = "";

                break;
            case 2:
                image = Recipe_Info.Find("Recipe_Output001").GetChild(0).GetComponent<Image>();
                text = Recipe_Info.Find("Recipe_Output001").GetChild(1).GetComponent<Text>();
                image.sprite = SelectedRecipe.Output[0].ItemSprite;
                image.color = IsColor;
                if (playerController.UISelectedPOIController.Inv_Output[0] != null)
                    text.text = playerController.UISelectedPOIController.Inv_Output[0].Count.ToString();
                else
                    text.text = "0";

                image = Recipe_Info.Find("Recipe_Output002").GetChild(0).GetComponent<Image>();
                text = Recipe_Info.Find("Recipe_Output002").GetChild(1).GetComponent<Text>();
                image.sprite = SelectedRecipe.Output[1].ItemSprite;
                image.color = IsColor;
                if (playerController.UISelectedPOIController.Inv_Output[1] != null)
                    text.text = playerController.UISelectedPOIController.Inv_Output[1].Count.ToString();
                else
                    text.text = "0";

                image = Recipe_Info.Find("Recipe_Output003").GetChild(0).GetComponent<Image>();
                text = Recipe_Info.Find("Recipe_Output003").GetChild(1).GetComponent<Text>();
                image.color = EmptyColor;
                text.text = "";

                break;
            case 3:
                image = Recipe_Info.Find("Recipe_Output001").GetChild(0).GetComponent<Image>();
                text = Recipe_Info.Find("Recipe_Output001").GetChild(1).GetComponent<Text>();
                image.sprite = SelectedRecipe.Output[0].ItemSprite;
                image.color = IsColor;
                if (playerController.UISelectedPOIController.Inv_Output[0] != null)
                    text.text = playerController.UISelectedPOIController.Inv_Output[0].Count.ToString();
                else
                    text.text = "0";

                image = Recipe_Info.Find("Recipe_Output002").GetChild(0).GetComponent<Image>();
                text = Recipe_Info.Find("Recipe_Output002").GetChild(1).GetComponent<Text>();
                image.sprite = SelectedRecipe.Output[1].ItemSprite;
                image.color = IsColor;
                if (playerController.UISelectedPOIController.Inv_Output[1] != null)
                    text.text = playerController.UISelectedPOIController.Inv_Output[1].Count.ToString();
                else
                    text.text = "0";

                image = Recipe_Info.Find("Recipe_Output003").GetChild(0).GetComponent<Image>();
                text = Recipe_Info.Find("Recipe_Output003").GetChild(1).GetComponent<Text>();
                image.sprite = SelectedRecipe.Output[2].ItemSprite;
                image.color = IsColor;
                if (playerController.UISelectedPOIController.Inv_Output[2] != null)
                    text.text = playerController.UISelectedPOIController.Inv_Output[2].Count.ToString();
                else
                    text.text = "0";

                break;
        }
    }

    public void Station_Input()
    {
        int ItemRequireCount = 1;
        if (Input.GetKey(KeyCode.LeftShift)) ItemRequireCount *= 10;
        if (Input.GetKey(KeyCode.LeftControl)) ItemRequireCount *= 5;

        int[] ItemRequireCounts = new int[3] { 1, 1, 1 };
        int[] ItemInputCounts = new int[3] { 1, 1, 1 };

        ScriptableObject_Recipe SelectedRecipe = playerController.UISelectedPOIController.SelectedRecipe;

        for (int i = 0; i < SelectedRecipe.InputCount; i++)
        {
            if (playerController.UISelectedPOIController.Inv_Input[i] != null)
            {
                ItemRequireCounts[i] =
                    playerController.UISelectedPOIController.
                    Inv_Input[i].ItemType.MaxCount -
                    playerController.UISelectedPOIController.
                    Inv_Input[i].Count;
            }
            else
            {
                ItemRequireCounts[i] = SelectedRecipe.Input[i].MaxCount;
            }

            if (!Input.GetKey(KeyCode.LeftAlt) && ItemRequireCounts[i] > ItemRequireCount) ItemRequireCounts[i] = ItemRequireCount;
            if (playerController.PlayerInventory.RemoveItem(new Item
            {
                ItemType = SelectedRecipe.Input[i],
                Count = ItemRequireCounts[i]
            }))
            {
                ItemInputCounts[i] = ItemRequireCounts[i];
            }
            else
                ItemInputCounts[i] = playerController.PlayerInventory.ClearItem(SelectedRecipe.Input[i]);
            playerController.UISelectedPOIController.InputItem(i, ItemInputCounts[i]);
        }
    }

    public void Station_Output()
    {
        playerController.ExtractStation(playerController.UISelectedPOIController, 2);
    }
    /*
    public void Station_Input_Item(int SlotNum)
    {
        ScriptableObject_Recipe SelectedRecipe = playerController.UISelectedPOIController.SelectedRecipe;

        int ItemRequireCount = 1;
        if (Input.GetKey(KeyCode.LeftShift)) ItemRequireCount *= 10;
        if (Input.GetKey(KeyCode.LeftControl)) ItemRequireCount *= 5;

        playerController.Station_Input_Item(SlotNum, 0, SelectedRecipe.Input[SlotNum], ItemRequireCount);
    }
     */
    public void Station_Input_Coolent(ScriptableObject_Item ItemType)
    {
        int ItemRequireCount = 1;
        if (Input.GetKey(KeyCode.LeftShift)) ItemRequireCount *= 10;
        if (Input.GetKey(KeyCode.LeftControl)) ItemRequireCount *= 5;
        playerController.Station_Input_Item(0, 3, ItemType, ItemRequireCount);
    }


    public void Station_Manageing()
    {
        if (playerController != null)
        {
            StationUI.SetActive(playerController.StationActive);

            if (playerController.StationActive)
            {
                if (!StationUIOpened)
                {
                    RecipeSelect();
                    StationUIOpened = true;
                }
                SelectedRecipeInfo(playerController.UISelectedPOIController.SelectedRecipe);
                Cursor.lockState = CursorLockMode.Confined;
            }
            else StationUIOpened = false;
        }
    }
    public void Station_Exit()
    {
        playerController.StationActive = false;
        StationUI.SetActive(playerController.StationActive);
        Cursor.lockState = CursorLockMode.Locked;
    }


    public void Shoping()
    {
        if(playerController!=null)
        {
            Shop.SetActive(playerController.ShopActive);
            if(playerController.ShopActive)
            {
                Cursor.lockState = CursorLockMode.Confined;
            }
        }
    }

    public void RespawnSet()
    {
        //if(playerController !=null)
        //{
        //    Debug.Log("RespawnAcive" + PlayerController.RespawnAcive);
        //    Respawn.SetActive(PlayerController.RespawnAcive);
        //}
    }

    public void Shoping_Exit()
    {
        playerController.ShopActive = false;
        Shop.SetActive(playerController.ShopActive);
        Cursor.lockState = CursorLockMode.Locked;
    }

    void InitializeAndSendRandomPrices()
    {
        int[] tempNodePriceCount = new int[6];
        int[] tempMixPriceCount = new int[6];

        for (int i = 0; i < 6; i++)
        {
            // Generate random values
            tempNodePriceCount[i] = Random.Range(10, 51); // 10~50 사이의 랜덤값
            tempMixPriceCount[i] = Random.Range(100, 151); // 100~150 사이의 랜덤값

            // Update UI elements with random values
            //nodePrice[i].text = tempNodePriceCount[i].ToString();
            mixPrice[i].text = tempMixPriceCount[i].ToString();
        }

        // Send random values to all clients
        PhotonView.Get(this).RPC("UpdatePriceCounts", RpcTarget.AllBuffered, tempNodePriceCount, tempMixPriceCount);

        for (int i = 0; i < 6; i++)
        {
            GameValue.getPrice(i, nodePriceCount[i], mixPriceCount[i]);
        }
    }

    [PunRPC]
    void UpdatePriceCounts(int[] nodePrices, int[] mixPrices)
    {
        if (nodePrices.Length != 6 || mixPrices.Length != 6)
        {
            Debug.LogError("Invalid array length received in RPC.");
            return;
        }

        for (int i = 0; i < 6; i++)
        {
            nodePriceCount[i] = nodePrices[i];
            mixPriceCount[i] = mixPrices[i];

            //nodePrice[i].text = nodePriceCount[i].ToString();
            mixPrice[i].text = mixPriceCount[i].ToString();
            GameValue.getPrice(i, nodePriceCount[i], mixPriceCount[i]);
        }

        // 랜덤 값이 초기화된 것으로 플래그 설정
        pricesInitialized = true;
    }


    public void Sell()
    {
        money.SetActive(true);
        /*
        for (int i = 0; i < nodesCount.Length; i++)
        {
            Amount = nodePriceCount[i] * playerController.nodeItiems[i];
            TotalSell += Amount;
        }
        for(int i=0; i< mixCount.Length; i++)
        {
            Amount = mixPriceCount[i] * playerController.mixItiems[i];
            TotalSell += Amount;
        }
        for (int i = 0; i < nodesCount.Length; i++)
        {
            nodesCount[i].text = "0";
            playerController.nodeItiems[i] = 0;
        }
        for(int i=0; i<mixCount.Length;i++)
        {
            mixCount[i].text = "0";
            playerController.mixItiems[i] = 0;
        }
         */
        TotalSell = playerController.Sell();
        GameValue.GetMomey(TotalSell);
    }

    void RequestRandomPricesFromMaster()
    {
        if (!pricesInitialized)
        {
            PhotonView.Get(this).RPC("RequestRandomPrices", RpcTarget.MasterClient);
        }
    }

    [PunRPC]
    void RequestRandomPrices()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            InitializeAndSendRandomPrices();
        }
    }

    public void PoiActive()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            SetPoi = !SetPoi;
            if (SetPoi)
            {
                ConMatUpdate();
                Poi.SetActive(true);
                Cursor.lockState = CursorLockMode.Confined;
            }
            else
            {
                ConMatUpdate();
                Poi.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
            }
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
        if (playerController != null)
        {
            // 기존 이벤트 리스너 제거
            playerController.OnInventoryChanged -= nodeCountUpdate;
            playerController.OnInventoryChanged -= RefreshInventory;
            playerController.OnInventoryChanged -= ConMatUpdate;
        }

        playerController = player;

        if (playerController != null)
        {
            // 새로운 이벤트 리스너 등록
            playerController.OnInventoryChanged += nodeCountUpdate;
            playerController.OnInventoryChanged += RefreshInventory;
            playerController.OnInventoryChanged += ConMatUpdate;
            nodeCountUpdate(); // 플레이어 등록 후 초기 인벤토리 업데이트
            RefreshInventory();
            ConMatUpdate();
            isItme = true;
        }

        localplayerck = true;
    }

    public void nodeCountUpdate()
    {
        if (playerController != null)
        {
            for (int i = 0; i < 6; i++)
            {
                count[i] = playerController.nodeItiems[i];
                //nodesCount[i].text = count[i].ToString();
            }
        }
    }

    public void mixCountUpdate()
    {
        for (int i = 0; i < 6; i++)
        {
            count[i] = playerController.mixItiems[i];
            mixCount[i].text = count[i].ToString();
        }
    }

    private void Die()
    {
        if (PlayerController.Hp == 0)
        {
            /*
            for (int i = 0; i < nodesCount.Length; i++)
            {
                //nodesCount[i].text = "0";
                mixCount[i].text = "0";
            }
            */
        }
    }

    void UpdateMoneyActive()
    {
        if (GameValue.lived)
        {
            money.SetActive(true);
            inventory.SetActive(false);
            Tab.SetActive(false);
        }
    }

    void UpdateInsideActive()
    {
        inside.SetActive(PlayerController.insideActive);
    }

    void UpdateInventoryActive()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            RefreshInventory();
            inventory_ck = !inventory_ck;
            if(inventory_ck)
            {
                Cursor.lockState = CursorLockMode.Confined;
            }
            else
                Cursor.lockState = CursorLockMode.Locked;
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
