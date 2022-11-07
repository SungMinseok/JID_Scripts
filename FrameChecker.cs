using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Collections.Generic;
 
#if UNITY_EDITOR || alpha
public class FrameChecker : MonoBehaviour
{
  float deltaTime = 0.0f;

  GUIStyle style0, style1, style2, style3;
  Rect rect0, rect1, rect2, rect3;
  string text0, text1,text2;
  float msec;
  float fps;
  float worstFps=100f;
    string itemList;

    //string currentTimeText;

    string COMMAND_KEY = "key";
    string COMMAND_ARGUMENTS = "arguments";
    string COMMAND_CHEATDESCRIPTION = "cheatDescription";
    string ITEM_ID = "ID";
    string ITEM_NAME = "name_kr";

    void Awake()
  {
    int w = Screen.width, h = Screen.height;

    
    style0 = new GUIStyle();
    style0.alignment = TextAnchor.LowerLeft;
    style0.fontSize = h * 2 / 100;
    style0.normal.textColor = Color.cyan;

    
    style1 = new GUIStyle();
    style1.alignment = TextAnchor.UpperLeft;
    style1.fontSize = h * 2 / 100;
    style1.normal.textColor = Color.cyan;
    
    style2 = new GUIStyle();
    style2.alignment = TextAnchor.UpperRight;
    style2.fontSize = h * 2 / 100;
    style2.normal.textColor = Color.cyan;


    rect0 = new Rect(0, h - style0.fontSize, w, style0.fontSize);
    rect1 = new Rect(0, 0, w, style1.fontSize);
    rect2 = new Rect(-130, 0, w, style2.fontSize);


    
    style3 = new GUIStyle();
    style3.alignment = TextAnchor.UpperLeft;
    style3.fontSize = (int)(h * 0.015);
    style3.normal.textColor = Color.cyan;
    rect3 = new Rect(260, 0, 0, style3.fontSize);

    //currentTimeText = DateTime.Now.ToString(("yyMMdd_HHmm"));

    //StartCoroutine ("worstReset");
  }
    void Start(){
        //itemList = new List<string>();
        int i = 0;
        foreach(var a in CSVReader.instance.data_item){
            itemList += a[ITEM_ID] + " " + a[ITEM_NAME] + "\t";
            if(++i%5==0) itemList += "\n";

        }

        // foreach(Item a in DBManager.instance.cache_ItemDataList){
        //     itemList += a.ID + " " + a.name + "\n";
        // }
    }

  // IEnumerator worstReset() //코루틴으로 15초 간격으로 최저 프레임 리셋해줌.
  // {
  //   while (true) {
  //     yield return new WaitForSeconds(15f);
  //     worstFps = 100f;
  //   }
  // }


  void Update()
  {
    deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        //if(isDebugMode){
    if(Input.GetKeyDown(KeyCode.F11)){    
      //GUI.Label(rect0, text0, GUIStyle.none);
      var curAlphaState = style0.normal.textColor.a ;
      var setAlphaState = curAlphaState == 0 ? 1 : 0;
      style0.normal.textColor = new Color(0,1,1,setAlphaState);
      style1.normal.textColor = new Color(0,1,1,setAlphaState);
      style2.normal.textColor = new Color(0,1,1,setAlphaState);

        
    }
        //}
  }
 
  void OnGUI()//소스로 GUI 표시.
  {

    //msec = deltaTime * 1000.0f;
    fps = 1.0f / deltaTime;  //초당 프레임 - 1초에

    // if (fps < worstFps)  //새로운 최저 fps가 나왔다면 worstFps 바꿔줌.
    //   worstFps = fps;
    // text = msec.ToString ("F1") + "ms (" + fps.ToString ("F1") + ") //worst : " + worstFps.ToString ("F1");

    text0 = "FPS:"+fps.ToString ("F1") + " / F11 : on/off";
    GUI.Label(rect0, text0, style0);
    //if(DBManager.instance.buildSubNum==0){

      
    //text1 = "Build # : "+ DBManager.instance.buildNum.ToString() + "." + DBManager.instance.buildSubNum +
    //        " / "+ DBManager.instance.buildDate;

    text1 = string.Format("Build v{0}",Application.version);

    GUI.Label(rect1, text1, style1);  
    text2 = "먹이창고0/1\n	절벽2/3\n	복도4/5\n	노개미 방6/7\n	세 갈래 길8/9\n	유치원10/11\n	수개미 방12/13\n	수개미 끝방14/15\n	광장16/17\n	식당18/19\n	부화장20/21\n	대왕 일개미방22/23\n	농장가는 길24/25\n	냉동굴26/27\n	버섯농장28/29\n	진딧물농장30/31\n	나가는 길32/33\n  병원34/35\n	시장가는 길36/37\n	야시장38/39\n	두 갈래 길40/41\n	히든월드42/43\n	귀족의 길44/45\n	공주개미 방46/47\n	여왕개미 방48/49\n\n  맵이동명령어 : t 숫자\n ex)병원 오른쪽 : t 35";
    //GUI.Label(rect2, text2, style2);
    //GUI.Label(rect3, itemList, style3);

    
        

  }
} 

#endif
