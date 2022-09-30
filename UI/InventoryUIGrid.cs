using System;
using System.Collections.Generic;
using InventorySystem;
using InventorySystem.Items;
using UnityEngine;
using UnityEngine.UI;


namespace InventorySystem.UI
{
    [RequireComponent(typeof(InventoryGrid))]
    [RequireComponent(typeof(GridLayoutGroup))]
    [RequireComponent(typeof(ContentSizeFitter))]
    public class InventoryUIGrid : MonoBehaviour
    {
        #region VARIABLES
        
        public Style style;

        public Dictionary<Vector2Int, InventoryUISlot> slotComponents = new Dictionary<Vector2Int, InventoryUISlot>();

        // --- Components ---
        public InventoryGrid inventoryGrid;
        private GridLayoutGroup _gridLayout;
        private ContentSizeFitter _sizeFitter;

        #endregion

        #region MONOBEHAVIOUR

        private void Awake()
        {
            Init();
            GenerateSlotObjects();
            GenerateItemObjects();
        }

        #endregion

        #region METHODS

        private void Init()
        {
            inventoryGrid = GetComponent<InventoryGrid>(); //Getting Grid to Generate UI
            _gridLayout = GetComponent<GridLayoutGroup>(); //Getting Grid Layout
            _sizeFitter = GetComponent<ContentSizeFitter>(); //Getting Size Fitter
            
            _gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            _gridLayout.constraintCount = inventoryGrid.gridSize.x;

            _sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            _sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }

        public void GenerateSlotObjects()
        {
            for (int y = 0; y < inventoryGrid.gridSize.y; y++)
            {
                for (int x = 0; x < inventoryGrid.gridSize.x; x++)
                {
                    if (slotComponents.ContainsKey(new Vector2Int(x, y))) continue;
                    // --- Create Slot Object ---
                    GameObject slotObj = Instantiate(style.slotObj, transform);
                    slotObj.name = "Slot [" + x + "," + y + "]"; //Set name to Slot Position
                    
                    InventoryUISlot slotComponent = slotObj.GetComponent<InventoryUISlot>(); //Try Get Slot Object
                    
                    if (slotComponent == null) //Add Slot Component if none exists
                    {
                        slotComponent = slotObj.AddComponent<InventoryUISlot>();
                    }
                    
                    // --- Update Slot Variables ---
                    slotComponent.uiGrid = this;
                    slotComponent.slotPosition = new Vector2Int(x, y);

                    slotComponents.Add(new Vector2Int(x, y), slotComponent);
                }
            }
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
        }

        public void GenerateItemObjects()
        {
            ClearItems();
            foreach (InventoryItem inventoryItem in inventoryGrid.items)
            {
                Vector2Int[] slots = Utilities.GetSlotsFromArea(inventoryItem.position, 
                    inventoryItem.position + inventoryItem.baseItem.size - Vector2Int.one);

                Item item = inventoryItem.baseItem;
                GameObject itemObj = Instantiate(style.itemObj, transform);
                
                // --- Get / Add Item Component ---
                InventoryUIItem itemComponent = itemObj.GetComponent<InventoryUIItem>();
                if (itemComponent == null) //Add Slot Component if none exists
                {
                    itemComponent = itemObj.AddComponent<InventoryUIItem>();
                }

                // --- Set Rect Size to Match Grid Size, Including Spacing. ---
                itemObj.GetComponent<RectTransform>().sizeDelta = new Vector2(
                    _gridLayout.cellSize.x * item.size.x + _gridLayout.spacing.x * (item.size.x - 1),
                    _gridLayout.cellSize.y * item.size.y + _gridLayout.spacing.y * (item.size.y - 1));

                Vector2 sum = Vector2.zero; //Used for Calculating Average Position of Slots.
                foreach(Vector2Int slot in slots)
                {
                    sum += slotComponents[slot].GetComponent<RectTransform>().anchoredPosition; //Calculate Average Position of Slots
                    slotComponents[slot].GetComponent<InventoryUISlot>().containedItem = itemComponent; //Update Slot Contained Item
                }
            
                // --- Set Position to Average Position of Slots. ---
                itemObj.GetComponent<RectTransform>().anchoredPosition = sum / slots.Length;

                // --- Update Slot Variables ---
                itemComponent.item = inventoryItem;
                itemComponent.uiGrid = this;
                itemComponent.slotsTaken = slots;
            }
        }

        public void ClearItems()
        {
            foreach (InventoryUISlot slot in slotComponents.Values)
            {
                if (slot.containedItem == null) continue;
                Destroy(slot.containedItem.gameObject);
                slot.containedItem = null;
            }
        }

        public Vector2 GetAveragePosition(Vector2Int[] slots)
        {
            Vector2 sum = Vector2.zero;
            foreach(Vector2Int slot in slots)
            {
                sum += slotComponents[slot].GetComponent<RectTransform>().anchoredPosition;
            }

            return sum / slots.Length;
        }

        public void UpdateSize(Vector2Int size)
        {
            
        }

        public void UpdateGrid()
        {
            GenerateSlotObjects();
            GenerateItemObjects();
        }

        #endregion
    }
}