using System;
using System.Collections;
using System.Collections.Generic;
using InventorySystem.UI;
using TMPro;
using UnityEngine;

namespace InventorySystem.World
{
    public class WorldItemPlayerManager : MonoBehaviour
    {
        #region VARIABLES

        public static WorldItemPlayerManager Instance;

        [Header("Pickup Item Variables")]
        public float pickupDistance = 2f;

        [SerializeField] private GameObject pickupTextUI;
        
        [Header("Drop Item Variables")]
        [SerializeField] private float spawnDistance;
    
        [Header("Components")]
        [SerializeField] private Transform dropItemTarget;
        [SerializeField] private Camera playerCamera;
        [SerializeField] private InventoryUIManager inventoryUIManager;
        
        private PlayerInputActions playerInput;
        private WorldItem hoveredItem;

        #endregion

        #region MONOBEHAVIOUR

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            playerInput = InputManager.Instance.PlayerInputActions;

            playerInput.Gameplay.PickupItem.performed += ctx => PickupHoveredItem();
        }

        private void Update()
        {
            GetHoveredWorldItem();

            UpdateUI();
        }

        #endregion

        #region METHODS

        public void DropWorldItem(InventoryItem item)
        {
            GameObject worldObj = Instantiate(item.baseItem.worldObject);
            worldObj.transform.position = dropItemTarget.position + (dropItemTarget.forward * spawnDistance);
            worldObj.transform.rotation = dropItemTarget.rotation;
            
            if (!worldObj.GetComponent<WorldItem>())
            {
                WorldItem objectItem = worldObj.AddComponent<WorldItem>();
                objectItem.item = item;
            }
        }

        private void GetHoveredWorldItem()
        {
            RaycastHit hit;

            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, pickupDistance))
            {
                Transform objectHit = hit.transform;

                hoveredItem = objectHit.GetComponent<WorldItem>() ? objectHit.GetComponent<WorldItem>() : null;

                if (hoveredItem != null)
                {
                    hoveredItem.OnHovered();
                }
            }
            else
            {
                hoveredItem = null;
            }
        }

        private void PickupHoveredItem()
        {
            if (!hoveredItem) return;

            if (inventoryUIManager.PickupItem(hoveredItem.item))
            {
                Destroy(hoveredItem.gameObject);
            }
        }

        private void UpdateUI()
        {
            if (!hoveredItem)
            {
                pickupTextUI.gameObject.SetActive(false);
                return;
            }

            if (!pickupTextUI) return;

            pickupTextUI.gameObject.SetActive(true);
        }

        #endregion
    }

}