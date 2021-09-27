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
    }
    public void SetSomeConfiner(Collider2D boundCollider){
        
        confiner2D.m_BoundingShape2D = boundCollider;
    }
    void Start()
    {
    }
    void Update()
    {
        
    }
}
