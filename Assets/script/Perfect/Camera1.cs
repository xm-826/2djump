using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Camera1 : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject Blue;
    [SerializeField] private GameObject Green;
    [SerializeField] private float line=-30f;
    
    Vector3 start_position;
    
    void Start()
    {
        
        start_position = transform.position - player.transform.position;
    }

    
    void LateUpdate()
    {

        transform.position = start_position + player.transform.position;
        BackgroundTrans();
    }

    private void BackgroundTrans()
    {
        if(player.transform.position.y>line)
        {
            Green.SetActive(true);
            Blue.SetActive(false);
        }
        else
        {
            Green.SetActive(false);
            Blue.SetActive(true);
        }
    }
}



