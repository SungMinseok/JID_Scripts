using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    //Vector3 playerOriginPos;
    PlayerManager thePlayer;
    //public GameObject clearPanel;
    public GameObject bgBlack;
    [Header("UI_States")]
    public Image dirtGauge;
    public TextMeshProUGUI honeyText;
    [Header("UI_Select")]
    public GameObject ui_select;
    public Transform ui_select_grid;
    public Color non_selected_color;
    public Color selected_color;
    public Sprite non_selected_sprite;
    public Sprite selected_sprite;
    [Header("UI_Effects")]
    public Transform effects;
    public bool onEffect;
    [Header("UI_GameOver")]
    public GameObject ui_gameOver;
    public Image ui_gameOver_image;
    public GameObject gameOverBtns;
    public Sprite[] gameOverSprites;
    public Animator gameOverNewImage;

    [Header("UI_GameEnd")]
    public GameObject ui_gameEnd;
    public CanvasGroup gameEndImageCanvas, gameEndTextCanvas0, gameEndTextCanvas1;
    public Image gameEndImage;
    public TextMeshProUGUI gameEndText0;
    public TextMeshProUGUI gameEndText1_0;
    public TextMeshProUGUI gameEndText1_1;
    public GameObject gameEndSkipBtn;
    public GameObject gameEndNextBtn;
    public bool gameEndCanSkip;
    [Header("UI_Fader")]
    public Animator ui_fader;
    [Header("UI_HUD")]
    public GameObject hud_state;
    public GameObject hud_inventory;
    [Header("UI_Book")]
    public GameObject ui_book;
    public Image bookMainImage;
    public Image bookTextImage;
    public Sprite[] bookMainSprites;
    public Sprite[] bookTextSprites;
    [Header("UI_Tutorial")]
    public GameObject ui_tutorial;
    public Transform tutorialSet;
    public bool waitTutorial;
    public int curTutorialID;
    // [Header("UI_Fog")]
    // public Canvas ui_fog_canvas;
    // public Transform ui_fog;
    // Vector3 offset = Vector3.zero;

    //WaitForSeconds waitTime = new WaitForSeconds(0.5f);
    WaitForSeconds wait10ms = new WaitForSeconds(0.01f);

    WaitForSeconds wait100ms = new WaitForSeconds(0.1f);
    WaitForSeconds wait500ms = new WaitForSeconds(0.5f);
    WaitForSeconds wait1000ms = new WaitForSeconds(1);
    WaitForSeconds wait2000ms = new WaitForSeconds(2);
    WaitForSeconds wait3000ms = new WaitForSeconds(3);

    int calculatedHoney;
    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        thePlayer = PlayerManager.instance;
        calculatedHoney = 0;
        honeyText.text = "0";
        ResetGameOverUI();
        //playerOriginPos = thePlayer.transform.position;
        // offset = transform.position - worldToUISpace(ui_fog_canvas, PlayerManager.instance.transform.position);
    }
    void OnDisable()
    {
        StopAllCoroutines();
    }
    void Update()
    {
 
        // KeyCode code = (KeyCode)Enum.Parse(typeof(KeyCode), alphaValue.ToString());
        // if(Input.GetKeyDown((KeyCode)Enum.Parse(typeof(KeyCode), alphaValue.ToString());))

        if (dirtGauge.fillAmount != DBManager.instance.curData.curDirtAmount / DBManager.instance.maxDirtAmount)
        {
            dirtGauge.fillAmount = DBManager.instance.curData.curDirtAmount / DBManager.instance.maxDirtAmount;
        }

        // if(honeyText.text != DBManager.instance.curData.curHoneyAmount.ToString()){

        //     int temp = DBManager.instance.curData.curHoneyAmount - int.Parse(honeyText.text);
        //     //honeyText.text = DBManager.instance.curData.curHoneyAmount.ToString();
        //     honeyText.text = temp > 0 ?  += 1;
        // }

        if (calculatedHoney != DBManager.instance.curData.curHoneyAmount)
        {
            int temp = DBManager.instance.curData.curHoneyAmount - calculatedHoney;
            if (temp >= 10 || temp <= -10)
            {
                calculatedHoney = calculatedHoney + temp / 10;
            }
            else
            {
                calculatedHoney = temp > 0 ? calculatedHoney + 1 : calculatedHoney - 1;

            }
            honeyText.text = string.Format("{0:#,###0}", calculatedHoney);
        }

        if (waitTutorial)
        {
            if (Input.GetButtonDown("Interact_OnlyKey"))
            {
                CloseTutorial();
            }
        }
    }

    public void ActivateEffect(int num, float timer, bool bgOn = true)
    {
        onEffect = true;
        StartCoroutine(ActivateEffectCoroutine(num, timer, bgOn));
    }
    IEnumerator ActivateEffectCoroutine(int num, float timer, bool bgOn)
    {

        var canvasGroup = effects.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;



        if (bgOn)
        {
            effects.GetChild(0).gameObject.SetActive(true);
        }

        effects.GetChild(num).gameObject.SetActive(true);



        while (canvasGroup.alpha <= 0.99f)
        {
            canvasGroup.alpha += 0.1f;
            yield return null;
        }


        yield return new WaitForSeconds(timer);



        while (canvasGroup.alpha >= 0.11)
        {
            canvasGroup.alpha -= 0.1f;
            yield return null;
        }

        effects.GetChild(num).gameObject.SetActive(false);

        if (bgOn)
        {
            effects.GetChild(0).gameObject.SetActive(false);
        }

        canvasGroup.alpha = 1;

        onEffect = false;
    }

    public void SetFadeOut(float speed = 1f)
    {
        ui_fader.SetTrigger("fadeOut");
    }
    public void SetFadeIn(float speed = 1f)
    {
        ui_fader.SetTrigger("fadeIn");
    }
    public void ResetFader(float value)
    {
        var defaultColor = ui_fader.GetComponent<Image>().color;
        ui_fader.GetComponent<Image>().color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, value);
    }
    public void FirstSceneLoad()
    {
        ResetFader(1);
        ui_fader.SetTrigger("fadeIn");
    }
    #region GameOver
    public void SetGameOverUI(int collectionID)
    {
        //PlayerManager.instance.LockPlayer();
        PlayerManager.instance.isGameOver = true;
        SoundManager.instance.BgmOff();
        StartCoroutine(SetGameOverUICoroutine(collectionID));
    }
    public void ResetGameOverUI()
    {
        gameOverBtns.SetActive(false);
        gameOverNewImage.gameObject.SetActive(false);
    }
    IEnumerator SetGameOverUICoroutine(int collectionID)
    {
        bool endingAlreadyOver;

        if (DBManager.instance.GetClearedEndingCollectionID(collectionID) == -1)
        {
            DBManager.instance.EndingCollectionOver(collectionID);
            endingAlreadyOver = false;
        }
        else
        {
            endingAlreadyOver = true;
        }

        if (collectionID == 21)
        {
            yield return wait2000ms;
        }
        else if (collectionID == 1)
        {
            yield return wait500ms;
        }

        yield return wait1000ms;
        yield return wait500ms;
        //SetFadeOut();
        LoadManager.instance.FadeOut();
        yield return wait1000ms;
        yield return wait500ms;

        switch (collectionID)
        {
            case 1:
                SoundManager.instance.PlaySound("spear_death");
                yield return wait3000ms;
                yield return wait500ms;
                break;
                // case 3 : 
                //     SoundManager.instance.PlaySound("ending_minigamefail");
                //     break;
        }


        if (MinigameManager.instance.nowMinigameNum != -1) MinigameManager.instance.minigameScriptTransforms[MinigameManager.instance.nowMinigameNum].gameObject.SetActive(false);
        ui_gameOver_image.sprite = DBManager.instance.endingCollectionSprites[collectionID];
        //ui_gameOver_image.sprite = gameOverSprites[num]; DBManager.instance.endingCollectionSprites[tempCardNum[i]]
        ui_gameOver.SetActive(true);
        LoadManager.instance.FadeIn();

        SoundManager.instance.PlaySound("gameover" + Random.Range(0, 3));
        yield return new WaitForSeconds(2f);
        gameOverBtns.gameObject.SetActive(true);


        if (!endingAlreadyOver)
        {

            UIManager.instance.gameOverNewImage.gameObject.SetActive(true);
        }
    }
    #endregion

    #region GameEnd
    public void SetGameEndUI(int num)
    {
        PlayerManager.instance.isGameOver = true;
        for (int i = 0; i < DBManager.instance.cache_GameEndDataList.Count; i++)
        {
            if (DBManager.instance.cache_GameEndDataList[i].endingNum == num)
            {
                Debug.Log(i + "번 엔딩 코루틴 시작");
                StartCoroutine(SetGameEndUICoroutine(i));
                return;
            }
        }

    }
    IEnumerator SetGameEndUICoroutine(int num)
    {
        GameEndList curGameEndList = DBManager.instance.cache_GameEndDataList[num];
        DBManager.instance.EndingCollectionOver(curGameEndList.endingCollectionNum);
        var reader0 = CSVReader.instance.data_collection;
        var reader1 = CSVReader.instance.data_story;
        string language = DBManager.instance.language;

        LoadManager.instance.FadeOut();
        yield return wait2000ms;


        gameEndImageCanvas.alpha = 0;
        gameEndTextCanvas0.alpha = 0;
        gameEndTextCanvas1.alpha = 0;
        gameEndNextBtn.SetActive(false);
        //gameEndSkipBtn.SetActive(false);

        ui_gameEnd.SetActive(true);
        LoadManager.instance.FadeIn();


        //for(int i=0;i<curGameEndList.storySprites.Length;i++){

        gameEndImage.sprite = curGameEndList.stories[0].sprite;

        gameEndImageCanvas.alpha = 0;
        while (gameEndImageCanvas.alpha < 1)
        {
            gameEndImageCanvas.alpha += 0.01f;
            yield return wait10ms;
        }

        yield return wait500ms;
        //}

        for (int i = 0; i < curGameEndList.stories.Length; i++)
        {

            if (curGameEndList.stories[i].sprite != null && i != 0)
            {
                gameEndImage.sprite = curGameEndList.stories[i].sprite;
            }

            //gameEndText0.text = curGameEndList.stories[i].descriptions;
            gameEndText0.text = reader1[int.Parse(curGameEndList.stories[i].descriptions)]["text_" + language].ToString();//curGameEndList.stories[i].descriptions;

            gameEndTextCanvas0.alpha = 0;
            while (gameEndTextCanvas0.alpha < 1)
            {
                gameEndTextCanvas0.alpha += 0.01f;
                yield return wait10ms;
            }

            yield return wait1000ms;
            gameEndCanSkip = true;
            gameEndNextBtn.SetActive(true);
            yield return new WaitUntil(() => !gameEndCanSkip);
            gameEndNextBtn.SetActive(false);

        }


        while (gameEndTextCanvas0.alpha > 0)
        {
            gameEndImageCanvas.alpha -= 0.01f;
            gameEndTextCanvas0.alpha -= 0.01f;
            yield return wait10ms;
        }

        yield return wait500ms;

        gameEndText1_0.text = "ending no." + curGameEndList.endingNum;
        //gameEndText1_1.text = curGameEndList.name;
        gameEndText1_1.text = reader0[curGameEndList.endingCollectionNum]["name_" + language].ToString();

        while (gameEndTextCanvas1.alpha < 1)
        {
            gameEndTextCanvas1.alpha += 0.01f;
            yield return wait10ms;
        }
        yield return wait2000ms;
        while (gameEndTextCanvas1.alpha > 0)
        {
            gameEndTextCanvas1.alpha -= 0.01f;
            yield return wait10ms;
        }
        yield return wait500ms;

        LoadManager.instance.LoadMain();
    }
    public void PushNextBtn()
    {

        gameEndCanSkip = false;
    }
    #endregion


    #region Book
    public void OpenBookUI(int bookMainNum, int bookTextNum)
    {
        bookMainImage.sprite = bookMainSprites[bookMainNum];
        bookTextImage.sprite = bookTextSprites[bookTextNum];
        ui_book.SetActive(true);
    }


    #endregion

    #region Tutorial
    public void OpenTutorialUI(int tutorialNum)
    {
        waitTutorial = true;
        curTutorialID = tutorialNum;
        PlayerManager.instance.LockPlayer();
        SoundManager.instance.PlaySound("item_get");

        for (int i = 0; i < tutorialSet.childCount; i++)
        {
            tutorialSet.GetChild(i).gameObject.SetActive(false);
        }

        int indexOfItemList = -1;//DBManager.instance.curData.itemList.FindIndex(x => x.itemID == 21);
        bool needSetPos = false;


        switch (tutorialNum)
        {
            case 0:
                indexOfItemList = GetMyItemIndex(5);
                needSetPos = true;
                break;
            case 3:
                indexOfItemList = GetMyItemIndex(21);
                needSetPos = true;
                break;
            case 4:
                indexOfItemList = GetMyItemIndex(2);
                needSetPos = true;
                break;
        }


        //if(tutorialNum == 3){
        //int indexOfItemList = DBManager.instance.curData.itemList.FindIndex(x => x.itemID == 21);
        
        if (needSetPos)
            {

                var panelPos = tutorialSet.GetChild(tutorialNum).GetChild(0).localPosition;
                Debug.Log(panelPos);

                if (indexOfItemList != -1)
                {
                    tutorialSet.GetChild(tutorialNum).GetChild(0).localPosition = new Vector2(panelPos.x, 442.2f - indexOfItemList * 63.07f);
                }
                else
                {
                    tutorialSet.GetChild(tutorialNum).GetChild(0).localPosition = new Vector2(panelPos.x, 442.2f);
                }
            }


        //}

        tutorialSet.GetChild(tutorialNum).gameObject.SetActive(true);
        ui_tutorial.gameObject.SetActive(true);
    }

    public int GetMyItemIndex(int _itemID)
    {
        return DBManager.instance.curData.itemList.FindIndex(x => x.itemID == _itemID);
    }

    public void CloseTutorial()
    {
        waitTutorial = false;
        ui_tutorial.gameObject.SetActive(false);
        for (int i = 0; i < tutorialSet.childCount; i++)
        {
            tutorialSet.GetChild(i).gameObject.SetActive(false);
        }
        PlayerManager.instance.UnlockPlayer();


    }


    #endregion


    public void SetHUD(bool active)
    {
        hud_state.SetActive(active);
        hud_inventory.SetActive(active);

        if (!active)
        {
            for (int i = 0; i < InventoryManager.instance.itemSlot.Length; i++)
            {
                InventoryManager.instance.itemSlot[i].itemDescriptionWindow.SetActive(false);
            }
        }
    }
    public void SetFadeHUD(bool active)
    {
    }
    public void ChangeSprite(Sprite sprite)
    {

    }
//     public void ChangeKey(){
//         foreach(KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
// {
//     if (Input.GetKey(kcode))
//         Debug.Log("KeyCode down: " + kcode);
// }
//         GameInputManager.SetKeyMap("Jump",)
//     }
    // public Vector3 worldToUISpace(Canvas parentCanvas, Vector3 worldPos)
    // {
    //     //Convert the world for screen point so that it can be used with ScreenPointToLocalPointInRectangle function
    //     Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
    //     Vector2 movePos;

    //     //Convert the screenpoint to ui rectangle local point
    //     RectTransformUtility.ScreenPointToLocalPointInRectangle(parentCanvas.transform as RectTransform, screenPos, parentCanvas.worldCamera, out movePos);
    //     //Convert the local point to world point
    //     return parentCanvas.transform.TransformPoint(movePos);
    // }

}
