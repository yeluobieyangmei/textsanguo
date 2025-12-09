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
    public Text 战场倒计时显示;

    [Header("用于成员列表显示")]
    public Transform 父对象;
    public GameObject 要克隆的对象;
    List<GameObject> 克隆池 = new List<GameObject>();
    玩家数据 当前选中玩家 = null;

    private 战场实例 当前连接的战场;
    private 玩家数据 当前玩家;
    private List<玩家数据> 已加入战场敌方玩家列表 = null;

    private float 上次刷新时间 = 0f;
    private float 刷新间隔 = 0.5f;
    private bool Boss攻击冷却中 = false;
    private float Boss攻击冷却时间 = 3f;
    private Coroutine Boss攻击冷却协程;

    public void 刷新玩家对象列表()
    {
        要克隆的对象.gameObject.SetActive(false);
        int count = 已加入战场敌方玩家列表.Count;

        foreach (var obj in 克隆池)
        {
            if (obj != null) Destroy(obj);
        }
        克隆池.Clear();

        for (int i = 0; i < count; i++)
        {
            GameObject 克隆对象 = Instantiate(要克隆的对象, 父对象);
            克隆对象.transform.GetChild(0).GetComponent<Text>().text = 已加入战场敌方玩家列表[i].姓名;
            克隆对象.gameObject.SetActive(true);
            克隆池.Add(克隆对象);

            Toggle t = 克隆对象.GetComponent<Toggle>();
            玩家数据 捕获玩家 = 已加入战场敌方玩家列表[i];
            t.onValueChanged.AddListener(isOn =>
            {
                if (isOn)
                {
                    当前选中玩家 = 捕获玩家;
                    Debug.Log($"选中敌方玩家: {当前选中玩家.姓名}");
                }
            });

            Button 攻击按钮 = 克隆对象.GetComponentInChildren<Button>();
            if (攻击按钮 != null)
            {
                玩家数据 按钮捕获玩家 = 已加入战场敌方玩家列表[i];
                攻击按钮.onClick.AddListener(() => 攻击指定玩家(按钮捕获玩家));
            }
        }

        // 移除：不再需要更新敌方玩家数量
    }

    private void Start()
    {
        当前玩家 = 全局变量.所有玩家数据表[全局变量.当前身份];

        if (攻击Boss按钮 != null)
            攻击Boss按钮.onClick.AddListener(点击攻击Boss);

        if (退出战场按钮 != null)
            退出战场按钮.onClick.AddListener(点击退出战场);
    }

    private IEnumerator 延迟自动连接()
    {
        yield return null;
        自动连接玩家战场();

        if (当前连接的战场 != null)
        {
            刷新战场显示();
            刷新玩家对象列表();
        }
    }

    private void OnEnable()
    {
        StartCoroutine(延迟自动连接());
    }

    private void Update()
    {
        if (当前连接的战场 != null)
        {
            刷新战场显示();
        }
        更新倒计时显示();
    }

    private void 自动连接玩家战场()
    {
        if (当前玩家 == null || 当前玩家.家族 == null) return;

        if (战场管理器.Instance != null)
        {
            战场实例 玩家战场 = 战场管理器.Instance.获取玩家战场(当前玩家);
            if (玩家战场 != null && 玩家战场.战场状态 == 战场状态.进行中)
            {
                连接战场实例(玩家战场);
            }
        }
    }

    public void 连接战场实例(战场实例 战场)
    {
        当前连接的战场 = 战场;
        if (战场 != null)
        {
            已加入战场敌方玩家列表 = 获取当前敌方玩家列表();
            刷新战场显示();
        }
    }

    private void 刷新战场显示()
    {
        if (当前连接的战场 == null) return;

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

        if (家族1名字 != null)
            家族1名字.text = 当前连接的战场.家族1.家族名字;
        if (家族2名字 != null)
            家族2名字.text = 当前连接的战场.家族2.家族名字;
        if (家族1积分 != null)
            家族1积分.text = 当前连接的战场.家族1积分.ToString();
        if (家族2积分 != null)
            家族2积分.text = 当前连接的战场.家族2积分.ToString();
    }

    private void 更新Boss攻击按钮状态()
    {
        if (攻击Boss按钮 != null && 当前连接的战场 != null && 当前玩家 != null)
        {
            bool 可以攻击Boss = 判断是否可以攻击Boss();
            攻击Boss按钮.interactable = !Boss攻击冷却中 && 可以攻击Boss;
        }
    }

    private bool 判断是否可以攻击Boss()
    {
        if (当前连接的战场 == null || 当前玩家 == null || 当前玩家.家族 == null)
            return false;

        if (当前连接的战场.Boss归属 == 0) return true;

        bool Boss属于己方 = false;
        if (当前玩家.家族 == 当前连接的战场.家族1 && 当前连接的战场.Boss归属 == 1)
            Boss属于己方 = true;
        else if (当前玩家.家族 == 当前连接的战场.家族2 && 当前连接的战场.Boss归属 == 2)
            Boss属于己方 = true;

        return !Boss属于己方;
    }

    public List<玩家数据> 获取当前敌方玩家列表()
    {
        if (当前连接的战场 == null || 当前玩家 == null || 当前玩家.家族 == null)
            return new List<玩家数据>();

        家族信息库 敌方家族 = null;
        if (当前玩家.家族 == 当前连接的战场.家族1)
            敌方家族 = 当前连接的战场.家族2;
        else if (当前玩家.家族 == 当前连接的战场.家族2)
            敌方家族 = 当前连接的战场.家族1;

        List<玩家数据> 敌方已加入玩家列表 = new List<玩家数据>();
        if (当前连接的战场 != null && 敌方家族 != null)
        {
            foreach (var 参战玩家 in 当前连接的战场.参战玩家列表)
            {
                if (参战玩家.家族 == 敌方家族 && 参战玩家.玩家属性.当前生命值 > 0)
                {
                    敌方已加入玩家列表.Add(参战玩家);
                }
            }
        }
        return 敌方已加入玩家列表;
    }

    public void 点击攻击Boss()
    {
        if (Boss攻击冷却中 || 当前连接的战场 == null) return;

        int 攻击伤害 = 当前玩家.玩家属性.攻击力 * Random.Range(80, 121) / 100;
        bool Boss被击败 = 当前连接的战场.攻击Boss(当前玩家, 攻击伤害);

        通用提示框.显示($"对Boss造成 {攻击伤害} 点伤害！");
        if (Boss被击败) 通用提示框.显示("Boss被击败！你的家族获得了Boss归属权！");

        StartCoroutine(Boss攻击冷却倒计时());
    }

    private IEnumerator Boss攻击冷却倒计时()
    {
        Boss攻击冷却中 = true;
        yield return new WaitForSeconds(Boss攻击冷却时间);
        Boss攻击冷却中 = false;
        更新Boss攻击按钮状态();
    }

    public void 点击退出战场()
    {
        if (当前连接的战场 == null) return;

        if (战场管理器.Instance.玩家退出战场(当前连接的战场.战场ID, 当前玩家))
        {
            通用提示框.显示("已退出战场");
            gameObject.SetActive(false);
        }
    }

    public void 攻击指定玩家(玩家数据 要攻击的玩家)
    {
        if (要攻击的玩家 == null || 当前连接的战场 == null) return;

        PVP攻击结果 攻击结果 = 战斗系统.玩家攻击玩家(当前玩家, 要攻击的玩家);
        if (!攻击结果.攻击成功) return;

        string 结果信息 = $"攻击 {要攻击的玩家.姓名}，造成 {攻击结果.伤害值} 点伤害！";
        if (攻击结果.目标死亡)
        {
            结果信息 += $" {要攻击的玩家.姓名} 被击败了！本家族获得 10 积分！";
            添加家族积分(当前玩家, 10);
            刷新玩家对象列表();
        }
        通用提示框.显示(结果信息);
        战斗系统.测试当前玩家死亡();
    }

    private void 添加家族积分(玩家数据 玩家, int 积分)
    {
        if (当前连接的战场 == null || 玩家?.家族 == null) return;

        if (玩家.家族 == 当前连接的战场.家族1)
            当前连接的战场.家族1积分 += 积分;
        else if (玩家.家族 == 当前连接的战场.家族2)
            当前连接的战场.家族2积分 += 积分;
    }

    /// <summary>
    /// 更新倒计时显示（从战场实例中读取数据）
    /// </summary>
    private void 更新倒计时显示()
    {
        if (战场倒计时显示 == null || 当前连接的战场 == null) return;
        
        // 从战场实例中读取倒计时数据
        if (!当前连接的战场.倒计时进行中)
        {
            战场倒计时显示.text = "战场未开始";
            return;
        }

        float 当前剩余时间 = 当前连接的战场.当前剩余时间;
        bool 是否加时阶段 = 当前连接的战场.是否加时阶段;
        
        int 分钟 = Mathf.FloorToInt(当前剩余时间 / 60);
        int 秒数 = Mathf.FloorToInt(当前剩余时间 % 60);
        string 倒计时文本 = $"{分钟:D2}:{秒数:D2}";

        if (是否加时阶段)
        {
            倒计时文本 = $"加时 {倒计时文本}";
            战场倒计时显示.color = Color.red; // 加时阶段显示红色
        }
        else
        {
            倒计时文本 = $"剩余 {倒计时文本}";
            
            // 根据剩余时间改变颜色
            if (当前剩余时间 <= 300) // 最后5分钟
            {
                战场倒计时显示.color = Color.red;
            }
            else if (当前剩余时间 <= 600) // 最后10分钟
            {
                战场倒计时显示.color = Color.yellow;
            }
            else
            {
                战场倒计时显示.color = Color.white;
            }
        }

        战场倒计时显示.text = 倒计时文本;
    }
}