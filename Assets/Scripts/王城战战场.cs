using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using 怪物数据结构;
using 玩家数据结构;
using 国家系统;

public class 王城战战场 : MonoBehaviour
{
    [Header("Boss信息显示")]
    public Text Boss血量;
    public Text Boss归属;
    public Button 攻击Boss按钮;

    [Header("家族信息显示")]
    public Text 家族1名字;
    public Text 家族2名字;
    public Text 家族1积分;
    public Text 家族2积分;

    [Header("战场控制")]
    public Button 退出战场按钮;

    // 私有变量
    private 战场实例 当前连接的战场;
    private 玩家数据 当前玩家;

    private void Start()
    {
        Debug.Log("------------------------开始获取当前玩家");
        当前玩家 = 全局变量.所有玩家数据表[全局变量.当前身份];
        Debug.Log($"------------------------当前玩家是:{当前玩家.姓名}");

        // 检查UI组件是否正确分配
        Debug.Log($"Boss血量组件: {Boss血量 != null}");
        Debug.Log($"Boss归属组件: {Boss归属 != null}");
        Debug.Log($"家族1名字组件: {家族1名字 != null}");
        Debug.Log($"家族2名字组件: {家族2名字 != null}");
        Debug.Log($"家族1积分组件: {家族1积分 != null}");
        Debug.Log($"家族2积分组件: {家族2积分 != null}");

        // 绑定按钮事件
        if (攻击Boss按钮 != null)
            攻击Boss按钮.onClick.AddListener(点击攻击Boss);

        if (退出战场按钮 != null)
            退出战场按钮.onClick.AddListener(点击退出战场);
    }

    private IEnumerator 延迟自动连接()
    {
        // 等待一帧，确保Start方法执行完毕
        yield return null;

        // 现在执行自动连接
        自动连接玩家战场();

        // 如果已经连接到战场，强制刷新一次
        if (当前连接的战场 != null)
        {
            Debug.Log("强制刷新战场显示");
            刷新战场显示();
        }
    }

    private void OnEnable()
    {
        Debug.Log("王城战战场UI被激活");
        // 延迟执行，确保Start方法先执行
        StartCoroutine(延迟自动连接());
    }

    private void Update()
    {
        // 实时刷新战场数据
        if (当前连接的战场 != null)
        {
            刷新战场显示();
        }
    }

    /// <summary>
    /// 自动连接到当前玩家参与的战场
    /// </summary>
    private void 自动连接玩家战场()
    {
        Debug.Log($"------------------------自动连接玩家战场里当前玩家是:{当前玩家.姓名}");

        if (当前玩家 == null || 当前玩家.家族 == null)
        {
            Debug.LogWarning("当前玩家没有家族，无法连接战场");
            return;
        }

        if (战场管理器.Instance != null)
        {
            战场实例 玩家战场 = 战场管理器.Instance.获取玩家战场(当前玩家);
            if (玩家战场 != null && 玩家战场.战场状态 == 战场状态.进行中)
            {
                连接战场实例(玩家战场);
            }
            else
            {
                Debug.Log("未找到可连接的战场或战场未开始");
            }
        }
    }

    /// <summary>
    /// 连接到指定的战场实例
    /// </summary>
    public void 连接战场实例(战场实例 战场)
    {
        当前连接的战场 = 战场;

        if (战场 != null)
        {
            Debug.Log($"UI成功连接到战场 {战场.战场ID}");
            Debug.Log($"家族1: {战场.家族1?.家族名字}");
            Debug.Log($"家族2: {战场.家族2?.家族名字}");
            Debug.Log($"战场状态: {战场.战场状态}");

            刷新战场显示();
        }
        else
        {
            Debug.LogError("尝试连接空的战场实例");
        }
    }

    /// <summary>
    /// 刷新战场基本信息显示
    /// </summary>
    private void 刷新战场显示()
    {
        if (当前连接的战场 == null) return;

        // 更新Boss信息
        if (Boss血量 != null)
            Boss血量.text = $"Boss当前血量：{当前连接的战场.Boss血量}";

        if (Boss归属 != null)
        {
            string 归属文本 = "无";
            if (当前连接的战场.Boss归属 == 1)
                归属文本 = 当前连接的战场.家族1.家族名字;
            else if (当前连接的战场.Boss归属 == 2)
                归属文本 = 当前连接的战场.家族2.家族名字;

            Boss归属.text = $"归属：{归属文本}";
        }

        // 更新家族信息
        if (家族1名字 != null)
            家族1名字.text = 当前连接的战场.家族1.家族名字;

        if (家族2名字 != null)
            家族2名字.text = 当前连接的战场.家族2.家族名字;

        if (家族1积分 != null)
            家族1积分.text = 当前连接的战场.家族1积分.ToString();

        if (家族2积分 != null)
            家族2积分.text = 当前连接的战场.家族2积分.ToString();
    }

    /// <summary>
    /// 点击攻击Boss
    /// </summary>
    public void 点击攻击Boss()
    {
        if (当前连接的战场 == null)
        {
            通用提示框.显示("未连接到战场！");
            return;
        }

        if (当前连接的战场.战场状态 != 战场状态.进行中)
        {
            通用提示框.显示("战场未开始！");
            return;
        }

        // 检查玩家是否在战场中
        if (!当前连接的战场.参战玩家列表.Contains(当前玩家))
        {
            通用提示框.显示("你还没有进入战场！");
            return;
        }

        // 计算攻击伤害（可以根据玩家属性计算）
        int 攻击伤害 = 当前玩家.玩家属性.攻击力 * Random.Range(80, 121) / 100; // 80%-120%的攻击力

        // 攻击Boss
        bool Boss被击败 = 当前连接的战场.攻击Boss(当前玩家, 攻击伤害);

        通用提示框.显示($"对Boss造成 {攻击伤害} 点伤害！");

        if (Boss被击败)
        {
            通用提示框.显示("Boss被击败！你的家族获得了Boss归属权！");
        }
    }

    /// <summary>
    /// 点击退出战场
    /// </summary>
    public void 点击退出战场()
    {
        if (当前连接的战场 == null || 战场管理器.Instance == null) return;

        if (战场管理器.Instance.玩家退出战场(当前连接的战场.战场ID, 当前玩家))
        {
            通用提示框.显示("已退出战场");

            // 清理UI
            当前连接的战场 = null;

            // 关闭界面
            gameObject.SetActive(false);
        }
    }
}