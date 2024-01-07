using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public enum ItemType
{
    CannotCombinate,
    Ingredient
}


[CreateAssetMenu(fileName = "Item Data", menuName = "Scriptable Object/Item", order = int.MaxValue)]
public class Item : ScriptableObject
{
    [Header("Item Settings")]
    [SerializeField] private int itemID;
    [SerializeField] private ItemType itemType;
    [SerializeField] private Sprite itemImage;
    [SerializeField] private GameObject itemPrefab;
}
