using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class PingZhang : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            player.setBlood(-1);     
        }
    }
}
