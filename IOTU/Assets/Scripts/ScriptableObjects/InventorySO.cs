using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IOTU
{
    /// <summary>
    /// The InventorySO class in the IOTU namespace is a ScriptableObject that holds a list of stored items
    /// and inventory dimensions. It subscribes to InventoryEvents to add or remove items from the inventory list.
    /// </summary>
    [CreateAssetMenu(fileName = "InventoryList", menuName = "IOTU/Inventory/List", order = 2)]
    public class InventorySO : DescriptionSO
    {
        // Default ScriptableObject data
        public List<StoredItem> m_StoredItems;
        public Dimensions m_InventoryDimensions;

        // Properties
        public List<StoredItem> list { get => m_StoredItems; set => m_StoredItems = value; }
        public Dimensions dimensions { get => m_InventoryDimensions; set => m_InventoryDimensions = value; }
       
        // Event subscriptions
        private void OnEnable()
        {
            InventoryEvents.AddToList += InventoryEvents_AddToList;
            InventoryEvents.RemoveFromList += InventoryEvents_RemoveFromList;
        }

        // Event unsubscriptions
        private void OnDisable()
        {
            InventoryEvents.AddToList -= InventoryEvents_AddToList;
            InventoryEvents.RemoveFromList -= InventoryEvents_RemoveFromList;
        }
        
        // Method to handle adding an item to the inventory list
        private void InventoryEvents_AddToList(StoredItem item)
        {
            list.Add(item);
        }

        // Method to handle removing an item from the inventory list
        private void InventoryEvents_RemoveFromList(StoredItem item)
        {
            list.RemoveAt(list.IndexOf(item));
        }
    }
}
