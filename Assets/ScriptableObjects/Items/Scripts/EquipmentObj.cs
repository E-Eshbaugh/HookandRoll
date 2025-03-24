using UnityEngine;

[CreateAssetMenu(fileName = "EquipmentObj", menuName = "InventorySystem/Items/EquipmentObj")]
public class EquipmentObj : ItemObj
{
    public float quality;
    public float durability;
    public void Awake()
    {
        type = ItemType.Equipment;
    }
}
