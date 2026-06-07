using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private int coinCount;
    public int CoinCount => coinCount;
    public static GameManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static GameManager Instance
    {
        get
        {
            return instance;
        }
    }

    
    public void CoinAdd(int amount=1)
    {
        coinCount += amount;
        UIController.instance.UpdateCoin(coinCount);
    }

    public void CoinZero()
    {
        coinCount = 0;
        UIController.instance.UpdateCoin(CoinCount);
    }
}
