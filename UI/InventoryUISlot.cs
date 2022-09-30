using InventorySystem.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace InventorySystem.UI
{
    public class InventoryUISlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        #region VARIABLES

        public InventoryUIGrid uiGrid;
        public Vector2Int slotPosition;
        public InventoryUIItem containedItem;

        #endregion

        #region UIEVENTS

        public void OnPointerEnter(PointerEventData eventData)
        {
            
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                InventoryUIManager.Instance.SlotClick(this);
            } else if (eventData.button == PointerEventData.InputButton.Right)
            {
                InventoryUIManager.Instance.CreateContextMenu(this, eventData.position);
            }
        }

        #endregion
    }

}