using System;
using InventorySystem.Items;
using UnityEngine;

namespace InventorySystem
{
    [Serializable]
    public class InventoryItem : ISerializationCallbackReceiver
    {
        // --- Inventory Items are items used by the inventory grid to save item details inside a grid. --- //

        #region VARIABLES

        public Item baseItem;

        public Vector2Int position; //Top Left Slot of Item in Inventory
        
        [NonSerialized]
        public AdditionalItemData itemData;
        public string additionalItemData;
        public string itemDataType;

        #endregion

        #region CONSTRUCTOR

        public InventoryItem(Item baseItem, AdditionalItemData itemData = null)
        {
            this.baseItem = baseItem;
            this.itemData = itemData;
        }

        #endregion

        #region SERIALISATIONCALLBACKS

        public void OnBeforeSerialize()
        {
            if (itemData == null) return;
            additionalItemData = JsonUtility.ToJson(itemData);
            itemDataType = itemData.GetType().ToString();
        }

        public void OnAfterDeserialize()
        {
            Type type = Type.GetType(itemDataType);
            itemData = (AdditionalItemData)JsonUtility.FromJson(additionalItemData, type);
        }

        #endregion

    }
    
    [Serializable]
    public class AdditionalItemData
    {
        
    }

    [Serializable]
    public class MagazineItemData : AdditionalItemData
    {
        public int ammoCount = 100;
    }
}