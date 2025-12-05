using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using 国家系统;
using 玩家数据结构;

public class 战场管理器 : MonoBehaviour
{
    public static 战场管理器 Instance { get; private set; }

    [Header("战场设置")]
    private float 准备时间 = 10f; // 5分钟倒计时

    private Dictionary<int, 战场实例> 活跃战场列表 = new Dictionary<int, 战场实例>();
    private int 战场ID计数器 = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
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

        // 创建新的战场实例
        战场实例 新战场 = new 战场实例()
        {
            战场ID = ++战场ID计数器,
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

        // 启动战场逻辑协程
        StartCoroutine(战场逻辑协程(战场));

        // 通知所有玩家战场开始
        通知战场开始(战场);
    }

    /// <summary>
    /// 战场主逻辑协程
    /// </summary>
    private IEnumerator 战场逻辑协程(战场实例 战场)
    {
        while (战场.战场状态 == 战场状态.进行中)
        {
            // Boss攻击逻辑（每3秒）
            if (Time.time - 战场.上次Boss攻击时间 >= 3f)
            {
                执行Boss攻击(战场);
                战场.上次Boss攻击时间 = Time.time;
            }

            // 检查胜利条件
            检查胜利条件(战场);

            yield return new WaitForSeconds(0.1f); // 避免过于频繁的检查
        }
    }

    /// <summary>
    /// 执行Boss攻击逻辑
    /// </summary>
    private void 执行Boss攻击(战场实例 战场)
    {
        // Boss归属方获得积分
        if (战场.Boss归属 == 1)
        {
            战场.家族1积分 += 50;
        }
        else if (战场.Boss归属 == 2)
        {
            战场.家族2积分 += 50;
        }

        Debug.Log($"Boss攻击！家族1积分: {战场.家族1积分}, 家族2积分: {战场.家族2积分}");
    }

    /// <summary>
    /// 检查胜利条件
    /// </summary>
    private void 检查胜利条件(战场实例 战场)
    {
        if (战场.家族1积分 >= 10000)
        {
            结束战场(战场, 战场.家族1);
        }
        else if (战场.家族2积分 >= 10000)
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
        if (胜利家族.族长ID != -1)
        {
            战场.所属国家.国王ID = 胜利家族.族长ID;
            Debug.Log($"{全局变量.所有玩家数据表[胜利家族.族长ID].姓名} 登顶王位！");
        }

        // 清理战场数据
        战场.所属国家.宣战家族1 = null;
        战场.所属国家.宣战家族2 = null;

        // 通知战场结束
        通知战场结束(战场);

        // 从活跃列表中移除
        活跃战场列表.Remove(战场.战场ID);
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
        Debug.Log($"通知: 战场结束，胜利家族: {战场.胜利家族?.家族名字}");
    }
}