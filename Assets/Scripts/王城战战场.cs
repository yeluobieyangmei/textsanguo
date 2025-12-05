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

    [Header("敌方成员列表")]
    public Transform 敌方成员列表父物体; // ScrollView的Content
    public GameObject 成员显示预制体; // 用于显示单个敌方成员的预制体

    [Header("战场控制")]
    public Button 退出战场按钮;

    // 私有变量
    private 战场实例 当前连接的战场;
    private List<GameObject> 已生成的成员对象 = new List<GameObject>();
    private 玩家数据 当前玩家;

    private void Awake()
    {
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

    private void OnEnable()
    {
        Debug.Log("王城战战场UI被激活");
        // 每次激活时尝试连接到当前玩家的战场
        自动连接玩家战场();

        // 如果已经连接到战场，强制刷新一次
        if (当前连接的战场 != null)
        {
            Debug.Log("强制刷新战场显示");
            刷新战场显示();
        }
    }

    private void Update()
    {
        // 实时刷新战场数据
        if (当前连接的战场 != null)
        {
            刷新战场显示();
            刷新敌方成员列表();
        }
    }

    /// <summary>
    /// 自动连接到当前玩家参与的战场
    /// </summary>
    private void 自动连接玩家战场()
    {
        if (战场管理器.Instance != null)
        {
            Debug.Log($"------------------------自动连接玩家战场里当前玩家是:{当前玩家.姓名}");
            战场实例 玩家战场 = 战场管理器.Instance.获取玩家战场(当前玩家);
            if (玩家战场 != null && 玩家战场.战场状态 == 战场状态.进行中)
            {
                连接战场实例(玩家战场);
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
            刷新敌方成员列表();
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
        if (当前连接的战场 == null)
        {
            Debug.LogWarning("当前连接的战场为空，无法刷新显示");
            return;
        }

        Debug.Log("开始刷新战场显示");

        // 更新Boss信息
        if (Boss血量 != null)
        {
            Boss血量.text = $"Boss当前血量：{当前连接的战场.Boss血量}";
            Debug.Log($"更新Boss血量: {当前连接的战场.Boss血量}");
        }
        else
        {
            Debug.LogError("Boss血量 Text组件未分配");
        }

        if (Boss归属 != null)
        {
            string 归属文本 = "无";
            if (当前连接的战场.Boss归属 == 1)
                归属文本 = 当前连接的战场.家族1.家族名字;
            else if (当前连接的战场.Boss归属 == 2)
                归属文本 = 当前连接的战场.家族2.家族名字;

            Boss归属.text = $"归属：{归属文本}";
            Debug.Log($"更新Boss归属: {归属文本}");
        }

        // 更新家族信息
        if (家族1名字 != null)
        {
            家族1名字.text = 当前连接的战场.家族1.家族名字;
            Debug.Log($"更新家族1名字: {当前连接的战场.家族1.家族名字}");
        }
        else
        {
            Debug.LogError("家族1名字 Text组件未分配");
        }

        if (家族2名字 != null)
        {
            家族2名字.text = 当前连接的战场.家族2.家族名字;
            Debug.Log($"更新家族2名字: {当前连接的战场.家族2.家族名字}");
        }
        else
        {
            Debug.LogError("家族2名字 Text组件未分配");
        }

        if (家族1积分 != null)
        {
            家族1积分.text = 当前连接的战场.家族1积分.ToString();
            Debug.Log($"更新家族1积分: {当前连接的战场.家族1积分}");
        }

        if (家族2积分 != null)
        {
            家族2积分.text = 当前连接的战场.家族2积分.ToString();
            Debug.Log($"更新家族2积分: {当前连接的战场.家族2积分}");
        }
    }

    /// <summary>
    /// 刷新敌方成员列表显示
    /// </summary>
    private void 刷新敌方成员列表()
    {
        if (当前连接的战场 == null || 敌方成员列表父物体 == null) return;

        // 获取敌方家族成员（只显示已进入战场的）
        List<玩家数据> 敌方成员 = 当前连接的战场.获取对方家族成员(当前玩家);

        // 清理多余的显示对象
        while (已生成的成员对象.Count > 敌方成员.Count)
        {
            int 最后索引 = 已生成的成员对象.Count - 1;
            if (已生成的成员对象[最后索引] != null)
            {
                DestroyImmediate(已生成的成员对象[最后索引]);
            }
            已生成的成员对象.RemoveAt(最后索引);
        }

        // 创建或更新成员显示对象
        for (int i = 0; i < 敌方成员.Count; i++)
        {
            GameObject 成员对象;

            // 如果对象不存在，创建新的
            if (i >= 已生成的成员对象.Count)
            {
                if (成员显示预制体 != null)
                {
                    成员对象 = Instantiate(成员显示预制体, 敌方成员列表父物体);
                    已生成的成员对象.Add(成员对象);
                }
                else
                {
                    // 如果没有预制体，创建简单的文本显示
                    成员对象 = 创建简单成员显示();
                    已生成的成员对象.Add(成员对象);
                }
            }
            else
            {
                成员对象 = 已生成的成员对象[i];
            }

            // 更新成员信息显示
            更新成员显示信息(成员对象, 敌方成员[i]);
        }
    }

    /// <summary>
    /// 创建简单的成员显示对象（当没有预制体时）
    /// </summary>
    private GameObject 创建简单成员显示()
    {
        GameObject 成员对象 = new GameObject("敌方成员");
        成员对象.transform.SetParent(敌方成员列表父物体);

        // 添加布局组件
        RectTransform rect = 成员对象.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(300, 60);

        // 添加背景
        Image 背景 = 成员对象.AddComponent<Image>();
        背景.color = new Color(0.2f, 0.3f, 0.4f, 0.8f);

        // 添加成员名字文本
        GameObject 名字对象 = new GameObject("成员名字");
        名字对象.transform.SetParent(成员对象.transform);
        Text 名字文本 = 名字对象.AddComponent<Text>();
        名字文本.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        名字文本.fontSize = 16;
        名字文本.color = Color.white;
        名字文本.alignment = TextAnchor.MiddleLeft;

        RectTransform 名字Rect = 名字对象.GetComponent<RectTransform>();
        名字Rect.anchorMin = new Vector2(0, 0);
        名字Rect.anchorMax = new Vector2(0.7f, 1);
        名字Rect.offsetMin = new Vector2(10, 0);
        名字Rect.offsetMax = new Vector2(0, 0);

        // 添加攻击按钮
        GameObject 按钮对象 = new GameObject("攻击按钮");
        按钮对象.transform.SetParent(成员对象.transform);
        Button 攻击按钮 = 按钮对象.AddComponent<Button>();
        Image 按钮图片 = 按钮对象.AddComponent<Image>();
        按钮图片.color = new Color(0.8f, 0.2f, 0.2f, 1f);

        RectTransform 按钮Rect = 按钮对象.GetComponent<RectTransform>();
        按钮Rect.anchorMin = new Vector2(0.75f, 0.2f);
        按钮Rect.anchorMax = new Vector2(0.95f, 0.8f);
        按钮Rect.offsetMin = Vector2.zero;
        按钮Rect.offsetMax = Vector2.zero;

        // 按钮文字
        GameObject 按钮文字对象 = new GameObject("按钮文字");
        按钮文字对象.transform.SetParent(按钮对象.transform);
        Text 按钮文字 = 按钮文字对象.AddComponent<Text>();
        按钮文字.text = "攻击";
        按钮文字.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        按钮文字.fontSize = 14;
        按钮文字.color = Color.white;
        按钮文字.alignment = TextAnchor.MiddleCenter;

        RectTransform 按钮文字Rect = 按钮文字对象.GetComponent<RectTransform>();
        按钮文字Rect.anchorMin = Vector2.zero;
        按钮文字Rect.anchorMax = Vector2.one;
        按钮文字Rect.offsetMin = Vector2.zero;
        按钮文字Rect.offsetMax = Vector2.zero;

        return 成员对象;
    }

    /// <summary>
    /// 更新成员显示信息
    /// </summary>
    private void 更新成员显示信息(GameObject 成员对象, 玩家数据 成员数据)
    {
        if (成员对象 == null || 成员数据 == null) return;

        // 直接通过路径访问第一个子对象的Text组件来更新成员名字
        if (成员对象.transform.childCount > 0)
        {
            Text 名字文本 = 成员对象.transform.GetChild(0).GetComponent<Text>();
            if (名字文本 != null)
            {
                名字文本.text = $"{成员数据.姓名} (等级{成员数据.等级})";
            }
        }

        // 如果有第二个子对象作为攻击按钮，也可以用类似方式访问
        if (成员对象.transform.childCount > 1)
        {
            Button 攻击按钮 = 成员对象.transform.GetChild(1).GetComponent<Button>();
            if (攻击按钮 != null)
            {
                攻击按钮.onClick.RemoveAllListeners();
                攻击按钮.onClick.AddListener(() => 攻击玩家(成员数据));
            }
        }
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
    /// 攻击指定玩家
    /// </summary>
    private void 攻击玩家(玩家数据 目标玩家)
    {
        if (当前连接的战场 == null || 目标玩家 == null) return;

        // 检查是否可以攻击
        if (目标玩家.家族 == 当前玩家.家族)
        {
            通用提示框.显示("不能攻击同家族成员！");
            return;
        }

        // 计算攻击伤害和积分奖励
        int 攻击伤害 = 当前玩家.玩家属性.攻击力 * Random.Range(90, 111) / 100;
        int 获得积分 = Random.Range(10, 31); // 10-30积分

        // 给攻击者家族加积分
        if (当前玩家.家族 == 当前连接的战场.家族1)
        {
            当前连接的战场.家族1积分 += 获得积分;
        }
        else if (当前玩家.家族 == 当前连接的战场.家族2)
        {
            当前连接的战场.家族2积分 += 获得积分;
        }

        通用提示框.显示($"攻击 {目标玩家.姓名}，造成 {攻击伤害} 伤害，获得 {获得积分} 积分！");

        Debug.Log($"{当前玩家.姓名} 攻击了 {目标玩家.姓名}，获得 {获得积分} 积分");
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
            清理成员列表();

            // 关闭界面
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 清理成员列表显示
    /// </summary>
    private void 清理成员列表()
    {
        foreach (var 成员对象 in 已生成的成员对象)
        {
            if (成员对象 != null)
            {
                DestroyImmediate(成员对象);
            }
        }
        已生成的成员对象.Clear();
    }

    private void OnDisable()
    {
        // 界面关闭时清理
        清理成员列表();
    }
}