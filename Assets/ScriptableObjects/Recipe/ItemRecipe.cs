using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemRecipe", menuName = "Scriptable Objects/ItemRecipe")]
public class ItemRecipe : ScriptableObject
{
    public int requiredItemsCount = 0;
    [Header("Ingredients")]
    public List<Item> requiredItems = new List<Item>();

    [Header("Product")]
    public GameObject returnPrefab;
}
