using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [Header("[Game Settings] ─────────────────────")]
    [SerializeField]
    [Tooltip("상호작용 시 카메라 줌")]
    public bool mode_zoomWhenInteract;
    // Start is called before the first frame update
    void Awake(){
        instance = this;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
