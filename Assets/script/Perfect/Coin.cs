using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("金币属性设置")]
    [SerializeField] private Transform start1;
    [SerializeField] private Transform end1;
    [SerializeField] private float speed = 0.5f;
    [SerializeField] private int amount = 1;
    private SpriteRenderer sp;

    private void Start()
    {
        sp = GetComponent<SpriteRenderer>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (GameManager.instance != null)
            {
                GameManager.instance.CoinAdd(amount);
            }
            GetComponent<Collider2D>().enabled = false;
            StartCoroutine(DeestroyCoin(start1.position, end1.position));
            
        }
    }

    IEnumerator DeestroyCoin(Vector3 start, Vector3 end)
    {
        float t = 0 ;
        while(t<1)
        {
            t += Time.deltaTime * speed;
            transform.position = Vector3.Lerp(start,end, t);

            Color color=sp.color;
            color.a = Mathf.Lerp(1f, 0f, t);
            sp.color = color;

            yield return null;
        }
        
        Destroy(gameObject);
        

    }


}
