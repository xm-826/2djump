using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "ScriptableObjects/Item")]
public class ItemSo : ScriptableObject
{
   
    [Header("物品相关的属性")]
    public string itemName;//名字
    public int num;//物品数量
    public int id;//物品Id
    public Sprite sprite;//物品的精灵
    public string itemDescription; //物品描述
    public float value;//物品的作用
    public float valueTime;
    
    public int maxNumber;


    //需要修改的属性
    public enum StatToChange
    {
        none,
        blood,
        speed
    };

    public StatToChange statToChange;

    public bool UseItem(PlayerController player)
    {
        if (player == null) return false;

        switch (statToChange)
        {
            case StatToChange.blood:
                player.setBlood(Mathf.Min(value, 1));
                Debug.Log("blood+1");
                return true;
            case StatToChange.speed:
                player.setSpeed(value, valueTime);
                return true;
            case StatToChange.none:
            default:
                break;
        }
        return false;
    }  

}
