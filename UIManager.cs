using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    Vector3 playerOriginPos;
    PlayerManager player;
    public GameObject clearPanel;

    void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    void Start()
    {
        player = PlayerManager.instance;
        playerOriginPos = player.transform.position;
    }


    public void ResetPos(){
        player.transform.position = playerOriginPos;
    }
}
