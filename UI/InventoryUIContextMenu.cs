using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem.UI
{
    public class InventoryUIContextMenu : MonoBehaviour
    {
        #region VARIABLES

        [SerializeField] private Style style;

        private List<GameObject> ctxMenuButtons = new List<GameObject>();

        #endregion

        #region MONOBEHAVIOUR

        private void Update()
        {
            if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
            {
                Destroy(gameObject);
            }
        }

        #endregion

        #region METHODS

        public InventoryUIContextButton GenerateContextButton(string name)
        {
            GameObject contextButtonObj = Instantiate(style.ctxBtnObj, transform);
            contextButtonObj.transform.name = name;
            InventoryUIContextButton contextButton = contextButtonObj.GetComponent<InventoryUIContextButton>();
            contextButton.textObj.text = name;

            return contextButton;
        }

        #endregion
    }

}