using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using InventorySystem.Items;
using InventorySystem.SaveSystem;
using InventorySystem.World;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace InventorySystem.UI
{
    public class InventoryUIManager : MonoBehaviour
    {
        // --- The Inventory UI Manager's Primary Function is to let the player have multi-grid inventories and Transfers. --- //
        // - This class takes ownership of an item from a grid once it's been attached to the cursor. - //
        // - It also deals with Interactions between the system and the world environment, such as dropping items - //

        #region VARIABLES

        public static InventoryUIManager Instance;
        
        public Item[] items;
        
        // - Events -
        public event Action<Item> UseItemEvent; //Invoked when item is used.

        // - Inventory UI Objects -
        public List<InventoryUIGrid> playerInventories = new List<InventoryUIGrid>(); //All "Player" Inventories.
        private HeldItemData heldItem = null; //Item attached to Mouse Cursor
        private InventoryUIContextMenu currentContextMenu; //Right Click Context Menu Instance.
        [SerializeField] private Transform heldItemContainer;
        
        [SerializeField] private WorldItemPlayerManager worldItemHandler;

        #endregion

        #region MONOBEHAVIOUR

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            worldItemHandler = WorldItemPlayerManager.Instance;
            
            PickupItem(new InventoryItem(items[Random.Range(0, items.Length)]));
            PickupItem(new InventoryItem(items[Random.Range(0, items.Length)]));
            PickupItem(new InventoryItem(items[Random.Range(0, items.Length)]));
        }

        private void Update()
        {
            if (heldItem != null)
            {
                heldItem.UIItem.transform.position = Input.mousePosition + heldItem.MouseOffset;
            }
        }
        

        #endregion

        #region METHODS

        public void SlotClick(InventoryUISlot slot)
        {
            if (heldItem == null && slot.containedItem != null)
            {
                GrabItem(slot, slot.containedItem);
                return;
            }

            if (heldItem != null)
            {
                PlaceItem(slot);
            }
        }

        public void CreateContextMenu(InventoryUISlot slot, Vector2 clickPos)
        {
            if (currentContextMenu != null)
            {
                Destroy(currentContextMenu.gameObject);
            }
            
            if (slot.containedItem == null) return;

            Item targetItem = slot.containedItem.item.baseItem;

            GameObject contextMenuObj = Instantiate(slot.uiGrid.style.ctxMenuObj, slot.transform.parent);

            // Rebuild Layout to Correctly Set Menu Position
            LayoutRebuilder.ForceRebuildLayoutImmediate(contextMenuObj.GetComponent<RectTransform>());

            contextMenuObj.GetComponent<RectTransform>().pivot = new Vector2(0, 1);
            contextMenuObj.transform.position = Input.mousePosition;

            InventoryUIContextMenu contextMenu = contextMenuObj.GetComponent<InventoryUIContextMenu>();

            currentContextMenu = contextMenu;

            if (targetItem.GetType() == typeof(Consumable)) // Consumable Context Button
            {
                Consumable consumableItem = (Consumable)targetItem;

                string btnName = consumableItem.consumableType switch //Set Consume Button Name Based on Item Type
                {
                    Consumable.ConsumableType.Drink => "Drink",
                    Consumable.ConsumableType.Food => "Eat",
                    Consumable.ConsumableType.Medical => "Use",
                    _ => "Consume"
                };

                //Create Consumable Context Button
                contextMenu.GenerateContextButton(btnName).ButtonClicked += ctx =>
                {
                    UseItemEvent?.Invoke(targetItem);
                };
            }

            //Drop Context Btn
            contextMenu.GenerateContextButton("Discard").ButtonClicked += ctx =>
            {
                DropItem(slot.containedItem);
            };
        }

        private void GrabItem(InventoryUISlot invSlot, InventoryUIItem invItem) //Attaches the Item to the Mouse
        {
            if (heldItem != null) return;

            // --- Item Offsets ---
            Vector2Int heldItemGridOffset = invSlot.slotPosition - invSlot.containedItem.item.position;
            Vector3 heldItemMouseOffset = invItem.transform.position - Input.mousePosition;
            
            heldItem = new HeldItemData(invItem, heldItemMouseOffset, heldItemGridOffset, invSlot);

            if (heldItemContainer != null)
            {
                invItem.transform.SetParent(heldItemContainer);
            }
            
            foreach (Vector2Int slot in invItem.slotsTaken) //Removing Item Reference from Slots
            {
                invItem.uiGrid.slotComponents[slot].GetComponent<InventoryUISlot>().containedItem = null;
            }
            
            invItem.uiGrid.inventoryGrid.RemoveItem(invItem.item); //Remove Item from Old Grid.
        }
        
        private void PlaceItem(InventoryUISlot slot) //Places Attached Item to Clicked Slot
        {
            Vector2Int[] selectedSlots = slot.uiGrid.inventoryGrid.SelectSlots(slot.slotPosition - heldItem.GridOffset, heldItem.UIItem.item.baseItem.size);
            
            if (selectedSlots == null) return;
            
            InventoryUIItem uiItem = heldItem.UIItem;
            heldItem = null;
            
            uiItem.transform.SetParent(slot.transform.parent);
            uiItem.uiGrid = slot.uiGrid; //Set Item Parent Grid to New Grid
            uiItem.uiGrid.inventoryGrid.AddItem(uiItem.item, selectedSlots[0]); //Add Item to New Grid
            uiItem.item.position = selectedSlots[0]; // Updating Item Position
            uiItem.slotsTaken = selectedSlots;

            uiItem.GetComponent<RectTransform>().anchoredPosition = slot.uiGrid.GetAveragePosition(selectedSlots); //Update Item Position

            foreach (Vector2Int slotPos in selectedSlots) //Add Item Reference to new Slots
            {
                slot.uiGrid.slotComponents[slotPos].containedItem = uiItem;
            }
        }

        private void DropItem(InventoryUIItem invItem)
        {
            invItem.uiGrid.inventoryGrid.RemoveItem(invItem.item);
            invItem.uiGrid.UpdateGrid();

            if (worldItemHandler)
            {
                worldItemHandler.DropWorldItem(invItem.item);
            }
            
            Destroy(invItem);
        }

        public bool PickupItem(InventoryItem inventoryItem)
        {
            foreach (InventoryUIGrid playerInventory in playerInventories)
            {
                playerInventory.UpdateGrid();
                if (playerInventory.inventoryGrid.AutoAddItem(inventoryItem))
                {
                    playerInventory.GenerateItemObjects();
                    return true;
                }
            }

            return false;
        }

        public void AttachedItemToOriginalSlot()
        {
            if (heldItem == null) return;
            
            PlaceItem(heldItem.OriginalSlot);
        }
        
        #endregion
    }

    internal class HeldItemData // Used to store data related to the current held item (item attached to mouse cursor)
    {
        public readonly InventoryUIItem UIItem;
        
        // Offsets
        public readonly Vector3 MouseOffset;
        public readonly Vector2Int GridOffset;
        public readonly InventoryUISlot OriginalSlot;

        public HeldItemData(InventoryUIItem uiItem, Vector3 mouseOffset, Vector2Int gridOffset, InventoryUISlot originalSlot)
        {
            UIItem = uiItem;
            MouseOffset = mouseOffset;
            GridOffset = gridOffset;
            OriginalSlot = originalSlot;
        }
    }

}