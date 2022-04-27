using UnityEngine;
using System.Collections;
 
#if UNITY_EDITOR || alpha
public class FrameChecker : MonoBehaviour
{
  float deltaTime = 0.0f;

  GUIStyle style0, style1;
  Rect rect0, rect1;
  float msec;
  float fps;
  float worstFps=100f;
  string text0, text1;

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


    rect0 = new Rect(0, h - style0.fontSize, w, style0.fontSize);
    rect1 = new Rect(0, 0, w, style1.fontSize);

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
      style1.normal.textColor = new Color(0,1,1,setAlphaState);
      style0.normal.textColor = new Color(0,1,1,setAlphaState);

        
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

    text0 = "FPS:"+fps.ToString ("F1");
    GUI.Label(rect0, text0, style0);
    text1 = "Build # : "+ DBManager.instance.buildNum.ToString() + 
            " / "+ DBManager.instance.buildDate;  ;
    GUI.Label(rect1, text1, style1);
  }
} 

#endif
