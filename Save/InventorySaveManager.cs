using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace InventorySystem.SaveSystem
{
    public static class InventorySaveManager
    {
        public static void SavePlayerInventory(PlayerInventoryData data, string saveName)
        {
            InitializeSaveFolder(saveName);

            File.WriteAllText(Application.persistentDataPath + "/Saves/" + saveName + "/PlayerInventory.json", JsonUtility.ToJson(data, true));
        }

        private static void InitializeSaveFolder(string saveName)
        {
            if (Directory.Exists(Application.persistentDataPath + "/Saves") == false)
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/Saves");
            }

            if (Directory.Exists(Application.persistentDataPath + "/Saves/" + saveName) == false)
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/Saves/" + saveName);
            }
        }
    }

    [Serializable]
    public class PlayerInventoryData
    {
        public GridData[] inventoryGrids;

        public PlayerInventoryData(GridData[] inventoryGrids)
        {
            this.inventoryGrids = inventoryGrids;
        }
    }
}