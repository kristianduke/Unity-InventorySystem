using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace InventorySystem.UI
{
    public class InventoryUIContextButton : MonoBehaviour, IPointerClickHandler
    {
        #region VARIABLES

        public event Action<bool> ButtonClicked;

        public TextMeshProUGUI textObj;

        #endregion

        #region UIEVENTS

        public void OnPointerClick(PointerEventData eventData)
        {
            ButtonClicked?.Invoke(true);
        }

        #endregion
    }

}