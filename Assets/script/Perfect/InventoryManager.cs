using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public GameObject InventoryMenu;
    private bool menuActive;
    public ItemSlot[] itemslot;

    //物品配置的加载
    public ItemSo[] itemSos;
    [SerializeField] private PlayerController player;

    public int num;

    //字典来记录物品的数量key为名字，value为数量值
    public Dictionary<string, int> itemDic = new Dictionary<string, int>();
    void Start()
    {
        if (player == null)
            player = FindFirstObjectByType<PlayerController>();
    }

    
    //开关背包
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab) && menuActive)
        {
            Time.timeScale = 1;
            InventoryMenu.SetActive(false);
            menuActive = false;
        }
        else if(Input.GetKeyDown(KeyCode.Tab) && !menuActive)
        {
            Time.timeScale = 0;
            InventoryMenu.SetActive(true);
            menuActive = true;
        }
    }

    //使用物品
    public bool UseItem(string additemName)
    {
        if(!itemDic.ContainsKey(additemName)||itemDic[additemName]<=0)
            return false;
        ItemSo itemSo = GetItemSoByName(additemName);
        if(itemSo==null)
            return false;
        bool useSuccess = itemSo.UseItem(player);
        if(useSuccess)
        {
            itemDic[additemName]-=1;
            if(itemDic[additemName]<=0)
            {
                itemDic.Remove(additemName);
            }
            SyncDictToSlots();
        }
        return useSuccess;
    }

    public void AddItem(ItemSo itemSo,int addnum)
    {
        //如果在字典中有物品的信息则改变物品数量
        if(itemDic.ContainsKey(itemSo.itemName))
            itemDic[itemSo.itemName] +=addnum;
        else
            itemDic.Add(itemSo.itemName,addnum);

        SyncDictToSlots();

        //int leftOver = 
    }

    public void SyncDictToSlots()
    {
        //先清空槽位
        foreach(var slot in itemslot)
            slot.EmptySlot();
        //遍历字典，填入槽位
         int j=0;
        foreach(var i in itemDic)
        {
            int dataCount = i.Value;
            string dataName = i.Key;
            if(dataCount <= 0)
                continue;
            //通过名字找到配置
            ItemSo dataItemSo = GetItemSoByName(dataName);
            if(dataItemSo==null)
                continue;
            
            for(;j<itemslot.Length;j++)
            {
                //当前槽位的物品数量为二者的最小值
                int addToSlot = Mathf.Min(dataCount,dataItemSo.maxNumber);
                itemslot[j].AddItem(dataItemSo.id,dataName,addToSlot,dataItemSo.sprite,dataItemSo.itemDescription,dataItemSo.value,dataItemSo.maxNumber);
                //物品数量改变
                dataCount -= addToSlot;
                //如果dataCount的值等于0则退出，记录J+1,且字典到下一个物品
                if(dataCount == 0)
                {
                    j++;
                    break;
                }
                    
                else if(dataCount<0)
                {
                    Debug.Log("有BUG,在物品数量");
                    break;
                }
            }
        }

    }


    //通过名字找配置
    public ItemSo GetItemSoByName(string addName)
    {
        foreach (var so in itemSos)
        {
            if(so.itemName == addName)
                return so;
        }
        Debug.Log("未找到改名字的配置");
        return null;
    }

     //取消选中
    public void DeselectAllSlots()
    {
        for(int i=0;i<itemslot.Length;i++)
        {
            itemslot[i].selectedShader.SetActive(false);
            itemslot[i].isSelected =false;
        }
    }

    /// <summary>供存档系统读取：返回当前背包的物品字典</summary>
    public Dictionary<string, int> GetInventoryDict()
    {
        return new Dictionary<string, int>(itemDic);
    }

    /// <summary>供存档系统恢复：用存档数据覆盖背包并刷新 UI</summary>
    public void RestoreInventory(Dictionary<string, int> data)
    {
        itemDic.Clear();
        foreach (var kv in data)
        {
            itemDic[kv.Key] = kv.Value;
        }
        SyncDictToSlots();
    }




}
