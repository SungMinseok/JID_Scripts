using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum DragTargetType{
    itemSlot,
    itemImage,
}

public class DragScript : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public DragTargetType type;
    RectTransform rectTransform;
    Canvas canvas;
    ItemSlotScript itemSlotScript;
    int curSlotIndex;//단순 인덱스 (0~10)
    int curItemSlotIndex; //페이지를 고려한 실제 인덱스

    void Start(){
        canvas = UIManager.instance.GetComponent<Canvas>();
        if(type == DragTargetType.itemImage){
            itemSlotScript = GetComponent<ItemSlotScript>();
            rectTransform = InventoryManager.instance.movingItemImage.GetComponent<RectTransform>();
            curSlotIndex = transform.GetSiblingIndex();
        }
    }
    public void OnBeginDrag(PointerEventData eventData){
        if(type == DragTargetType.itemImage){

            curItemSlotIndex = curSlotIndex + (InventoryManager.instance.curPage * InventoryManager.instance.slotCountPerPage);
            //Debug.Log("onBegin : "+curItemSlotIndex);

            //원천차단
            if(DBManager.instance.curData.itemList.Count <= curItemSlotIndex){
                //Debug.Log("ckeks");
                return;
            }

            InventoryManager.instance.itemMoveAccepted = false;
            InventoryManager.instance.itemIsMoving = true;

            InventoryManager.instance.movingItemImage.sprite = 
            DBManager.instance.cache_ItemDataList[DBManager.instance.curData.itemList[curItemSlotIndex].itemID].icon;
            
            rectTransform.anchoredPosition = transform.localPosition;

            itemSlotScript.itemSlot.itemImage.color = new Color(1,1,1,0.3f);
            InventoryManager.instance.movingItemImage.gameObject.SetActive(true);


            //rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }   
    public void OnDrag(PointerEventData eventData){
        if(type == DragTargetType.itemImage){
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }   
    public void OnDrop(PointerEventData eventData){
        if(type == DragTargetType.itemImage){

            curItemSlotIndex = curSlotIndex + (InventoryManager.instance.curPage * InventoryManager.instance.slotCountPerPage);
            //Debug.Log("OnDrop : "+curItemSlotIndex);
            //원천차단
            if(DBManager.instance.curData.itemList.Count <= curItemSlotIndex){
                //Debug.Log("차단");
                return;
            }
            //Debug.Log(curItemSlotIndex + "/" + DBManager.instance.curData.itemList.Count);


            InventoryManager.instance.itemMoveAccepted = true;
            //Debug.Log("$4525");
            InventoryManager.instance.itemMoveEndIndex = 
            transform.GetSiblingIndex() + (InventoryManager.instance.curPage * InventoryManager.instance.slotCountPerPage);
            //Debug.Log(transform.GetSiblingIndex());
            //rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }   
    public void OnEndDrag(PointerEventData eventData){

        if(InventoryManager.instance.itemMoveAccepted){
            InventoryManager.instance.itemMoveAccepted = false;
            InventoryManager.instance.itemIsMoving = false;
            InventoryManager.instance.ChangeItemSlotPosition(curItemSlotIndex,InventoryManager.instance.itemMoveEndIndex);
        }
        

        rectTransform.anchoredPosition = Vector2.zero;
        itemSlotScript.itemSlot.itemImage.color = new Color(1,1,1,1f);
        InventoryManager.instance.movingItemImage.gameObject.SetActive(false);

        // if(!fixedBlock){
                
        //     rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        // }
    }   
}
