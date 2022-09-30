using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace InventorySystem.Items
{
    [CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Items/Base")]
    public class Item : ScriptableObject
    {
        #region VARIABLES

        public Vector2Int size;
        public Sprite icon;
        public GameObject worldObject;

        #endregion
    }

}