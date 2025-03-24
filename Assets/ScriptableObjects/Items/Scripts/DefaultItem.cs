using UnityEngine;

[CreateAssetMenu(fileName = "DefaultItem", menuName = "InventorySystem/Items/DefaultItem")]
public class DefaultItem : ItemObj
{
    public void Awake()
    {
        type = ItemType.Default;
    } 
}
