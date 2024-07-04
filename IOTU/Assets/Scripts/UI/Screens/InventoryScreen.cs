using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace IOTU
{
    /// <summary>
    /// Represents the inventory screen, managing items and UI interactions.
    /// </summary>
    public class InventoryScreen : UIScreen
    {
        public static InventoryScreen Instance;

        private bool m_IsInventoryReady;
        public List<StoredItem> StoredItems;
        public Dimensions InventoryDimensions;
        public static Dimensions SlotDimension { get; private set; }

        private static Label m_Name;        // Label to display item name
        private static string name;         // Static string to hold item name (used for removal)
        private static Label m_Description; // Label to display item description
        private Button m_DropButton;        // Button to drop items

        VisualElement m_InventoryGrid;      // VisualElement to hold the inventory grid
        private VisualElement m_Telegraph;  // VisualElement to represent the telegraph effect

        /// <summary>
        /// Constructor to initialize the inventory screen with its parent VisualElement.
        /// </summary>
        /// <param name="rootElement">The root VisualElement to attach UI elements to.</param>
        public InventoryScreen(VisualElement rootElement) : base(rootElement)
        {
            SubscribeToEvents();
            SetVisualElements();
            RegisterCallbacks();
            LoadInventory();

            m_IsTransparent = true;
        }

        // Disables the screen and unsubscribes from events.
        public override void Disable()
        {
            base.Disable();
            UnsubscribeFromEvents();
        }

        // Subscribes to inventory events.
        private void SubscribeToEvents()
        {
            InventoryEvents.InventorySet += ListHandler;
            InventoryEvents.DimensionSet += DimensionHandler;
        }

        // Unsubscribes from inventory events.
        private void UnsubscribeFromEvents()
        {
            InventoryEvents.InventorySet -= ListHandler;
            InventoryEvents.DimensionSet -= DimensionHandler;
        }

        // Sets references to UI elements by querying the root VisualElement.
        private async void SetVisualElements()
        {
            m_Name = m_RootElement.Q<Label>("friendly_name");
            m_Description = m_RootElement.Q<Label>("description");

            m_InventoryGrid = m_RootElement.Q<VisualElement>("grid");
            m_DropButton = m_RootElement.Q<Button>("btn_drop");

            // Wait for the end of the frame to ensure UI elements are properly initialized
            await UniTask.WaitForEndOfFrame();

            ConfigureInventoryTelegraph();
            ConfigureSlotDimensions();
            m_IsInventoryReady = true;
        }

        // Registers callbacks for UI button clicks.
        private void RegisterCallbacks()
        {
            m_EventRegistry.RegisterCallback<ClickEvent>(m_DropButton, evt => Remove());
        }

        /// <summary>
        /// Event handler for receiving the list of stored items.
        /// </summary>
        /// <param name="list">The list of stored items.</param>
        private void ListHandler(List<StoredItem> list)
        {
            StoredItems = list;
        }

        /// <summary>
        /// Event handler for receiving the inventory dimensions.
        /// </summary>
        /// <param name="dim">The dimensions of the inventory grid.</param>
        private void DimensionHandler(Dimensions dim)
        {
            InventoryDimensions = dim;
        }

        // Removes an item from the inventory.
        private void Remove()
        {
            if (StoredItems.Count > 0)
            {
                StoredItem itemToRemove = StoredItems.FirstOrDefault(x => x.Details.Name == name);
                RemoveItemFromInventoryGrid(itemToRemove.RootVisual);
                StoredItems.Remove(itemToRemove);
            }
        }

        // Creates a visual element for the telegraph effect (yellow box).
        private void ConfigureInventoryTelegraph()
        {
            m_Telegraph = new VisualElement
            {
                name = "Telegraph",
                style =
                {
                    position = Position.Absolute,
                    visibility = Visibility.Hidden
                }
            };

            m_Telegraph.AddToClassList("slot-icon-highlighted");
            AddItemToInventoryGrid(m_Telegraph);
        }
        
         public static void UpdateItemDetails(ItemDefinition item)
        {
            name = item.Name;
            m_Name.text = item.Name;
            m_Description.text = item.Description;
            
        }
        
        // Sets the slot dimensions based on the size of the first slot in the inventory grid.
        private void ConfigureSlotDimensions()
        {
            VisualElement firstSlot = m_InventoryGrid.Children().First();

            SlotDimension = new Dimensions
            {
                Width = Mathf.RoundToInt(150),
                Height = Mathf.RoundToInt(150)
            };
        }

        /// <summary>
        /// Adds an item visual element to the inventory grid.
        /// </summary>
        /// <param name="item">The item visual element to add.</param>
        private void AddItemToInventoryGrid(VisualElement item) => m_InventoryGrid.Add(item);

        /// <summary>
        /// Removes an item visual element from the inventory grid.
        /// </summary>
        /// <param name="item">The item visual element to remove.</param>
        private void RemoveItemFromInventoryGrid(VisualElement item) => m_InventoryGrid.Remove(item);

        /// <summary>
        /// Sets the position of an item visual element within the inventory grid.
        /// </summary>
        /// <param name="element">The item visual element.</param>
        /// <param name="vector">The position vector.</param>
        private static void SetItemPosition(VisualElement element, Vector2 vector)
        {
            element.style.left = vector.x;
            element.style.top = vector.y;
        }

        /// <summary>
        /// Attempts to find a position in the inventory grid for a new item.
        /// </summary>
        /// <param name="newItem">The new item visual element.</param>
        /// <returns>True if a position was found; false otherwise.</returns>
        private async Task<bool> GetPositionForItem(VisualElement newItem)
        {
            for (int y = 0; y < InventoryDimensions.Height; y++)
            {
                for (int x = 0; x < InventoryDimensions.Width; x++)
                {
                    SetItemPosition(newItem, new Vector2(SlotDimension.Width * x, SlotDimension.Height * y));

                    StoredItem overlappingItem = StoredItems.FirstOrDefault(s => s.RootVisual != null && s.RootVisual.layout.Overlaps(newItem.layout));

                    if (overlappingItem == null)
                    {
                        return true; // Found a position for the item
                    }
                }
            }

            return false; // No space found for the item
        }

        /// <summary>
        /// Configures an inventory item with its visual representation.
        /// </summary>
        /// <param name="item">The stored item data.</param>
        /// <param name="visual">The visual representation of the item.</param>
        private static void ConfigureInventoryItem(StoredItem item, ItemVisual visual)
        {
            item.RootVisual = visual;
            visual.style.visibility = Visibility.Visible;
        }

        // Loads the inventory items into the inventory grid.
        private async void LoadInventory()
        {
            await UniTask.WaitUntil(() => m_IsInventoryReady);

            foreach (StoredItem loadedItem in StoredItems)
            {
                ItemVisual inventoryItemVisual = new ItemVisual(loadedItem.Details);

                AddItemToInventoryGrid(inventoryItemVisual);

                bool inventoryHasSpace = await GetPositionForItem(inventoryItemVisual);

                if (!inventoryHasSpace)
                {
                    Debug.Log("No space - Cannot pick up the item");
                    RemoveItemFromInventoryGrid(inventoryItemVisual);
                    continue;
                }

                ConfigureInventoryItem(loadedItem, inventoryItemVisual);
            }
        }
    }
}
