using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IOTU
{
    [Serializable]
    public class StoredItem
    {
        public ItemDefinition Details;
        public ItemVisual RootVisual;

    }
    /// <summary>
    /// The InventoryManager class manages the inventory system.
    /// It handles events between the model (InventorySO) and view/UI (InventoryEvents).
    /// </summary>
    public class InventoryManager : MonoBehaviour
    {
        InventorySO m_InventorySO; // Reference to the InventorySO scriptable object
        
        // Event subscriptions
        private void OnEnable()
        {
            // Listen for events from the View/UI
            InventoryEvents.ItemRemove += InventoryEvents_ItemRemove;

            // Listen for events from the Model
            InventoryEvents.ModelAddToList += InventoryEvents_ModelAddToList;
        }

        // Event unsubscriptions
        private void OnDisable()
        {
            InventoryEvents.ItemRemove -= InventoryEvents_ItemRemove;
            InventoryEvents.ModelAddToList -= InventoryEvents_ModelAddToList;
        }

        private void Start()
        {
            Initialize();
        }

        // Initialize method to set defaults and notify the View/UI
        private void Initialize()
        {
            // Load the InventorySO scriptable object from resources
            m_InventorySO = Resources.Load<InventorySO>("Inventory/InventoryList");

            // Retrieve stored items and inventory dimensions from InventorySO
            List<StoredItem> storedItems = m_InventorySO.list;
            Dimensions inventoryDimensions = m_InventorySO.dimensions;

            // Notify the View/UI of default values from the Model (InventorySO)
            InventoryEvents.InventorySet?.Invoke(storedItems);
            InventoryEvents.DimensionSet?.Invoke(inventoryDimensions);
        }

        // View event handler for removing an item from the inventory
        public void InventoryEvents_ItemRemove(StoredItem item)
        {
            InventoryEvents.RemoveFromList?.Invoke(item);
        }

        // Model event handler for adding an item to the inventory list (response if Model data externally modified)
        public void InventoryEvents_ModelAddToList(StoredItem item)
        {
            InventoryEvents.AddToList?.Invoke(item);
        }
    }
}
