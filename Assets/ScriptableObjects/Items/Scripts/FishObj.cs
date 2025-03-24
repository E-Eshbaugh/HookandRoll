using UnityEngine;

[CreateAssetMenu(fileName = "FishObj", menuName = "InventorySystem/Items/FishObj")]
public class FishObj : ItemObj
{
    public float size;
    public void Awake()
    {
        type = ItemType.Fish;
    }
}
