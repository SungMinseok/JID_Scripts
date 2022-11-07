using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HunterManager : MonoBehaviour
{
    public static HunterManager instance;
    public List<int> spawnCondition_itemIDList;
    public int spawnCondition_trigNum;
    public float spawnCondition_requiredTime;
    public NPCScript hunterBody;
    public Dialogue[] dialogues0;
    [Space]
    [Header("Debug━━━━━━━━━━━━━━━━━━━━━")]
    public bool isSpawned;
    public bool timeChecked;
    Coroutine timerCoroutine;
    bool timerCoroutineFlag;



    [SerializeField]
    float requiredTime;

    void Awake(){
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        if(hunterBody!=null) hunterBody.gameObject.SetActive(false);
    }

    void Update(){
        if(DBManager.instance.activateHunter){
            CheckSpawnCondition();

        }

        // if(!isSpawned

        // ){
        //     isSpawned = true;
        //     SpawnHunter();
        // }

    }
    void CheckSpawnCondition(){
        var theDB = DBManager.instance;

        if(!PlayerManager.instance.CheckEquipments(0,spawnCondition_itemIDList)
        &&DBManager.instance.CheckTrigOver(spawnCondition_trigNum)
        
        ){
            if(timerCoroutine == null)
                timerCoroutine = StartCoroutine(SpawnTimerCoroutine());

        }


        else{

            Debug.Log("조건 불충족");
            if(timerCoroutine!=null){
                StopCoroutine(timerCoroutine);
                timerCoroutine = null;
                ResetSpawnCondition();
            }
        }
        
    }

    void ResetSpawnCondition(){
        requiredTime = 0;
    }

    IEnumerator SpawnTimerCoroutine(){
        Debug.Log("SpawnTimerCoroutine");

        WaitForSeconds wait100ms = new WaitForSeconds(0.1f);

        while(!isSpawned){
            yield return wait100ms ;
            requiredTime += 0.1f;
            
            if(requiredTime>=spawnCondition_requiredTime){

                //timeChecked = true;
                SpawnHunter();

            }  
        }

    }

    void SpawnHunter(){
        if(!isSpawned){
            isSpawned = true;
            ResetSpawnCondition();
            //Debug.Log("spawn hunter");
            PlayerManager.instance.CaughtByHunter();
        }
    }
    IEnumerator SpawnHunterCoroutine(){
        yield return null;
    }
    void DestroyHunter(){

    }
}
