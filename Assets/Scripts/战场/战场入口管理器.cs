using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using 玩家数据结构;

public class 战场入口管理器 : MonoBehaviour
{
    [Header("入口UI组件")]
    public GameObject 战场入口弹窗;
    public Text 战场提示文本;
    public Button 进入战场按钮;
    public Button 稍后进入按钮;

    [Header("战场主界面")]
    public GameObject 战场主界面; // 你的王城战战场UI

    private 战场实例 当前战场;
    private bool 已显示提示 = false;

    private void Start()
    {
        // 绑定按钮事件
        if (进入战场按钮 != null)
            进入战场按钮.onClick.AddListener(点击进入战场);

        if (稍后进入按钮 != null)
            稍后进入按钮.onClick.AddListener(点击稍后进入);

        // 初始隐藏UI
        if (战场入口弹窗 != null)
            战场入口弹窗.SetActive(false);
    }

    private void Update()
    {
        检查战场状态();
    }

    private void 检查战场状态()
    {
        if (战场管理器.Instance == null) return;

        玩家数据 当前玩家 = 全局变量.所有玩家数据表[全局变量.当前身份];
        战场实例 玩家战场 = 战场管理器.Instance.获取玩家战场(当前玩家);

        if (玩家战场 != null)
        {
            当前战场 = 玩家战场;

            // 如果玩家还没进入战场，显示入口提示
            if (!当前战场.参战玩家列表.Contains(当前玩家))
            {
                显示战场入口提示();
            }
            else
            {
                // 玩家已在战场中，隐藏入口提示
                隐藏战场入口提示();
            }
        }
        else
        {
            // 没有战场时隐藏提示
            隐藏战场入口提示();
            已显示提示 = false;
        }
    }

    private void 显示战场入口提示()
    {
        if (战场入口弹窗 == null || 当前战场 == null) return;

        // 显示弹窗
        战场入口弹窗.SetActive(true);

        // 更新提示文本
        if (战场提示文本 != null)
        {
            玩家数据 当前玩家 = 全局变量.所有玩家数据表[全局变量.当前身份];
            string 对手家族 = (当前玩家.家族 == 当前战场.家族1) ?
                            当前战场.家族2.家族名字 : 当前战场.家族1.家族名字;

            if (当前战场.战场状态 == 战场状态.准备中)
            {
                战场提示文本.text = $"王城战即将开始！\n" +
                                   $"对手家族：{对手家族}\n" +
                                   $"剩余准备时间：{(int)当前战场.剩余准备时间}秒\n\n" +
                                   $"是否现在进入战场？";
            }
            else if (当前战场.战场状态 == 战场状态.进行中)
            {
                战场提示文本.text = $"王城战正在进行中！\n" +
                                   $"对手家族：{对手家族}\n" +
                                   $"家族1积分：{当前战场.家族1积分}\n" +
                                   $"家族2积分：{当前战场.家族2积分}\n\n" +
                                   $"是否立即加入战斗？";
            }
        }

        已显示提示 = true;
    }

    private void 隐藏战场入口提示()
    {
        if (战场入口弹窗 != null)
            战场入口弹窗.SetActive(false);
    }

    public void 点击进入战场()
    {
        if (当前战场 == null || 战场管理器.Instance == null) return;

        玩家数据 当前玩家 = 全局变量.所有玩家数据表[全局变量.当前身份];

        // 玩家进入战场
        if (战场管理器.Instance.玩家进入战场(当前战场.战场ID, 当前玩家))
        {
            通用提示框.显示($"成功进入战场！");

            // 隐藏入口弹窗
            隐藏战场入口提示();

            // 如果战场正在进行，打开战场主界面
            if (当前战场.战场状态 == 战场状态.进行中 && 战场主界面 != null)
            {
                战场主界面.SetActive(true);
            }
        }
        else
        {
            通用提示框.显示("进入战场失败！");
        }
    }

    public void 点击稍后进入()
    {
        // 暂时隐藏提示，但不进入战场
        隐藏战场入口提示();
        已显示提示 = false; // 允许再次显示

        通用提示框.显示("你可以随时通过菜单进入战场");
    }

    /// <summary>
    /// 手动打开战场（可以绑定到菜单按钮）
    /// </summary>
    public void 手动打开战场()
    {
        玩家数据 当前玩家 = 全局变量.所有玩家数据表[全局变量.当前身份];

        if (当前战场 != null)
        {
            if (!当前战场.参战玩家列表.Contains(当前玩家))
            {
                // 还没进入，先进入战场
                点击进入战场();
            }
            else
            {
                // 已经在战场中，直接打开界面
                if (战场主界面 != null)
                    战场主界面.SetActive(true);
            }
        }
        else
        {
            通用提示框.显示("当前没有可参与的战场");
        }
    }
}