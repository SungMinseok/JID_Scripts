using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager instance;
    void Awake(){
    //Application.targetFrameRate = 60;
    if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        LoadResources();
        PutDictionary();
    }
    
    
    public Sprite[] itemSprites;
    Dictionary<string, Sprite> itemSpritesDic;

    void LoadResources(){
        //audioClips = Resources.LoadAll<AudioClip>("Sounds");
        //bgmClips = Resources.LoadAll<AudioClip>("BGM");
        itemSprites = Resources.LoadAll<Sprite>("Sprites/Items");
    }
    void PutDictionary(){

        itemSpritesDic = new Dictionary<string, Sprite>();
        foreach (Sprite a in itemSprites)
        {
            if (itemSpritesDic.ContainsKey(a.name) == false){

                itemSpritesDic.Add(a.name, a);
                //print(a.name);
            }
        }
    }
    
    public Sprite GetItemSprite(string spriteName){
        if (itemSpritesDic.ContainsKey(spriteName) == false)
        {
            Debug.LogError(spriteName + " is not Contained in Resources");
            return null;
        }
        return itemSpritesDic[spriteName];
    }

    //void Update(){
//        Debug.Log(string.Format("{0}, {1}",PlayerManager.instance.transform.position.x, UIManager.instance.hud_inventory.transform.GetChild(0).position.x));
    //}
}
