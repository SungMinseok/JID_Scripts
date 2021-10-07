using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SceneController : MonoBehaviour
{
    public static SceneController instance;
    [Header("맵")]
    public PolygonCollider2D[] mapBounds;
    public CinemachineVirtualCamera virtualCamera;
    public CinemachineConfiner2D confiner2D;
    [Header("NPC")]
    public List<NPCScript> npcs;
    [Header("오브젝트")]
    public Transform[] objects;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    public void SetConfiner(int num){
        confiner2D.m_BoundingShape2D = mapBounds[num];
        SetCurrentMapName(num);
        
           // Debug.Log(num + " : 맵번호");
    }
    public void SetSomeConfiner(Collider2D boundCollider){
        
        confiner2D.m_BoundingShape2D = boundCollider;
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
    void Start()
    {
    }
    void Update()
    {
        
    }
}
