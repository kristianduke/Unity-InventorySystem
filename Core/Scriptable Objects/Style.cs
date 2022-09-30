using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem.UI
{
    [CreateAssetMenu(fileName = "New Style", menuName = "Inventory/Style")]
    public class Style : ScriptableObject
    {
        #region VARIABLES

        [Header("Grid Objects")]
        public GameObject slotObj;
        public GameObject itemObj;

        [Header("Context Menu")] 
        public GameObject ctxMenuObj;
        public GameObject ctxBtnObj;

        #endregion
    }

}