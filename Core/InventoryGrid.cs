using System;
using System.Collections.Generic;
using System.Linq;
using InventorySystem.Items;
using UnityEngine;

namespace InventorySystem
{
    public class InventoryGrid : MonoBehaviour
    {
        // --- The inventory grid is responsible for managing inventory slots, and any items contained in those slots --- //
    
        #region VARIABLES

        public Vector2Int gridSize;
        public Dictionary<Vector2Int, InventorySlot> slots = new Dictionary<Vector2Int, InventorySlot>(); //Stores all references to items in the grid.
        public List<InventoryItem> items = new List<InventoryItem>();

        #endregion

        #region MONOBEHAVIOUR

        private void Awake()
        {
            GenerateGrid();
        }

        #endregion

        #region METHODS

        public void GenerateGrid()
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                for (int x = 0; x < gridSize.x; x++)
                {
                    InventorySlot slot = new InventorySlot();
                    slots.Add(new Vector2Int(x, y), slot);
                }
            }
        }

        public bool AddItem(InventoryItem invItem, Vector2Int position)
        {
            Vector2Int itemSize = invItem.baseItem.size;
            Vector2Int[] selectedSlots = null;

            selectedSlots = SelectSlots(position, itemSize);

            if (selectedSlots == null) return false;

            foreach (Vector2Int slot in selectedSlots) //Update Slot Vacancy.
            {
                slots[slot].vacant = false;
            }

            invItem.position = position;
            items.Add(invItem);

            return true;
        }

        public bool AutoAddItem(InventoryItem invItem)
        {
            Vector2Int itemSize = invItem.baseItem.size;
            Vector2Int[] selectedSlots = null;
            
            foreach (Vector2Int slot in slots.Keys)
            {
                selectedSlots = SelectSlots(slot, itemSize);
                if(selectedSlots != null) break;
            }

            if (selectedSlots == null) return false;
            
            foreach (Vector2Int slot in selectedSlots) //Add item reference to all required slots.
            {
                slots[slot].vacant = false;
            }
            
            invItem.position = selectedSlots[0];
            items.Add(invItem);

            return true;
        }

        public bool RemoveItem(InventoryItem inventoryItem)
        {
            if (!items.Contains(inventoryItem)) return false;
            items.Remove(inventoryItem);
            
            Vector2Int[] itemSlots = Utilities.GetSlotsFromArea(inventoryItem.position,
                inventoryItem.position + inventoryItem.baseItem.size - Vector2Int.one);

            foreach (Vector2Int slot in itemSlots)
            {
                slots[slot].vacant = true;
            }

            return true;
        }

        public Vector2Int[] SelectSlots(Vector2Int position, Vector2Int size)
        {
            List<Vector2Int> slotList = new List<Vector2Int>();

            if (position.x < 0) return null;
            if (position.y < 0) return null;
            if (position.x + size.x > gridSize.x) return null; //Check Within X Boundaries
            if (position.y + size.y > gridSize.y) return null; //Check Within Y Boundaries

            for (int y = position.y; y < position.y + size.y; y++)
            {
                for (int x = position.x; x < position.x + size.x; x++)
                {
                    if (slots[new Vector2Int(x, y)].vacant == false) return null; //Check Each Slot is Unoccupied
                    
                    slotList.Add(new Vector2Int(x, y));
                }
            }

            return slotList.ToArray();
        }

        #endregion

        #region DATA

        public static string GridToJson(InventoryGrid grid, bool prettyPrint = false)
        {
            GridData gridData = new GridData(grid.gridSize, grid.items.ToArray());
            string jsonData = JsonUtility.ToJson(gridData, prettyPrint);

            return jsonData;
        }

        #endregion
    }

    [Serializable]
    public class GridData
    {
        public Vector2Int size;
        public InventoryItem[] items;

        public GridData(Vector2Int size, InventoryItem[] items)
        {
            this.size = size;
            this.items = items;
        }
    }
}
