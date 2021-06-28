using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;
    public PolygonCollider2D[] mapBounds;
    public CinemachineVirtualCamera virtualCamera;
    public CinemachineConfiner2D confiner2D;
    // Start is called before the first frame update
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

    public void SetConfiner(int num){
        confiner2D.m_BoundingShape2D = mapBounds[num];
    }
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
