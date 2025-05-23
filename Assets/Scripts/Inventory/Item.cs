// InventoryItem.cs
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Item : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // public Image image;
    // public Canvas canvas;

    public int quantity;
    public int ItemID;

    public int Value = 0;

    public enum ItemType
    {
        None,
        Generic, 
        Ingredient,
        Cooked,
        Rice,
        Sushi,
        Fish,
    }
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

    public ItemType type;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = Resources.Load<Sprite>("Box"); // You'll need a box sprite in Resources folder
            spriteRenderer.sortingOrder = 1;
        }
    }

    // Set the item's position on the conveyor
    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public Item() {
        type = ItemType.Generic;
    }

    public string getType() {
        return type.ToString();
    }
}