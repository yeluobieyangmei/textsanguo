using System.Collections.Generic;
using UnityEngine;
using 国家系统;
using 玩家数据结构;

public enum 战场状态
{
    准备中,
    进行中,
    已结束
}

public class 战场实例
{
    public int 战场ID { get; set; }
    public string 战场类型 { get; set; }
    public 国家信息库 所属国家 { get; set; }
    public 家族信息库 家族1 { get; set; }
    public 家族信息库 家族2 { get; set; }
    public 战场状态 战场状态 { get; set; }

    // 时间相关
    public float 剩余准备时间 { get; set; }
    public float 开始时间 { get; set; }
    public float 上次Boss攻击时间 { get; set; }
    
    // 战场倒计时相关
    public float 战场总时长 { get; set; } = 1800f; // 30分钟 = 1800秒
    public float 加时时长 { get; set; } = 300f; // 5分钟 = 300秒
    public float 当前剩余时间 { get; set; } = 0f;
    public bool 倒计时进行中 { get; set; } = false;
    public bool 是否加时阶段 { get; set; } = false;

    // 战斗数据
    public int 家族1积分 { get; set; } = 0;
    public int 家族2积分 { get; set; } = 0;
    public int Boss归属 { get; set; } = 0; // 0=无归属, 1=家族1, 2=家族2
    public int Boss血量 { get; set; } = 1000000;
    public int Boss最大血量 { get; set; } = 1000000;

    // 参战玩家
    public List<玩家数据> 参战玩家列表 { get; set; } = new List<玩家数据>();

    // 胜利信息
    public 家族信息库 胜利家族 { get; set; }

    /// <summary>
    /// 获取对方家族的参战成员（用于UI显示）
    /// </summary>
    public List<玩家数据> 获取对方家族成员(玩家数据 当前玩家)
    {
        List<玩家数据> 对方成员 = new List<玩家数据>();

        // 确定对方家族
        家族信息库 对方家族 = null;
        if (当前玩家.家族 == 家族1)
        {
            对方家族 = 家族2;
        }
        else if (当前玩家.家族 == 家族2)
        {
            对方家族 = 家族1;
        }

        if (对方家族 != null)
        {
            // 只返回已经进入战场的对方家族成员
            foreach (var 玩家 in 参战玩家列表)
            {
                if (玩家.家族 == 对方家族)
                {
                    对方成员.Add(玩家);
                }
            }
        }

        return 对方成员;
    }

    /// <summary>
    /// 玩家攻击Boss
    /// </summary>
    public bool 攻击Boss(玩家数据 攻击者, int 伤害)
    {
        if (战场状态 != 战场状态.进行中) return false;

        Boss血量 -= 伤害;
        if (Boss血量 <= 0)
        {
            // Boss归属给击杀者的家族
            if (攻击者.家族 == 家族1)
            {
                Boss归属 = 1;
            }
            else if (攻击者.家族 == 家族2)
            {
                Boss归属 = 2;
            }

            // 恢复Boss血量到最大值
            Boss血量 = Boss最大血量;

            Debug.Log($"Boss被 {攻击者.姓名} 击败！归属: {(Boss归属 == 1 ? 家族1.家族名字 : 家族2.家族名字)}，Boss血量已恢复");
            return true;
        }

        return false;
    }
}