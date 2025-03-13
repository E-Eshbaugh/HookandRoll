using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData) {
        Item item = eventData.pointerDrag.GetComponent<Item>();
        item.parentAfterDrag = transform;
    }
}