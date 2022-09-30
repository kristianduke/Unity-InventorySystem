using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem.Items
{
    [CreateAssetMenu(fileName = "New Consumable", menuName = "Inventory/Items/Consumable")]
    public class Consumable : Item
    {
        public enum ConsumableType {Food, Drink, Medical}

        public ConsumableType consumableType;
        public float amount;
    }
}