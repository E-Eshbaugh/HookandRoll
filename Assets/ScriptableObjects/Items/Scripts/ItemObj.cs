using UnityEngine;


public enum ItemType {
    Fish,
    Equipment,
    Sushi,
    Default
}
public abstract class ItemObj : ScriptableObject
{

    public string itemName {
        get { return name; }
    }
    public int id;
    public GameObject prefab;
    public ItemType type;

    [TextArea(15,20)]
    public string description;
    
}
