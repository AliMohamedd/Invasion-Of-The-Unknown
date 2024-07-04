using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UIElements;

namespace IOTU
{
    /// <summary>
    /// Public static delegates to manage Inventory (note these are "events" in the conceptual sense
    /// and not the strict C# sense).
    /// </summary>
    public static class InventoryEvents
    {
        #region Grid:
        
        // Presenter --> View:
        public static Action<List<StoredItem>> InventorySet;
        public static Action<Dimensions> DimensionSet;
        public static Action<StoredItem> AddToList;

        // View -> Presenter:
        public static Action<StoredItem> ItemRemove;
        
        // Presenter -> Model: 
        public static Action<StoredItem> RemoveFromList;
        
        // Model -> Presenter:  
        public static Action<StoredItem> ModelAddToList;
        
        #endregion

    }
}
