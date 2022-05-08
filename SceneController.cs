using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    public TranslateText[] translateTexts;
    public GameObject[] textObjs;
    public Transform centerViewPoint;
    public Transform tempObj;

    
    [Header("Set Demo")]
    public Location[] locations;
    void Awake()
    {
        instance = this;


    }
    void Start(){   
        /* if(SceneManager.GetActiveScene().name == "Main"){
            SoundManager.instance.PlayBGM("jelly in the dark");
        }
        else  */if(SceneManager.GetActiveScene().name.Substring(0,5) == "Level"){
            SoundManager.instance.ChangeBgm("juicy drug");
            SceneController.instance.virtualCamera.Follow = PlayerManager.instance.transform;

            
#if demo
        locations[0].isLocked = true;
#endif

        }

        // Debug.Log("11");     
        // //GameObject[] a = GameObject.FindGameObjectsWithTag("TranslateText");
        // textObjs = GameObject.FindObjectsOfTypeAll("TranslateText");
        // for(int i=0;i<textObjs.Length;i++){
        // Debug.Log("33");     
        //     translateTexts[i] = textObjs[i].GetComponent<TranslateText>();
        // }
        // Debug.Log("22");     

    }
    void OnDisable(){
        StopAllCoroutines();
    }

    public void SetConfiner(int mapNum, bool isDirect = false){
        if(isDirect){
            virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_XDamping = 0;
            virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_YDamping = 0;
            confiner2D.m_BoundingShape2D = mapBounds[mapNum];
            SetCurrentMapName(mapNum);
            Invoke("RecoverConfinerDamping",1f);

        }
        else{
            
            virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_XDamping = 2;
            virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_YDamping = 2;
            
            confiner2D.m_BoundingShape2D = mapBounds[mapNum];
            SetCurrentMapName(mapNum);
        }
        //virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_XDamping = 2;
        //virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_YDamping = 2;
        
           // Debug.Log(num + " : 맵번호");
    }
    public void RecoverConfinerDamping(){
        
        virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_XDamping = 2;
        virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_YDamping = 2;
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
        //DBManager.instance.curData.curMapName = CSVReader.instance.GetIndexToString(mapNum, "map");
        DBManager.instance.curData.curMapNum = mapNum;
    }
    public void SetPlayerPosition(){
        PlayerManager.instance.transform.position = new Vector2(DBManager.instance.curData.playerX,DBManager.instance.curData.playerY);
    }
    public void SetPlayerEquipments(){
        if(DBManager.instance.curData.curEquipmentsID!=null){

            PlayerManager.instance.equipments_id = DBManager.instance.curData.curEquipmentsID;
        }
        PlayerManager.instance.ApplyEquipments();

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
    public void SetCameraNoised(float intensity, float duration){
        SoundManager.instance.PlaySound("shock_effect_"+Random.Range(0,2));
        StartCoroutine(SetCameraNoisedCoroutine(intensity, duration));
    }
    IEnumerator SetCameraNoisedCoroutine(float intensity, float duration){
        virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = intensity;
        yield return new WaitForSeconds(duration);
        virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0;
    }


}
