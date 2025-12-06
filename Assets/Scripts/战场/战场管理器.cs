using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using 国家系统;
using 玩家数据结构;

public class 战场管理器 : MonoBehaviour
{
    public static 战场管理器 Instance { get; private set; }
    public 王城战战场 王城战战场;
    public 国家功能显示 国家功能显示;

    [Header("战场设置")]
    private float 准备时间 = 10f; // 5分钟倒计时

    // 战场类型固定ID
    private const int 王城战战场ID = 1;
    private const int 攻城战战场ID = 2;
    private const int 家族战战场ID = 3;
    // 可以继续添加其他战场类型...

    private Dictionary<int, 战场实例> 活跃战场列表 = new Dictionary<int, 战场实例>();
    private int 战场ID计数器 = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // 游戏启动时清理可能残留的战场数据
            清理所有战场();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 启动王城战（当两个家族都宣战后调用）
    /// </summary>
    public void 启动王城战(国家信息库 国家, 家族信息库 家族1, 家族信息库 家族2)
    {
        Debug.Log($"启动王城战: {家族1.家族名字} VS {家族2.家族名字}");

        // 清理可能存在的王城战战场
        if (活跃战场列表.ContainsKey(王城战战场ID))
        {
            Debug.Log("检测到已存在的王城战战场，正在清理...");
            结束战场(活跃战场列表[王城战战场ID], null);
        }

        // 创建新的王城战战场实例
        战场实例 新战场 = new 战场实例()
        {
            战场ID = 王城战战场ID,
            战场类型 = "王城战",
            所属国家 = 国家,
            家族1 = 家族1,
            家族2 = 家族2,
            战场状态 = 战场状态.准备中,
            剩余准备时间 = 准备时间
        };

        活跃战场列表.Add(新战场.战场ID, 新战场);

        // 启动倒计时协程
        StartCoroutine(战场倒计时协程(新战场));

        // 通知所有相关玩家战场即将开始
        通知战场准备(新战场);

        Debug.Log($"王城战战场创建成功，ID: {新战场.战场ID}，{新战场.剩余准备时间}秒后开始");
    }

    /// <summary>
    /// 战场倒计时协程
    /// </summary>
    private IEnumerator 战场倒计时协程(战场实例 战场)
    {
        Debug.Log($"战场 {战场.战场ID} 开始倒计时，剩余时间: {战场.剩余准备时间}秒");

        while (战场.剩余准备时间 > 0)
        {
            yield return new WaitForSeconds(1f);
            战场.剩余准备时间--;

            // 每30秒通知一次
            if (战场.剩余准备时间 % 30 == 0 || 战场.剩余准备时间 <= 10)
            {
                Debug.Log($"战场 {战场.战场ID} 倒计时: {战场.剩余准备时间}秒");
            }
        }

        // 倒计时结束，正式开始战场
        开始战场(战场);
    }

    /// <summary>
    /// 正式开始战场
    /// </summary>
    private void 开始战场(战场实例 战场)
    {
        Debug.Log($"战场 {战场.战场ID} 正式开始！");

        战场.战场状态 = 战场状态.进行中;
        战场.开始时间 = Time.time;

        // 新增：AI家族成员自动进入战场
        AI家族成员自动进入战场(战场);

        // 启动战场逻辑协程
        StartCoroutine(战场逻辑协程(战场));

        // 通知所有玩家战场开始
        通知战场开始(战场);
    }

    /// <summary>
    /// AI家族成员自动进入战场
    /// </summary>
    private void AI家族成员自动进入战场(战场实例 战场)
    {
        // 获取当前真实玩家
        玩家数据 真实玩家 = 全局变量.所有玩家数据表[全局变量.当前身份];

        // 确定哪个是AI家族（不是真实玩家的家族）
        家族信息库 AI家族 = null;
        if (真实玩家.家族 == 战场.家族1)
        {
            AI家族 = 战场.家族2;
        }
        else if (真实玩家.家族 == 战场.家族2)
        {
            AI家族 = 战场.家族1;
        }

        if (AI家族 != null)
        {
            Debug.Log($"AI家族 {AI家族.家族名字} 的成员自动进入战场");

            // 让AI家族的所有成员自动进入战场
            foreach (var AI成员 in AI家族.家族成员)
            {
                if (!战场.参战玩家列表.Contains(AI成员))
                {
                    战场.参战玩家列表.Add(AI成员);
                    Debug.Log($"AI玩家 {AI成员.姓名} 自动进入战场");
                }
            }

            Debug.Log($"AI家族共有 {AI家族.家族成员.Count} 名成员进入战场");
        }
        else
        {
            Debug.LogWarning("未找到AI家族，可能真实玩家没有参与此战场");
        }
    }

    /// <summary>
    /// 战场主逻辑协程
    /// </summary>
    private IEnumerator 战场逻辑协程(战场实例 战场)
    {
        while (战场.战场状态 == 战场状态.进行中)
        {
            // Boss归属积分结算（每3秒）
            if (Time.time - 战场.上次Boss攻击时间 >= 3f)
            {
                检测Boss归属并加积分(战场);
                战场.上次Boss攻击时间 = Time.time;
            }

            // 检查胜利条件
            检查胜利条件(战场);

            yield return new WaitForSeconds(0.1f); // 避免过于频繁的检查
        }
    }

    /// <summary>
    /// 检测Boss归属并给归属方加积分
    /// </summary>
    private void 检测Boss归属并加积分(战场实例 战场)
    {
        // Boss归属方获得积分
        if (战场.Boss归属 == 1)
        {
            战场.家族1积分 += 50;
            Debug.Log($"{战场.家族1.家族名字} +50积分!");
        }
        else if (战场.Boss归属 == 2)
        {
            战场.家族2积分 += 50;
            Debug.Log($"{战场.家族2.家族名字} +50积分!");
        }
    }

    /// <summary>
    /// 检查胜利条件
    /// </summary>
    private void 检查胜利条件(战场实例 战场)
    {
        if (战场.家族1积分 >= 1000) //1万是正常值，临时测试改为1000
        {
            结束战场(战场, 战场.家族1);
        }
        else if (战场.家族2积分 >= 1000)
        {
            结束战场(战场, 战场.家族2);
        }
    }

    /// <summary>
    /// 结束战场
    /// </summary>
    private void 结束战场(战场实例 战场, 家族信息库 胜利家族)
    {
        Debug.Log($"战场 {战场.战场ID} 结束！胜利家族: {胜利家族.家族名字}");

        战场.战场状态 = 战场状态.已结束;
        战场.胜利家族 = 胜利家族;

        // 胜利家族族长登顶王位
        if (战场.所属国家.国王ID != -1) //如果不是-1就代表本国有国王了
        {
            if (战场.所属国家.国王ID != 胜利家族.族长ID)
            {
                全局变量.所有玩家数据表[战场.所属国家.国王ID].官职 = 官职枚举.国民;
                战场.所属国家.国王ID = 胜利家族.族长ID;
                战场.所属国家.执政家族 = 胜利家族;
                全局变量.所有玩家数据表[胜利家族.族长ID].官职 = 官职枚举.国王;
            }
            
        }
        else
        {
            if (胜利家族.族长ID != -1)
            {
                战场.所属国家.国王ID = 胜利家族.族长ID;
                战场.所属国家.执政家族 = 胜利家族;
            }
        }
        Debug.Log($"{全局变量.所有玩家数据表[胜利家族.族长ID].姓名} 登顶王位！");

        // 停止战场协程
        StopCoroutine(战场逻辑协程(战场));

        // 清理战场数据
        战场.所属国家.宣战家族1 = null;
        战场.所属国家.宣战家族2 = null;
        战场.所属国家.宣战家族1 = 胜利家族;
        // 清空参战玩家列表
        战场.参战玩家列表.Clear();

        // 重置战场数据
        战场.家族1积分 = 0;
        战场.家族2积分 = 0;
        战场.Boss血量 = 0;
        战场.Boss归属 = 0;

        // 通知战场结束
        通知战场结束(战场);

        // 从活跃列表中移除
        活跃战场列表.Remove(战场.战场ID);

        Debug.Log($"战场 {战场.战场ID} 已完全销毁和清理");
    }

    /// <summary>
    /// 清理所有活跃战场（用于重置或调试）
    /// </summary>
    public void 清理所有战场()
    {
        foreach (var 战场 in 活跃战场列表.Values)
        {
            战场.战场状态 = 战场状态.已结束;
            战场.参战玩家列表.Clear();
        }

        活跃战场列表.Clear();
        Debug.Log("所有战场已清理");
    }

    /// <summary>
    /// 玩家进入战场
    /// </summary>
    public bool 玩家进入战场(int 战场ID, 玩家数据 玩家)
    {
        if (活跃战场列表.TryGetValue(战场ID, out 战场实例 战场))
        {
            if (!战场.参战玩家列表.Contains(玩家))
            {
                战场.参战玩家列表.Add(玩家);
                Debug.Log($"玩家 {玩家.姓名} 进入战场 {战场ID}");
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 玩家退出战场
    /// </summary>
    public bool 玩家退出战场(int 战场ID, 玩家数据 玩家)
    {
        if (活跃战场列表.TryGetValue(战场ID, out 战场实例 战场))
        {
            if (战场.参战玩家列表.Remove(玩家))
            {
                Debug.Log($"玩家 {玩家.姓名} 退出战场 {战场ID}");
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 获取战场实例
    /// </summary>
    public 战场实例 获取战场实例(int 战场ID)
    {
        活跃战场列表.TryGetValue(战场ID, out 战场实例 战场);
        return 战场;
    }

    /// <summary>
    /// 获取玩家当前可参与的战场
    /// </summary>
    public 战场实例 获取玩家战场(玩家数据 玩家)
    {
        // 首先检查玩家本身是否为null
        if (玩家 == null)
        {
            Debug.Log("玩家对象是空的");
            return null;
        }

        foreach (var 战场 in 活跃战场列表.Values)
        {
            if (战场.家族1 == null)
            {
                Debug.Log("战场家族1是空的");
            }
            if (战场.家族2 == null)
            {
                Debug.Log("战场家族2是空的");
            }
            if (玩家.家族 == null)
            {
                Debug.Log("玩家家族是空的");
            }
            if ((战场.家族1 == 玩家.家族 || 战场.家族2 == 玩家.家族) &&
                战场.战场状态 != 战场状态.已结束)
            {
                return 战场;
            }
        }
        return null;
    }

    // 通知方法（后续可以扩展为事件系统）
    private void 通知战场准备(战场实例 战场)
    {
        // 这里可以触发UI显示、发送消息等
        Debug.Log($"通知: 战场准备中，{战场.剩余准备时间}秒后开始");
    }

    private void 通知战场开始(战场实例 战场)
    {
        Debug.Log("通知: 战场正式开始！");
    }

    private void 通知战场结束(战场实例 战场)
    {
        通用提示框.显示($"王城战结束，胜利家族: {战场.胜利家族?.家族名字}");
        Debug.Log($"通知: 战场结束，胜利家族: {战场.胜利家族?.家族名字}");
        国家功能显示.刷新显示();
        王城战战场.gameObject.SetActive(false);
    }
}