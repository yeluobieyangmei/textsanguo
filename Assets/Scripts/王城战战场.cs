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

    [Header("用于成员列表显示")]
    public Transform 父对象;
    public GameObject 要克隆的对象;
    List<GameObject> 克隆池 = new List<GameObject>();
    玩家数据 当前选中玩家 = null;

    // 私有变量
    private 战场实例 当前连接的战场;
    private 玩家数据 当前玩家;
    private List<玩家数据> 已加入战场敌方玩家列表 = null;

    // 性能优化相关
    private float 上次刷新时间 = 0f;
    private float 刷新间隔 = 0.5f; // 每0.5秒刷新一次
    private int 上次敌方玩家数量 = -1; // 用于检测敌方玩家列表变化

    // Boss攻击冷却相关
    private bool Boss攻击冷却中 = false;
    private float Boss攻击冷却时间 = 3f; // 3秒冷却
    private Coroutine Boss攻击冷却协程;

    public void 刷新玩家对象列表()
    {
        要克隆的对象.gameObject.SetActive(false);
        int count = 已加入战场敌方玩家列表.Count;

        foreach (var obj in 克隆池)//遍历每个被克隆出来的对象
        {
            if (obj != null) Destroy(obj);//如果这个对象在unity中还在不是空的 就Destroy(obj)销毁这个对象
        }
        克隆池.Clear();

        for (int i = 0; i < count; i++)
        {
            GameObject 克隆对象 = Instantiate(要克隆的对象, 父对象);
            克隆对象.transform.GetChild(0).GetComponent<Text>().text = 已加入战场敌方玩家列表[i].姓名;
            克隆对象.gameObject.SetActive(true);
            克隆池.Add(克隆对象);

            // 处理 Toggle 选择逻辑
            Toggle t = 克隆对象.GetComponent<Toggle>();//获取每个克隆对象上的Toggle组件
            玩家数据 捕获玩家 = 已加入战场敌方玩家列表[i]; // 闭包捕获
            t.onValueChanged.AddListener(isOn => //如果这个对象被点击了，就把当前选中玩家赋值为当前选中玩家
            {
                if (isOn)
                {
                    当前选中玩家 = 捕获玩家;
                    Debug.Log($"选中敌方玩家: {当前选中玩家.姓名}");
                }
            });
        }

        // 更新敌方玩家数量记录
        上次敌方玩家数量 = count;
        Debug.Log($"刷新玩家列表完成，当前敌方玩家数量: {count}");
    }

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
            刷新玩家对象列表();
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
        // 性能优化：限制刷新频率
        if (当前连接的战场 != null && Time.time - 上次刷新时间 > 刷新间隔)
        {
            刷新战场显示();
            上次刷新时间 = Time.time;
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

    // 在连接战场实例时赋值
    public void 连接战场实例(战场实例 战场)
    {
        当前连接的战场 = 战场;

        if (战场 != null)
        {
            Debug.Log($"UI成功连接到战场 {战场.战场ID}");
            Debug.Log($"家族1: {战场.家族1?.家族名字}");
            Debug.Log($"家族2: {战场.家族2?.家族名字}");
            Debug.Log($"战场状态: {战场.战场状态}");
            Debug.Log($"战场总参战玩家数量: {战场.参战玩家列表.Count}");

            // 通过公共方法给私有变量赋值
            已加入战场敌方玩家列表 = 获取当前敌方玩家列表();

            // 输出所有参战玩家信息
            Debug.Log("=== 所有参战玩家 ===");
            for (int i = 0; i < 战场.参战玩家列表.Count; i++)
            {
                var 玩家 = 战场.参战玩家列表[i];
                Debug.Log($"  {i + 1}. {玩家.姓名} (家族: {玩家.家族?.家族名字})");
            }

            刷新战场显示();
        }
        else
        {
            Debug.LogError("尝试连接空的战场实例");
            已加入战场敌方玩家列表 = null;
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

        // 检查敌方玩家列表是否有变化，只有变化时才刷新UI
        检查并更新敌方玩家列表();

        // 更新Boss攻击按钮状态（Boss归属变化时按钮状态也要更新）
        更新Boss攻击按钮状态();
    }

    /// <summary>
    /// 检查并更新敌方玩家列表
    /// </summary>
    private void 检查并更新敌方玩家列表()
    {
        // 获取最新的敌方玩家列表
        List<玩家数据> 最新敌方玩家列表 = 获取当前敌方玩家列表();

        // 检查数量是否有变化
        if (最新敌方玩家列表.Count != 上次敌方玩家数量)
        {
            Debug.Log($"敌方玩家数量发生变化: {上次敌方玩家数量} -> {最新敌方玩家列表.Count}");
            已加入战场敌方玩家列表 = 最新敌方玩家列表;
            刷新玩家对象列表();
        }
        else
        {
            // 数量相同，检查成员是否有变化（可选，根据需要决定是否实现）
            bool 成员有变化 = false;
            if (已加入战场敌方玩家列表 != null)
            {
                for (int i = 0; i < 最新敌方玩家列表.Count; i++)
                {
                    if (i >= 已加入战场敌方玩家列表.Count ||
                        已加入战场敌方玩家列表[i].ID != 最新敌方玩家列表[i].ID)
                    {
                        成员有变化 = true;
                        break;
                    }
                }
            }

            if (成员有变化)
            {
                Debug.Log("敌方玩家成员发生变化");
                已加入战场敌方玩家列表 = 最新敌方玩家列表;
                刷新玩家对象列表();
            }
        }
    }

    /// <summary>
    /// 更新Boss攻击按钮状态
    /// </summary>
    private void 更新Boss攻击按钮状态()
    {
        if (攻击Boss按钮 != null && 当前连接的战场 != null && 当前玩家 != null)
        {
            bool 可以攻击Boss = 判断是否可以攻击Boss();

            // 设置按钮是否可点击
            攻击Boss按钮.interactable = !Boss攻击冷却中 && 可以攻击Boss;

            // 更新按钮文字显示状态
            Text 按钮文字 = 攻击Boss按钮.GetComponentInChildren<Text>();
            if (按钮文字 != null)
            {
                if (Boss攻击冷却中)
                {
                    按钮文字.text = "冷却中...";
                }
                else if (!可以攻击Boss)
                {
                    按钮文字.text = "己方Boss";
                }
                else
                {
                    按钮文字.text = "攻击";
                }
            }

            // 设置按钮颜色
            Image 按钮图片 = 攻击Boss按钮.GetComponent<Image>();
            if (按钮图片 != null)
            {
                if (!可以攻击Boss)
                {
                    // Boss属于己方时，按钮显示灰色
                    按钮图片.color = new Color(0.5f, 0.5f, 0.5f, 1f);
                }
                else if (Boss攻击冷却中)
                {
                    // 冷却中显示暗红色
                    按钮图片.color = new Color(0.6f, 0.3f, 0.3f, 1f);
                }
                else
                {
                    // 可以攻击时显示正常红色
                    按钮图片.color = new Color(0.8f, 0.2f, 0.2f, 1f);
                }
            }
        }
    }

    /// <summary>
    /// 判断当前玩家是否可以攻击Boss
    /// </summary>
    private bool 判断是否可以攻击Boss()
    {
        if (当前连接的战场 == null || 当前玩家 == null || 当前玩家.家族 == null)
            return false;

        // Boss无归属时，任何人都可以攻击
        if (当前连接的战场.Boss归属 == 0)
        {
            return true;
        }

        // 判断Boss是否属于当前玩家的家族
        bool Boss属于己方 = false;

        if (当前玩家.家族 == 当前连接的战场.家族1 && 当前连接的战场.Boss归属 == 1)
        {
            Boss属于己方 = true;
        }
        else if (当前玩家.家族 == 当前连接的战场.家族2 && 当前连接的战场.Boss归属 == 2)
        {
            Boss属于己方 = true;
        }

        // Boss属于己方时不能攻击，属于敌方或无归属时可以攻击
        return !Boss属于己方;
    }

    /// <summary>
    /// 输出敌方已加入玩家信息
    /// </summary>
    private void 输出敌方玩家信息()
    {
        if (当前连接的战场 == null || 当前玩家 == null) return;

        Debug.Log("=== 敌方玩家信息 ===");

        // 确定敌方家族
        家族信息库 敌方家族 = null;
        if (当前玩家.家族 == 当前连接的战场.家族1)
        {
            敌方家族 = 当前连接的战场.家族2;
            Debug.Log($"当前玩家属于家族1，敌方家族是：{敌方家族.家族名字}");
        }
        else if (当前玩家.家族 == 当前连接的战场.家族2)
        {
            敌方家族 = 当前连接的战场.家族1;
            Debug.Log($"当前玩家属于家族2，敌方家族是：{敌方家族.家族名字}");
        }
        else
        {
            Debug.LogWarning("当前玩家不属于任何参战家族");
            return;
        }

        // 获取敌方已加入战场的玩家对象列表
        List<玩家数据> 敌方已加入玩家列表 = 获取敌方已加入玩家列表(敌方家族);

        Debug.Log($"敌方家族 {敌方家族.家族名字} 已加入战场的玩家数量：{敌方已加入玩家列表.Count}");

        if (敌方已加入玩家列表.Count > 0)
        {
            Debug.Log("敌方已加入玩家详细信息：");
            for (int i = 0; i < 敌方已加入玩家列表.Count; i++)
            {
                var 敌方玩家 = 敌方已加入玩家列表[i];
                Debug.Log($"  {i + 1}. {敌方玩家.姓名} (等级: {敌方玩家.等级}, 攻击力: {敌方玩家.玩家属性.攻击力})");
            }
        }
        else
        {
            Debug.Log("敌方家族暂无玩家加入战场");
        }

        Debug.Log("=== 敌方玩家信息结束 ===");
    }

    /// <summary>
    /// 获取敌方已加入战场的玩家列表
    /// </summary>
    private List<玩家数据> 获取敌方已加入玩家列表(家族信息库 敌方家族)
    {
        List<玩家数据> 敌方已加入玩家列表 = new List<玩家数据>();

        if (当前连接的战场 == null || 敌方家族 == null) return 敌方已加入玩家列表;

        foreach (var 参战玩家 in 当前连接的战场.参战玩家列表)
        {
            if (参战玩家.家族 == 敌方家族)
            {
                敌方已加入玩家列表.Add(参战玩家);
            }
        }

        return 敌方已加入玩家列表;
    }

    /// <summary>
    /// 公共方法：获取当前敌方已加入战场的玩家列表
    /// </summary>
    public List<玩家数据> 获取当前敌方玩家列表()
    {
        if (当前连接的战场 == null || 当前玩家 == null || 当前玩家.家族 == null)
        {
            return new List<玩家数据>();
        }

        // 确定敌方家族
        家族信息库 敌方家族 = null;
        if (当前玩家.家族 == 当前连接的战场.家族1)
        {
            敌方家族 = 当前连接的战场.家族2;
        }
        else if (当前玩家.家族 == 当前连接的战场.家族2)
        {
            敌方家族 = 当前连接的战场.家族1;
        }

        return 获取敌方已加入玩家列表(敌方家族);
    }

    /// <summary>
    /// 点击攻击Boss
    /// </summary>
    public void 点击攻击Boss()
    {
        if (Boss攻击冷却中)
        {
            通用提示框.显示("Boss攻击冷却中，请稍后再试！");
            return;
        }

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

        // 检查是否可以攻击Boss
        if (!判断是否可以攻击Boss())
        {
            if (当前连接的战场.Boss归属 == 0)
            {
                通用提示框.显示("Boss暂无归属，无法攻击！");
            }
            else
            {
                通用提示框.显示("Boss已属于你的家族，无法攻击！");
            }
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

        // 开始Boss攻击冷却
        开始Boss攻击冷却();
    }

    /// <summary>
    /// 开始Boss攻击冷却
    /// </summary>
    private void 开始Boss攻击冷却()
    {
        if (Boss攻击冷却协程 != null)
        {
            StopCoroutine(Boss攻击冷却协程);
        }

        Boss攻击冷却协程 = StartCoroutine(Boss攻击冷却倒计时());
    }

    /// <summary>
    /// Boss攻击冷却倒计时协程
    /// </summary>
    private IEnumerator Boss攻击冷却倒计时()
    {
        Boss攻击冷却中 = true;
        Debug.Log($"Boss攻击进入冷却，{Boss攻击冷却时间}秒后可再次攻击");

        yield return new WaitForSeconds(Boss攻击冷却时间);

        Boss攻击冷却中 = false;
        Debug.Log("Boss攻击冷却结束，可以再次攻击");

        // 立即更新按钮状态
        更新Boss攻击按钮状态();
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

            // 停止Boss攻击冷却协程
            if (Boss攻击冷却协程 != null)
            {
                StopCoroutine(Boss攻击冷却协程);
                Boss攻击冷却协程 = null;
            }
            Boss攻击冷却中 = false;

            // 关闭界面
            gameObject.SetActive(false);
        }
    }
}