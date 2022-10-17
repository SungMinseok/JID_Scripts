using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodySoundScript : MonoBehaviour
{
    public enum BodyType{
        lucky,
        lucky_back,
        pushable,//속도 1 이상일 때, 플레이어 근처일 때 소리재생
        ant,//플레이어 근처일때 걷는 소리 재생
        none
    }
    public BodyType bodyType;
    public bool isNearPlayer;
    public bool alwaysOnPlay;
    [Header("PlayLoopSoundInCollider━━━━━━━━━━━━━━━━━━━━")]
    public bool playLoopSound;
    public string playLoopSoundFileName;
    [Header("pushable━━━━━━━━━━━━━━━━━━━━")]
    //public bool isPushed;
    public MovingSound[] movingSounds;
    [System.Serializable]
    public class MovingSound{
        public string name;
        public WaitForSeconds waitSoundLength;
        
    }
    Rigidbody2D rb;
    bool soundFlag;
    WaitForSeconds wait1550ms = new WaitForSeconds(1.55f);
    void Start(){
        
        if(bodyType == BodyType.pushable){
            rb = GetComponent<Rigidbody2D>();
            //Debug.Log(SoundManager.instance.GetSoundLength("CandyRoll0"));

            for(int i=0;i<movingSounds.Length;i++){
                movingSounds[i].waitSoundLength = new WaitForSeconds(SoundManager.instance.GetSoundLength( movingSounds[i].name));

            }

        }
    }
    void Update(){
        if(bodyType == BodyType.pushable){
            if(isNearPlayer&&(rb.velocity.x >1 || rb.velocity.x < -1) && !soundFlag){
                soundFlag = true;
                StartCoroutine(PlaySoundCoroutine());
            }
        }
    }
    public void PlayWalkSound(){
        switch(bodyType){
            case BodyType.lucky : 
                if(PlayerManager.instance.bodyMode == 1 && PlayerManager.instance.isGrounded)
                    SoundManager.instance.PlaySound("lucky_walk_0"+Random.Range(1,3),0.3f);
                break;
            case BodyType.lucky_back : 
                if(PlayerManager.instance.onRope){

                SoundManager.instance.PlaySound("rope_"+Random.Range(1,6));
                }
                else{
                SoundManager.instance.PlaySound("ladder_0"+Random.Range(1,3));

                }
                break;
            case BodyType.ant : 
                //if(GetComponentInParent<NPCScript>().isNearPlayer){
                if(!alwaysOnPlay){

                    if(isNearPlayer && (!PlayerManager.instance.isPlayingMinigame && !PlayerManager.instance.isGameOver)){
                        SoundManager.instance.PlaySound("AntWalk"+Random.Range(0,4));

                    }
                }
                else{
                    SoundManager.instance.PlaySound("AntWalk"+Random.Range(0,4));

                }
                break;
        }
    } 
    public void PlayRandomSound(string randomSoundName){
        switch(randomSoundName){
            case "pick" : 
                SoundManager.instance.PlaySound("icebreak_0"+Random.Range(1,5));
                break;
            case "antwalk" : 
                SoundManager.instance.PlaySound("AntWalk"+Random.Range(0,4));
                break;
        }
    }
    public void PlaySound(string fileName){
        SoundManager.instance.PlaySound(fileName);
    }
    public void PlaySomeSound(string fileName){
        if(fileName == "mushroom_whip"){
            if(DBManager.instance.curData.curMapNum == 14){
                SoundManager.instance.PlaySound(fileName);
            }
        }
    }
    public void PlaySoundOnlyNearPlayer(string fileName){
        if(isNearPlayer){
            SoundManager.instance.PlaySound(fileName);
        }
    }
    public void test(string a, int b){

    }
    IEnumerator PlaySoundCoroutine(){
        int ranNum = Random.Range(0,movingSounds.Length);
        SoundManager.instance.PlaySound(movingSounds[ranNum].name);
        yield return movingSounds[ranNum].waitSoundLength;
        soundFlag = false;
    }
    void OnTriggerEnter2D(Collider2D other){
        if(playLoopSound){

            if(other.CompareTag("Player")){
                SoundManager.instance.PlayLoopSound(playLoopSoundFileName);
            }
        }

    }
    void OnTriggerExit2D(Collider2D other){
        if(playLoopSound){

            if(other.CompareTag("Player")){
                SoundManager.instance.StopLoopSound();
            }
        }
    }

    //public void PlayLoopSoundIn
}
