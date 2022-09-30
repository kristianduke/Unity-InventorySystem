using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem
{
    public static class Utilities
    {
        public static Vector2Int[] GetSlotsFromArea(Vector2Int topLeft, Vector2Int bottomRight)
        {
            List<Vector2Int> selectedSlots = new List<Vector2Int>();
            for (int y = topLeft.y; y <= bottomRight.y; y++)
            {
                for (int x = topLeft.x; x <= bottomRight.x; x++)
                {
                    selectedSlots.Add(new Vector2Int(x, y));
                }
            }

            return selectedSlots.ToArray();
        }
    }

}