using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitPlatfrom : MonoBehaviour
{
    [SerializeField] private float coldTime=0.1f;
    [SerializeField] private GameObject gameObject1;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            StartCoroutine(TimeCold());
            
            
        }
    }

    private IEnumerator TimeCold()
    {
        yield return new WaitForSeconds(coldTime);
        gameObject1.SetActive(true);
    }
}
