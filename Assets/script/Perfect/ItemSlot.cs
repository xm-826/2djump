using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using GladeAgenticAI.Bridge;
using UnityEngine.EventSystems;
public class ItemSlot : MonoBehaviour,IPointerClickHandler
{
    //物品数据
    [Header("物品数据")]
    [SerializeField] public string itemName;

    [SerializeField] public int ItemId;
    //槽位中物品的数量
    [SerializeField] public int num;
    [SerializeField] private Sprite sprite;
    [SerializeField] private string itemDescription;
    //槽位堆叠的最大值
    [SerializeField] public int maxNumber;
    public bool isFull;

    //槽位数据
    [Header("槽位数据")]
    [SerializeField] public TMP_Text numText;
    [SerializeField] private Image itemImage;

    [Header("描述槽位数据")]
    [SerializeField] private TMP_Text ItemDesciptionNameText;
    [SerializeField] private TMP_Text ItemDesciptionText;
    public Image itemDescriptionImage;
    [SerializeField] private Sprite emptySprite;


    public GameObject selectedShader;
    public bool isSelected;

    [SerializeField] private InventoryManager inventoryManager;

    private void Awake()
    {
        if (inventoryManager == null)
            inventoryManager = FindFirstObjectByType<InventoryManager>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            OnLeftClick();
        }
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            OnRightClick();
        }
    }

    //数据的分配与计算已经在物品管理器中设置好，在itemSlot只需加载即可
    public int AddItem(int addId, string addname, int addnum, Sprite addsprite, string itemDescription, float addvalue, int addMaxnumber)
    {
        this.ItemId = addId;
        this.itemName = addname;
        this.sprite = addsprite;
        this.itemDescription = itemDescription;
        this.maxNumber = addMaxnumber;
        this.num = addnum;

        // 更新UI
        itemImage.sprite = sprite;
        numText.text = num.ToString();
        numText.enabled = num > 0;
        isFull = num >= maxNumber;

        return 0;
    }

    public void OnLeftClick()
    {
        if(isSelected)
        {
            bool useAble = inventoryManager.UseItem(itemName);
        }

        else
        {
             //取消选择
            inventoryManager.DeselectAllSlots();
            // 选择
            selectedShader.SetActive(true);
            isSelected =true;
            ItemDesciptionNameText.text = itemName.ToString();
            ItemDesciptionText.text = itemDescription.ToString();
            if(itemDescriptionImage.sprite == null) 
                itemDescriptionImage.sprite = emptySprite;
            itemDescriptionImage.sprite = sprite;
        }
            
    }

    public void OnRightClick()
    {
        
    }

    public void EmptySlot()
    {
        itemName = "";
        ItemId = 0;
        num = 0;
        numText.enabled = false;
        itemImage.sprite = emptySprite;
        ItemDesciptionNameText.text = "";
        ItemDesciptionText.text = "";
        itemDescriptionImage.sprite = emptySprite;
        isSelected = false;
        selectedShader.SetActive(false);
        isFull = false;
    }
}
