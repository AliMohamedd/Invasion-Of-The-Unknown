using System;
using UnityEngine;

/// <summary>
/// ScriptableObject representing an item definition.
/// </summary>
[CreateAssetMenu(fileName ="New Item", menuName ="IOTU/Inventory/Item", order = 1)]
public class ItemDefinition : ScriptableObject
{
    public string ID = Guid.NewGuid().ToString(); // Unique identifier for the item
    public string Name; // Name of the item
    public string Description; // Description of the item
    public Sprite Icon; // Icon representing the item in UI
    public Dimensions SlotDimension; // Dimensions of the item's slot in inventory
}

/// <summary>
/// Struct representing dimensions (height and width).
/// </summary>
[Serializable]
public struct Dimensions
{
    public int Height; // Height dimension
    public int Width; // Width dimension
}
