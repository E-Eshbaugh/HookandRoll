using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "InventorySystem/Inventory")]
public class InventoryObj : ScriptableObject
{
    public List<InventorySpace> Container = new List<InventorySpace>();
    public void AddItem(ItemObj _item, int _amount) {
        bool hasItem = false;
        for (int i=0; i < Container.Count; i++) {
            if (Container[i].item == _item) {
                Container[i].AddAmount(_amount);
                hasItem = true;
                break;
            }
        }
        if (!hasItem){
            Container.Add(new InventorySpace(_item, _amount));
        }
    }
}

[System.Serializable]
public class InventorySpace {

    public ItemObj item;
    public int amount;
    public InventorySpace(ItemObj _item, int _amount) {
        item = _item;
        amount = _amount;
    }

    public void AddAmount(int value) {
        amount += value;
    }
}