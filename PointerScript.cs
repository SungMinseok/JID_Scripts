using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum PointerTargetType{
    itemSlot,
    inventoryTab,
}

public class PointerScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public PointerTargetType type;
    public ItemSlot curItemSlot;
    Coroutine inventoryTabHoveringCoroutine;
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(type == PointerTargetType.itemSlot){
            curItemSlot = InventoryManager.instance.itemSlotScripts[eventData.pointerEnter.transform.GetSiblingIndex()].itemSlot;
            //Debug.Log(eventData.pointerEnter.transform.GetSiblingIndex());
            if(curItemSlot.itemSlotBtn.interactable)
                curItemSlot.itemDescriptionWindow.SetActive(true);
        }
        else if(type == PointerTargetType.inventoryTab){
            if(InventoryManager.instance.itemIsMoving && !InventoryManager.instance.inventoryTabHovering){
                InventoryManager.instance.inventoryTabHovering = true;
                inventoryTabHoveringCoroutine = StartCoroutine(InventoryManager.instance.InventoryTabHoveringCoroutine());
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(type == PointerTargetType.itemSlot){
            curItemSlot.itemDescriptionWindow.SetActive(false);
        }
        else if(type == PointerTargetType.inventoryTab){
            if(InventoryManager.instance.inventoryTabHovering){
                InventoryManager.instance.inventoryTabHovering = false;
                StopCoroutine(inventoryTabHoveringCoroutine);
                //inventoryTabHoveringCoroutine = StartCoroutine(InventoryManager.instance.InventoryTabHoveringCoroutine());
            }
        }
    }

}
