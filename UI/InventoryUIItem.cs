using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace InventorySystem.UI
{
    public class InventoryUIItem : MonoBehaviour
    {
        #region VARIABLES

        public InventoryItem item;
        public InventoryUIGrid uiGrid;
        public Vector2Int[] slotsTaken;

        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI itemNameText;

        #endregion

        #region MONOBEHAVIOUR

        private void Start()
        {
            if (icon != null)
            {
                icon.sprite = item.baseItem.icon;
            }

            if (itemNameText != null)
            {
                itemNameText.text = item.baseItem.name;
            }
        }

        #endregion
    }

}