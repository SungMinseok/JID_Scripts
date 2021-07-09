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
    public uint buildNum;
    public GameObject debugPanel,cheatPanel;
    public Text buildInfoText;
    public Text buildDateText;
    public Animator animator;
    


    [SerializeField]
    List<AlertDebug> alertDebugList = new List<AlertDebug>();
    public GameObject alertPanel, textObject;

    void Awake(){
        Application.targetFrameRate = 60;
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
        //instance = this;
    }

    void Start()
    {
        //animator = GetComponent<Animator>();
#if UNITY_EDITOR || alpha
 		isDebugMode = true;
        debugPanel.SetActive(true);
        buildInfoText.text = "Build # : "+ buildNum.ToString();
        buildInfoText.text += "\n"+ DateTime.Now.ToString(("yyyy-MM-dd"));  
#else
        isDebugMode = false;
        debugPanel.SetActive(false);
        cheatPanel.SetActive(false);
#endif




    }

    
#if UNITY_EDITOR || alpha
    void Update(){
        if(isDebugMode){
            if(Input.GetKeyDown(KeyCode.Return)){
                cheatPanel.SetActive(!cheatPanel.activeSelf);
                CheatManager.instance.cheat.Select();
                CheatManager.instance.cheat.ActivateInputField();
                
            }
            if(Input.GetKeyDown(KeyCode.F10)){
                SceneManager.LoadScene("warehouse");
            }
        }
    }
#endif

    public void PrintDebug(string text){
        if(isDebugMode){    

            if(alertDebugList.Count>50){
                Destroy(alertDebugList[0].textObject.gameObject);
                alertDebugList.Remove(alertDebugList[0]);
            }
            animator.SetTrigger("activate");

            AlertDebug newAlertDebug = new AlertDebug();
            newAlertDebug.text= "[" + DateTime.Now.ToString(("mm:ss:ff")) + "] " + text;

            GameObject newText = Instantiate(textObject,alertPanel.transform);
            newAlertDebug.textObject = newText.GetComponent<Text>();
            newAlertDebug.textObject.text = newAlertDebug.text;


            alertDebugList.Add(newAlertDebug);

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
