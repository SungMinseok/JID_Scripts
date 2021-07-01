using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System; 
public class DebugManager : MonoBehaviour
{
    public static DebugManager instance;
    public bool isDebugMode;
    public uint buildNum;
    public GameObject debugPanel;
    public Text buildInfoText;
    public Text buildDateText;


    [SerializeField]
    List<AlertDebug> alertDebugList = new List<AlertDebug>();
    public GameObject alertPanel, textObject;

    void Awake(){
        //Application.targetFrameRate = 60;
        // if (null == instance)
        // {
        //     instance = this;
        //     DontDestroyOnLoad(this.gameObject);
        // }
        // else
        // {
        //     Destroy(this.gameObject);
        // }
    }

    void Start()
    {
#if UNITY_EDITOR || alpha
 		isDebugMode = true;
#else
        isDebugMode = false;
#endif
        if(isDebugMode){
            debugPanel.SetActive(true);
        }
        else{
            debugPanel.SetActive(false);
        }
        buildInfoText.text = "Build # : "+ buildNum.ToString();
        buildInfoText.text += "\n"+ DateTime.Now.ToString(("yyyy-MM-dd"));  




    }

    public void PrintDebug(string text){
        if(isDebugMode){    

            if(alertDebugList.Count>3){
                Destroy(alertDebugList[0].textObject.gameObject);
                alertDebugList.Remove(alertDebugList[0]);
            }

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
