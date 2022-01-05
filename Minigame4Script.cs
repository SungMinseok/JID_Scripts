using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
//using System;
//Level, Stage  : Level이 상위 개념
//Level : 총 3레벨
//Stage : 레벨 당 3스테이지

//진딧물 미니게임
public class Minigame4Script : MonoBehaviour
{

    //x : 7.5  y : 4
    [Header("[Game Settings]─────────────────")]
    public float aphidLifeTime = 2f;
    public float aphidCreationCycle = 1;
    public int aphidCreationCount = 100;
    public float miniAntSpeed;
    public float miniLuckySpeed;



    
    [Space]
    [Header("[Game Objects]─────────────────")]
    public GameObject[] aphids;
    public Transform miniLucky, miniAnt;
    public Vector2 miniAntDestination;
    public PolygonCollider2D mapCollider;
    public Transform mapViewPoint;
    [Space]

    [Header("[Debug]─────────────────")]
    public int closestAphid;
    public List<float> tempDistanceList;
    public List<int> tempAphidNumList;
    public float curMapScrollSpeed = 0 ;
    Coroutine minigameCoroutine;
    public bool isActive;

    WaitForSeconds waitAphidLifeTime;
    WaitForSeconds waitAphidCreationCycle;

    WaitForSeconds wait1s = new WaitForSeconds(1f);
    WaitForSeconds wait2s = new WaitForSeconds(2f);
    WaitForSeconds wait3s = new WaitForSeconds(3f);
    Vector3 flyingBottlePos;
    public Transform test;

    public float wInput;
    public float hInput;
    public float correctionValue;
    public bool creationFlag;

    void Start(){
        waitAphidCreationCycle = new WaitForSeconds(aphidCreationCycle);
    }
    
    private void OnEnable() {
        Debug.Log("A");
        minigameCoroutine = StartCoroutine(MinigameCoroutine());

        SceneController.instance.SetSomeConfiner(mapCollider);
        SceneController.instance.virtualCamera.Follow = mapViewPoint;



        //int i = Random.Range(0,1);
    }
    // void Update(){
    //     if(!creationFlag){
    //         creationFlag = true;
            
    //     }
        
    // }    

    void Update(){
        
        wInput = Input.GetAxisRaw("Horizontal");
        hInput = Input.GetAxisRaw("Vertical");
    }
    void FixedUpdate(){

        
        if(wInput!=0 || hInput!=0){
            miniLucky.Translate(new Vector2(wInput * miniLuckySpeed,hInput * miniLuckySpeed));
        }

        if(miniAntDestination != Vector2.zero){
            miniAnt.position = Vector2.MoveTowards(miniAnt.position,miniAntDestination,miniAntSpeed);
        }
    }
    IEnumerator MinigameCoroutine(){

        SetRandomPosition();

        for(int i=0;i<aphidCreationCount;i++){
            GameObject curAphid = aphids[Random.Range(0,aphids.Length)];

            if(curAphid.activeSelf){
                Debug.Log("AA");
                continue;
            }

            aphids[Random.Range(0,aphids.Length)].SetActive(true);
            CalculateDistance();
            yield return waitAphidCreationCycle;
        }

    }

    public void SetRandomPosition(){
        foreach(GameObject aphid in aphids){
            aphid.transform.localPosition = new Vector2(Random.Range(-7.5f,7.5f),Random.Range(-4f,2.5f));
        }
    }

    public void CalculateDistance(){
        // tempDistanceList.Clear();
        // tempAphidNumList.Clear();
        Transform closestAphid = null;
        float closestDistance = 100f;

        for(int i=0;i<aphids.Length;i++){
            if(!aphids[i].activeSelf) continue;
            float tempDistance = Vector2.Distance(miniAnt.position,aphids[i].transform.position);
            if(closestDistance>tempDistance){
                closestDistance = tempDistance;
                closestAphid = aphids[i].transform;
            }
            // tempDistanceList.Add(Vector2.Distance(miniAnt.position,aphids[i].transform.position));
            // tempAphidNumList.Add(i);
        }

        miniAntDestination = new Vector2(closestAphid.position.x,closestAphid.position.y);


        //miniAntDestination = new Vector2(closestAphid)
    }
}
