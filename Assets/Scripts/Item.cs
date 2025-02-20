using UnityEngine;

[System.Serializable]
public class Item {
    public int id;
    public string itemName;
    public int quantity;

    public Item(int id, string itemName, int quantity) {
        this.id = id;
        this.itemName = itemName;
        this.quantity = quantity;
    }
}
