# 2D横版闯关游戏（Unity）
基于 Unity 开发的 2D 横版平台闯关游戏，包含玩家控制、敌人AI、存档系统、背包系统、UI交互等核心功能。

## 快速开始
### 环境要求
- Unity 版本：2022.3
- 运行平台：Windows

### 运行步骤
1. 克隆仓库到本地：
   ```bash
  git clone https://github.com/xm-826/2djump.git
2.用 Unity Hub 打开项目根目录
3.打开场景文件（比如 Assets/Scenes/MainScene.unity）
4.点击 Unity 编辑器的「Play」按钮开始运行

#### 核心功能说明
玩家系统
基础移动（左右、跳跃）、落地 / 受伤 / 死亡 / 复活状态机
血量系统、受击反馈、加速 / 回血道具交互
地面射线检测（多射线判定落地，避免漏判）

敌人系统
巡逻 / 追击状态 AI（射线检测发现玩家）
碰撞判定（玩家踩头顶击杀，侧面碰撞扣血）
状态机管理（Idle/Move/Run/Die/Turn）

存档系统
存档点触发（按 E 存档，自动存档可选）
保存 / 读取：玩家位置、金币数量、背包物品
本地 JSON 持久化存储（路径：Application.persistentDataPath/Checkpoint.json）

背包系统
物品拾取 / 使用 / 堆叠（ScriptableObject 配置物品属性）
Tab 键打开 / 关闭背包，暂停游戏时间
物品描述、选中高亮、数量管理

其他功能
移动平台（玩家跟随平台移动）
金币收集（动画 + 计数）
UI 系统（开始界面 / 死亡界面 / 设置界面 / 结算界面）
背景切换（根据玩家 Y 坐标切换蓝 / 绿背景）

#### 代码结构
PlayerController	玩家状态机、移动 / 跳跃 / 受伤 / 复活

EnemyController	敌人 AI、状态机、碰撞判定

SaveController	存档 / 读档逻辑、JSON 序列化

InventoryManager	背包管理、物品拾取 / 使用

SaveTrigger	存档点触发器（按 E 存档）

ItemSo	物品配置（ScriptableObject）

UIController	所有 UI 界面的交互逻辑

GameManager	全局金币计数、单例管理

#### 学习笔记
状态机模式：用枚举管理玩家 / 敌人状态，拆分 UpdateState（状态判断）和 ExecuteStateBehavior（状态行为），逻辑更清晰

射线检测：多射线判定地面，解决单射线漏判问题

存档序列化：Dictionary 无法直接序列化，需转为 List 存储

单例模式：GameManager/UIController 用单例管理全局状态，避免重复挂载
