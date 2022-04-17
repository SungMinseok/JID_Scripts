using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System; 
using UnityEngine.SceneManagement;
public class DebugManager : MonoBehaviour
{
    public static DebugManager instance;
    public bool isDebugMode;
    public bool cheatAvailable;
    //public uint buildNum;
    //public string buildDate;
    public GameObject debugPanel,cheatPanel;
    public Text buildInfoText;
    public Text buildDateText;
    public Animator animator;
    Vector3 playerOriginPos;

    


    [SerializeField]
    List<AlertDebug> alertDebugList = new List<AlertDebug>();
    public GameObject alertPanel, textObject;

    void Awake(){
        
        // if (null == instance)
        // {
        //     instance = this;
        //     DontDestroyOnLoad(this.gameObject);
        // }
        // else
        // {
        //     Destroy(this.gameObject);
        // }
        instance = this;
    }

    void Start()
    {
        
        // playerOriginPos = PlayerManager.instance.transform.position;
        //animator = GetComponent<Animator>();
#if UNITY_EDITOR || alpha
 		isDebugMode = true;
        debugPanel.SetActive(true);
        buildInfoText.text = "Build # : "+ DBManager.instance.buildNum.ToString();
        //buildInfoText.text += "\n"+ DateTime.Now.ToString(("yyyy-MM-dd"));  
        buildInfoText.text += "\n"+ DBManager.instance.buildDate;  
#else
        isDebugMode = false;
        debugPanel.SetActive(false);
        cheatPanel.SetActive(false);
#endif




    }

    
#if UNITY_EDITOR || alpha
    // void Update(){
    //     if(isDebugMode){
    //         if(Input.GetKeyDown(KeyCode.Return)){
    //             cheatPanel.SetActive(!cheatPanel.activeSelf);
    //             //if(PlayerManager.instance.canMove) PlayerManager.instance.canMove = !cheatPanel.activeSelf;
    //             CheatManager.instance.cheat.Select();
    //             CheatManager.instance.cheat.ActivateInputField();
                
    //         }
    //         if(Input.GetKeyDown(KeyCode.F10)){
    //             //SceneManager.LoadScene("warehouse");
    //             PlayerManager.instance.RevivePlayer();
    //             CheatManager.instance.InputCheat("t 0");
    //             //ResetPlayerPos();
    //         }
    //     }
    // }

#endif

    public void PrintDebug(string text){
        if(isDebugMode){    
            Debug.Log(text);

            // if(alertDebugList.Count>50){
            //     Destroy(alertDebugList[0].textObject.gameObject);
            //     alertDebugList.Remove(alertDebugList[0]);
            // }
            // animator.SetTrigger("activate");

            // AlertDebug newAlertDebug = new AlertDebug();
            // newAlertDebug.text= "[" + DateTime.Now.ToString(("mm:ss:ff")) + "] " + text;

            // GameObject newText = Instantiate(textObject,alertPanel.transform);
            // newAlertDebug.textObject = newText.GetComponent<Text>();
            // newAlertDebug.textObject.text = newAlertDebug.text;


            // alertDebugList.Add(newAlertDebug);

        }
        
    }

    // Update is called once per frame
    // void Update()
    // {
    //     if(Input.GetKeyDown(KeyCode.Space)){
    //         PrintDebug("test");
    //     }
    // }
}



[System.Serializable]
public class AlertDebug{
    public string text;
    public Text textObject;
}
