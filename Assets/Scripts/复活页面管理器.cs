using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using 玩家数据结构;

/// <summary>
/// 复活页面管理器 - 作为Unity按钮和战斗系统静态方法之间的桥梁
/// </summary>
public class 复活页面管理器 : MonoBehaviour
{
    [Header("UI组件")]
    public Text 标题文本;
    public Text 内容文本;
    public Button 完美复活按钮;
    public Button 半血复活按钮;
    public Button 退出战场按钮;
    public GameObject 弹窗主体;

    // 静态实例，方便全局调用
    public static 复活页面管理器 实例;

    // 复活成本常量
    private const int 完美复活黄金成本 = 100;
    private const int 半血复活铜钱成本 = 2000;

    // 当前死亡的玩家
    private 玩家数据 死亡玩家;

    private void Awake()
    {
        // 设置单例
        if (实例 == null)
        {
            实例 = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // 初始化按钮事件
        初始化按钮事件();

        // 初始时隐藏弹窗
        隐藏弹窗();
    }

    private void 初始化按钮事件()
    {
        if (完美复活按钮 != null)
        {
            完美复活按钮.onClick.RemoveAllListeners();
            完美复活按钮.onClick.AddListener(点击完美复活);
        }

        if (半血复活按钮 != null)
        {
            半血复活按钮.onClick.RemoveAllListeners();
            半血复活按钮.onClick.AddListener(点击半血复活);
        }

        if (退出战场按钮 != null)
        {
            退出战场按钮.onClick.RemoveAllListeners();
            退出战场按钮.onClick.AddListener(点击退出战场);
        }
    }

    /// <summary>
    /// 显示死亡复活弹窗
    /// </summary>
    public void 显示死亡弹窗(玩家数据 死亡的玩家)
    {
        死亡玩家 = 死亡的玩家;

        // 设置弹窗内容
        if (标题文本 != null)
            标题文本.text = "角色死亡";

        if (内容文本 != null)
            内容文本.text = "完美复活:每次消耗100黄金，恢复100%血量\n半血复活: 每次消耗2000铜钱，恢复50 % 血量";

        // 更新按钮文本和状态
        更新复活按钮状态();

        // 显示弹窗
        if (弹窗主体 != null)
            弹窗主体.SetActive(true);
    }

    private void 更新复活按钮状态()
    {
        if (死亡玩家 == null) return;

        // 检查完美复活是否可用
        bool 可以完美复活 = 死亡玩家.黄金 >= 完美复活黄金成本;
        if (完美复活按钮 != null)
            完美复活按钮.interactable = 可以完美复活;


        // 检查半血复活是否可用
        bool 可以半血复活 = 死亡玩家.铜钱 >= 半血复活铜钱成本;
        if (半血复活按钮 != null)
            半血复活按钮.interactable = 可以半血复活;
    }

    // ========== 以下是按钮点击事件，可以在Unity中直接绑定 ==========

    /// <summary>
    /// 点击完美复活按钮（Unity按钮可直接调用）
    /// </summary>
    public void 点击完美复活()
    {
        if (死亡玩家 == null) return;

        // 调用战斗系统的静态方法
        if (战斗系统.完美复活(死亡玩家))
        {
            通用提示框.显示($"完美复活成功！消耗 {完美复活黄金成本} 黄金");
            隐藏弹窗();
        }
        else
        {
            通用提示框.显示("黄金不足，无法完美复活！");
            更新复活按钮状态();
        }
    }

    /// <summary>
    /// 点击半血复活按钮（Unity按钮可直接调用）
    /// </summary>
    public void 点击半血复活()
    {
        if (死亡玩家 == null) return;

        // 调用战斗系统的静态方法
        if (战斗系统.半血复活(死亡玩家))
        {
            通用提示框.显示($"半血复活成功！消耗 {半血复活铜钱成本} 铜钱");
            隐藏弹窗();
        }
        else
        {
            通用提示框.显示("铜钱不足，无法半血复活！");
            更新复活按钮状态();
        }
    }

    /// <summary>
    /// 点击退出战场按钮（Unity按钮可直接调用）
    /// </summary>
    public void 点击退出战场()
    {
        if (死亡玩家 == null) return;

        Debug.Log($"{死亡玩家.姓名} 选择退出战场");

        // 退出战场逻辑
        if (战场管理器.Instance != null)
        {
            战场实例 玩家战场 = 战场管理器.Instance.获取玩家战场(死亡玩家);
            if (玩家战场 != null)
            {
                // 退出战场数据
                if (战场管理器.Instance.玩家退出战场(玩家战场.战场ID, 死亡玩家))
                {
                    通用提示框.显示("已退出战场");

                    // 关闭战场UI（如果当前玩家是死亡的玩家）
                    if (死亡玩家.ID == 全局变量.当前身份)
                    {
                        关闭战场UI();
                    }
                }
            }
        }

        隐藏弹窗();
    }

    /// <summary>
    /// 关闭战场UI
    /// </summary>
    private void 关闭战场UI()
    {
        // 查找并关闭王城战战场UI
        王城战战场 战场UI = GameObject.FindObjectOfType<王城战战场>();
        if (战场UI != null && 战场UI.gameObject.activeInHierarchy)
        {
            战场UI.gameObject.SetActive(false);
            Debug.Log("复活弹窗退出战场：已关闭战场UI");
        }
    }

    /// <summary>
    /// 隐藏弹窗
    /// </summary>
    public void 隐藏弹窗()
    {
        if (弹窗主体 != null)
            弹窗主体.SetActive(false);

        死亡玩家 = null;
    }
}