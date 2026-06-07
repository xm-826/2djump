using UnityEngine;

/// <summary>
/// 存档触发器：挂在场景中的存档点 GameObject 上。
/// 每个存档点有独立的编号和重生坐标，玩家走到范围内按 E 即可激活。
///
/// 使用方式：
///   1. 在场景中放置一个空 GameObject，挂上此脚本
///   2. 添加 Collider2D（勾选 IsTrigger）
///   3. 设置 checkpointId（每个存档点用不同数字，如 0, 1, 2...）
///   4. 可选的 spawnPoint —— 不填则用自身位置作为重生点
/// </summary>
public class SaveTrigger : MonoBehaviour
{
    [Header("存档点配置")]
    [Tooltip("存档点编号，同一场景中不要重复")]
    [SerializeField] private int checkpointId = 0;

    [Tooltip("玩家重生位置。留空则使用本 GameObject 的位置作为重生点。")]
    [SerializeField] private Transform spawnPoint;

    [Header("UI 提示")]
    [SerializeField] private GameObject textUI;
    [SerializeField] private Animator ani;

    [SerializeField] private SaveController SaveController;

    // ── 运行时状态 ────────────────────────────────────────────
    private bool canSave = false;   // 玩家是否在触发范围内

    private void Update()
    {
        // 玩家在范围内 + 按下 E → 执行存档
        if (canSave && Input.GetKeyDown(KeyCode.E))
        {
            ExecuteSave();
        }

        if(checkpointId ==5 && canSave)
        {
            ExecuteSave();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canSave = true;
            if (ani != null)
                ani.SetBool("flag", true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canSave = false;
            if (ani != null)
                ani.SetBool("flag", false);
        }
    }

    /// <summary>执行存档：将当前存档点的目标坐标和编号写入 SaveController</summary>
    private void ExecuteSave()
    {
        // 获取重生坐标：优先使用 spawnPoint 的位置，没有则用自身位置
        Vector3 spawnPos = (spawnPoint != null) ? spawnPoint.position : transform.position;

    
        if (SaveController != null)
        {
            SaveController.Save(spawnPos, checkpointId);
            print("存档成功，存档点: " + checkpointId);
        }
        else
        {
            print("存档失败：场景中没有找到 SaveController");
        }
    }
}