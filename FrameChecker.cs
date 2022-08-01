using UnityEngine;
using System.Collections;
 
#if UNITY_EDITOR || alpha
public class FrameChecker : MonoBehaviour
{
  float deltaTime = 0.0f;

  GUIStyle style0, style1, style2;
  Rect rect0, rect1, rect2;
  string text0, text1,text2;
  float msec;
  float fps;
  float worstFps=100f;

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

    //StartCoroutine ("worstReset");
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

    // text1 = "Build # : "+ DBManager.instance.buildNum.ToString() + 
    //         " / "+ DBManager.instance.buildDate;  
    // }
    // else{
      
    text1 = "Build # : "+ DBManager.instance.buildNum.ToString() + "." + DBManager.instance.buildSubNum +
            " / "+ DBManager.instance.buildDate;  
    //}
    GUI.Label(rect1, text1, style1);  
    text2 = "0먹이창고\n	1절벽\n	2복도\n	3노개미 방\n	4세 갈래 길\n	5유치원\n	6수개미 방\n	7수개미 끝방\n	8광장\n	9식당\n	10부화장\n	11대왕 일개미방\n	12농장가는 길\n	13냉동굴\n	14버섯농장\n	15진딧물농장\n	16나가는 길\n	17병원\n	18시장가는 길\n	19야시장\n	20두 갈래 길\n	21히든월드\n	22귀족의 길\n	23공주개미 방\n	24여왕개미 방\n";
    GUI.Label(rect2, text2, style2);
  }
} 

#endif
