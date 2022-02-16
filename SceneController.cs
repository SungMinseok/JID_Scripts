using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SceneController : MonoBehaviour
{
    public static SceneController instance;
    [Header("세팅값")]
    public float defaultZoomInSize = 4f;
    public float defaultZoomOutSize = 5.3f;
    [Header("맵")]
    public PolygonCollider2D[] mapBounds;
    public PolygonCollider2D[] mapZoomBounds;
    public CinemachineVirtualCamera virtualCamera;
    public CinemachineConfiner2D confiner2D;
    [Header("NPC")]
    public List<NPCScript> npcs;
    [Header("오브젝트")]
    public Transform[] objects;
    // Start is called before the first frame update
    public Collider2D temp;
    void Awake()
    {
        instance = this;
    }

    public void SetConfiner(int mapNum){
        confiner2D.m_BoundingShape2D = mapBounds[mapNum];
        SetCurrentMapName(mapNum);
        
           // Debug.Log(num + " : 맵번호");
    }
    public void SetSomeConfiner(Collider2D boundCollider = null, bool isDirect = false){
        if(isDirect){
            virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_XDamping = 0;
            virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_YDamping = 0;

        }
        else{
            
            virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_XDamping = 2;
            virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_YDamping = 2;
        }
        if(boundCollider == null) temp = confiner2D.m_BoundingShape2D;
        confiner2D.m_BoundingShape2D = boundCollider;
        //virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_XDamping = 2;
        //virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_YDamping = 2;
    }
    public void SetCurrentMapName(int mapNum){
        //Debug.Log(DBManager.instance);
        DBManager.instance.curData.curMapName = CSVReader.instance.GetIndexToString(mapNum, "map");
        DBManager.instance.curData.curMapNum = mapNum;
    }
    public void SetPlayerPosition(){
        PlayerManager.instance.transform.position = new Vector2(DBManager.instance.curData.playerX,DBManager.instance.curData.playerY);
    }
    public void SetFirstLoad(){
        
        SceneController.instance.SetCurrentMapName(0);
        //DBManager.instance.curData.playerX = PlayerManager.instance.transform.position.x;
        //DBManager.instance.curData.playerY = PlayerManager.instance.transform.position.y;
    }
    public void CameraView(Transform target, float speed=2){
        if(target!=null){
            SceneController.instance.virtualCamera.Follow = target;//ObjectController.instance.npcs[0].transform;
        }
        else{
            //DM("Error : no pos");
        }
    }
    
    public void SetLensOrthoSize(float value, float speed = 0.1f){
        if(virtualCamera.m_Lens.OrthographicSize > value){
            StartCoroutine(SetDownLensOrthoSizeCoroutine(value, speed));
        }
        else{
            StartCoroutine(SetUpLensOrthoSizeCoroutine(value, speed));
        }
    }
    IEnumerator SetDownLensOrthoSizeCoroutine(float value, float speed){  // size = 3.5, y= 4 
        while (virtualCamera.m_Lens.OrthographicSize > value)
        {
            virtualCamera.m_Lens.OrthographicSize -= speed;
            yield return null;
        }
    }
    IEnumerator SetUpLensOrthoSizeCoroutine(float value, float speed){  // size = 5.3, y= 6 
        while (virtualCamera.m_Lens.OrthographicSize < value)
        {
            virtualCamera.m_Lens.OrthographicSize += speed;
            yield return null;
        }
    }
    public void SetCameraDefaultZoomIn(){

        SetLensOrthoSize(defaultZoomInSize,0.075f);
    }
    public void SetCameraDefaultZoomOut(){

        SetLensOrthoSize(defaultZoomOutSize,0.1f);
    }


    #region Main Control
    public void PushStartBtn(){
        LoadManager.instance.StartBtn();
    }

    #endregion
}
