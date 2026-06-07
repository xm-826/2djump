using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextTriiger : MonoBehaviour
{
    [SerializeField] private Animator ani;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //Debug.Log("11111");
            if (ani != null)
            {
                ani.SetBool("flag", true);
                //Debug.Log("222");
            }
                
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (ani != null)
                ani.SetBool("flag", false);
        }
    }
}
