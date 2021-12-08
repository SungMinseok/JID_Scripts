using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum PointerTargetType{
    itemSlot,
}

public class PointerScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public PointerTargetType type;
    public ItemSlot curItemSlot;
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(type == PointerTargetType.itemSlot){
            curItemSlot = InventoryManager.instance.itemSlot[eventData.pointerEnter.transform.GetSiblingIndex()];
            //Debug.Log(eventData.pointerEnter.transform.GetSiblingIndex());
            if(curItemSlot.itemSlotBtn.interactable)
                curItemSlot.itemDescriptionWindow.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(type == PointerTargetType.itemSlot){
            curItemSlot.itemDescriptionWindow.SetActive(false);
        }
    }

}
