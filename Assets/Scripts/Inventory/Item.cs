// InventoryItem.cs
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Item : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("UI")]
    // public Image image;
    // public Canvas canvas;

    [HideInInspector] public Transform parentAfterDrag;
    // drag and drop
    public void OnBeginDrag(PointerEventData eventData) {
        // image.raycastTarget = false;
        Debug.Log("Started Dragging");
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        // transform.SetParent(canvas.transform);
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData) {
        // Debug.Log("Dragging");
        // Get the current position and print x and y coordinates to Debug log
        Vector3 position = transform.position;
        Debug.Log("Position: x = " + position.x + ", y = " + position.y);
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData) {
        // image.raycastTarget = true;
        Debug.Log("Stopped Dragging");
        transform.SetParent(parentAfterDrag);
    }
}
