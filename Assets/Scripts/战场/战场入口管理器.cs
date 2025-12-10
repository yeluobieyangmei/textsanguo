using System.Collections;
using System.Collections.Generic;
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
    private bool 已显示30秒提示 = false; // 新增：记录是否已显示30秒提示
    private bool 用户手动关闭 = false; // 新增：记录用户是否手动关闭了UI
    private bool UI当前显示状态 = false; // 新增：记录UI当前是否显示，避免重复操作
    private bool 需要检测战场 = false; // 新增：只有在需要时才检测战场状态
    private Coroutine 当前准备倒计时协程; // 新增：记录当前的准备倒计时协程

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
        // 只有在需要检测时才运行检测逻辑
        if (需要检测战场)
        {
            检查战场状态();
        }
    }

    /// <summary>
    /// 重置显示状态
    /// </summary>
    private void 重置显示状态()
    {
        已显示提示 = false;
        已显示30秒提示 = false;
        用户手动关闭 = false;
        UI当前显示状态 = false;
    }

    private void 检查战场状态()
    {
        if (战场管理器.Instance == null) return;

        玩家数据 当前玩家 = 全局变量.所有玩家数据表[全局变量.当前身份];
        战场实例 玩家战场 = 战场管理器.Instance.获取玩家战场(当前玩家);

        if (玩家战场 != null)
        {
            当前战场 = 玩家战场;

            if (!当前战场.参战玩家列表.Contains(当前玩家))
            {
                检查是否显示入口提示();
                
                // 如果UI已经显示，实时更新UI内容
                if (UI当前显示状态)
                {
                    更新UI显示();
                }
            }
            else
            {
                隐藏战场入口提示();
            }
        }
        else
        {
            隐藏战场入口提示();
            重置显示状态();
        }
    }

    private void 检查是否显示入口提示()
    {
        if (当前战场 == null) return;

        if (当前战场.战场状态 == 战场状态.准备中 && 当前战场.剩余准备时间 <= 战场管理器.Instance.UI显示阈值公开 && !已显示30秒提示)
        {
            显示战场入口提示();
            已显示30秒提示 = true;
        }
        else if (当前战场.战场状态 == 战场状态.进行中 && !已显示提示)
        {
            显示战场入口提示();
        }
    }

    private void 显示战场入口提示()
    {
        if (战场入口弹窗 == null || 当前战场 == null || 用户手动关闭) return;

        战场入口弹窗.SetActive(true);
        UI当前显示状态 = true;

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
                                   $"请做好准备，战场即将开始！";
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

        // 更新按钮状态
        更新按钮状态();

        已显示提示 = true;
    }

    /// <summary>
    /// 实时更新UI显示内容
    /// </summary>
    private void 更新UI显示()
    {
        if (当前战场 == null || 战场提示文本 == null) return;

        玩家数据 当前玩家 = 全局变量.所有玩家数据表[全局变量.当前身份];
        string 对手家族 = (当前玩家.家族 == 当前战场.家族1) ?
                        当前战场.家族2.家族名字 : 当前战场.家族1.家族名字;

        // 实时更新文本内容
        if (当前战场.战场状态 == 战场状态.准备中)
        {
            战场提示文本.text = $"王城战即将开始！\n" +
                               $"对手家族：{对手家族}\n" +
                               $"剩余准备时间：{(int)当前战场.剩余准备时间}秒\n\n" +
                               $"请做好准备，战场即将开始！";
        }
        else if (当前战场.战场状态 == 战场状态.进行中)
        {
            战场提示文本.text = $"王城战正在进行中！\n" +
                               $"对手家族：{对手家族}\n" +
                               $"家族1积分：{当前战场.家族1积分}\n" +
                               $"家族2积分：{当前战场.家族2积分}\n\n" +
                               $"是否立即加入战斗？";
        }

        // 实时更新按钮状态
        更新按钮状态();
    }

    /// <summary>
    /// 更新按钮状态
    /// </summary>
    private void 更新按钮状态()
    {
        if (进入战场按钮 == null) return;

        // 如果战场还在准备中，禁用进入战场按钮
        if (当前战场 != null && 当前战场.战场状态 == 战场状态.准备中)
        {
            进入战场按钮.interactable = false;
            // 可以更改按钮文本显示状态
            Text 按钮文本 = 进入战场按钮.GetComponentInChildren<Text>();
            if (按钮文本 != null)
            {
                按钮文本.text = $"等待开始({(int)当前战场.剩余准备时间}秒)";
            }
        }
        else if (当前战场 != null && 当前战场.战场状态 == 战场状态.进行中)
        {
            进入战场按钮.interactable = true;
            Text 按钮文本 = 进入战场按钮.GetComponentInChildren<Text>();
            if (按钮文本 != null)
            {
                按钮文本.text = "进入战场";
            }
        }
    }

    private void 隐藏战场入口提示()
    {
        if (战场入口弹窗 != null)
        {
            战场入口弹窗.SetActive(false);
            UI当前显示状态 = false; // 更新显示状态
            Debug.Log("战场入口UI已隐藏"); // 修改日志信息
        }
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
        // 设置用户手动关闭标志
        用户手动关闭 = true;

        // 隐藏提示
        隐藏战场入口提示();

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

    /// <summary>
    /// 开始战场准备倒计时（由战场管理器调用）
    /// </summary>
    /// <param name="准备时间">准备时间（秒）</param>
    /// <param name="UI显示阈值">UI显示阈值（秒）</param>
    public void 开始战场准备倒计时(float 准备时间, float UI显示阈值)
    {
        // 停止之前的倒计时协程
        if (当前准备倒计时协程 != null)
        {
            StopCoroutine(当前准备倒计时协程);
        }

        // 重置状态
        重置显示状态();

        // 启动新的倒计时协程
        当前准备倒计时协程 = StartCoroutine(准备阶段倒计时协程(准备时间, UI显示阈值));

        Debug.Log($"开始战场准备倒计时：{准备时间}秒，UI将在{准备时间 - UI显示阈值}秒后显示");
    }

    /// <summary>
    /// 战场开始后启动实时检测（由战场管理器调用）
    /// </summary>
    public void 开始战场实时检测()
    {
        // 停止准备倒计时协程
        if (当前准备倒计时协程 != null)
        {
            StopCoroutine(当前准备倒计时协程);
            当前准备倒计时协程 = null;
        }

        // 启动实时检测
        需要检测战场 = true;
        Debug.Log("战场已开始，启动实时检测模式");
    }

    /// <summary>
    /// 停止所有检测（战场结束时调用）
    /// </summary>
    public void 停止所有检测()
    {
        // 停止准备倒计时协程
        if (当前准备倒计时协程 != null)
        {
            StopCoroutine(当前准备倒计时协程);
            当前准备倒计时协程 = null;
        }

        // 停止实时检测
        需要检测战场 = false;

        // 隐藏UI
        if (UI当前显示状态)
        {
            隐藏战场入口提示();
        }

        // 重置状态
        重置显示状态();
        当前战场 = null;

        Debug.Log("战场已结束，停止所有检测");
    }

    /// <summary>
    /// 直接显示战场UI（协程中调用）
    /// </summary>
    private void 直接显示战场UI()
    {
        if (战场管理器.Instance == null) return;

        // 获取当前玩家的战场
        玩家数据 当前玩家 = 全局变量.所有玩家数据表[全局变量.当前身份];
        当前战场 = 战场管理器.Instance.获取玩家战场(当前玩家);

        if (当前战场 != null && !当前战场.参战玩家列表.Contains(当前玩家))
        {
            显示战场入口提示();
            Debug.Log("协程中直接显示战场UI成功");
        }
        else
        {
            Debug.Log("无法显示UI：战场为空或玩家已在战场中");
        }
    }

    /// <summary>
    /// 准备阶段倒计时协程（性能优化版本）
    /// </summary>
    private IEnumerator 准备阶段倒计时协程(float 准备时间, float UI显示阈值)
    {
        float 剩余时间 = 准备时间;
        bool 已显示UI = false;

        while (剩余时间 > 0)
        {
            // 当到达UI显示阈值时，显示UI并启动实时检测
            if (!已显示UI && 剩余时间 <= UI显示阈值 && !用户手动关闭)
            {
                需要检测战场 = true; // 启动实时检测
                已显示UI = true;
                Debug.Log($"到达UI显示阈值（{剩余时间}秒），启动实时检测模式并显示UI");

                // 直接获取战场并显示UI（不等待Update检测）
                直接显示战场UI();
            }

            yield return new WaitForSeconds(1f);
            剩余时间--;
        }

        // 倒计时结束，战场即将开始
        Debug.Log("准备阶段倒计时结束，战场即将开始");
        当前准备倒计时协程 = null;
    }
}