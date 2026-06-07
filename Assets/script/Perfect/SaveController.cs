using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class SaveController : MonoBehaviour
{
    [Header("玩家和摄像机")]
    [SerializeField] private Transform playerps;
    [SerializeField] private Transform mainCamera;
    [SerializeField] private Transform p1;

    //自动读取存档
    public static bool AutoLoadOnStart = false;
    private static string SavePath => Application.persistentDataPath + "/Checkpoint.json";


    private int  currentCheckpointId = -1;
    private Vector3 currentSpawnPosition;

    public int CurrentCheckpointId => currentCheckpointId;
    public Vector3 CurrentSpawnPosition => currentSpawnPosition;

    [SerializeField] private InventoryManager invMgr;


    public static bool HasSaveFile()
    {
        return File.Exists(SavePath);
    }



    private void Start()
    {
        if (AutoLoadOnStart)
        {
            Load();
            AutoLoadOnStart = false;
        }
    }



    public void Save(Vector3 spawnPosition, int checkpointId)
    {
        currentCheckpointId  = checkpointId;
        currentSpawnPosition = spawnPosition;

        // 背包数据：字典 → List
        List<InventoryEntry> invList = new List<InventoryEntry>();
        if (invMgr != null)
        {
            Dictionary<string, int> dict = invMgr.GetInventoryDict();
            foreach (var kv in dict)
            {
                invList.Add(new InventoryEntry { itemName = kv.Key, count = kv.Value });
            }
        }

        CheckpointData data = new CheckpointData
        {
            checkpointId = checkpointId,
            spawnX       = spawnPosition.x,
            spawnY       = spawnPosition.y,
            spawnZ       = spawnPosition.z,
            Coin         = GameManager.instance != null ? GameManager.instance.CoinCount : 0,
            inventoryItems = invList
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);

        
    }

    public void Load()
    {
        if (!File.Exists(SavePath))
        {
            Debug.LogWarning("没有存档，使用默认初始位置复活");
            // 无存档时重置为默认状态
            currentCheckpointId = -1;
            currentSpawnPosition = p1.position; 
            ApplyRespawnData(); // 抽离通用逻辑，确保状态生效
            return;
        }

        string json = File.ReadAllText(SavePath);
        CheckpointData data = JsonUtility.FromJson<CheckpointData>(json);

        currentCheckpointId = data.checkpointId;
        currentSpawnPosition = new Vector3(data.spawnX, data.spawnY, data.spawnZ);
        ApplyRespawnData();
    }

// 抽离复活数据应用逻辑，统一处理有/无存档的情况
    private void ApplyRespawnData()
    {
        // 恢复金币
        if (GameManager.instance != null)
        {
            GameManager.instance.CoinZero();
            // 无存档时金币为0，有存档时从data读取
            if (currentCheckpointId != -1) 
            {
                // 仅当有有效存档时恢复金币（避免无存档时覆盖）
                string json = File.ReadAllText(SavePath);
                CheckpointData data = JsonUtility.FromJson<CheckpointData>(json);
                GameManager.instance.CoinAdd(data.Coin);
            }
        }

        // 传送玩家和相机
        if (playerps != null)
        {
            Vector3 oldPos = playerps.position;
            playerps.position = currentSpawnPosition;
            if (mainCamera != null)
            {
                mainCamera.position += currentSpawnPosition - oldPos;
            }
        }

        // 无存档时清空背包
        if (currentCheckpointId == -1)
        {
            if (invMgr != null)
            {
                invMgr.RestoreInventory(new Dictionary<string, int>());
            }
        }
        else
        {
            // 有存档时恢复背包
            string json = File.ReadAllText(SavePath);
            CheckpointData data = JsonUtility.FromJson<CheckpointData>(json);
            if (data.inventoryItems != null && data.inventoryItems.Count > 0)
            {
                Dictionary<string, int> restoreDict = new Dictionary<string, int>();
                foreach (var entry in data.inventoryItems)
                {
                    restoreDict[entry.itemName] = entry.count;
                }
                if (invMgr != null)
                {
                    invMgr.RestoreInventory(restoreDict);
                }
            }
        }
    }

    //物品字典转为结构体
    [System.Serializable]
    public struct InventoryEntry
    {
        public string itemName;
        public int count;
    }

    [System.Serializable]
    public class CheckpointData
    {
        public int checkpointId;
        public float spawnX;
        public float spawnY;
        public float spawnZ;
        public int Coin;
        public List<InventoryEntry> inventoryItems;
    }

}