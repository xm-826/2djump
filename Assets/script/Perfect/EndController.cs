using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndController : MonoBehaviour
{
    [SerializeField] private Animator aniEnd;
    [SerializeField] private GameObject gameObject1;
    [SerializeField] private TMP_Text coinText;
    private bool canEnd = false;

    private void Start()
    {
        gameObject1.SetActive(false);
        
    }
    private void Update()
    {
        if(canEnd && Input.GetKeyDown(KeyCode.E))
        {
            gameObject1.SetActive(true);

            aniEnd.SetBool("flag",true);
            
            if(GameManager.instance !=null)
            {
                Debug.Log("coinm");
                UpdateCoin(GameManager.instance.CoinCount);
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            canEnd = true;
        }
    }

    public void UpdateCoin(int count)
    {
        coinText.text = "SCORE : "+ count;

    }
    
}
   
