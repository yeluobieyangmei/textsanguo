using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using 国家系统;
using 玩家数据结构;

public class 更换国家脚本 : MonoBehaviour
{
    [Header("国家列表父对象（国家列表显示/列表）")]
    public Transform 国家列表父对象;

    [Header("国家对象模板（列表下的那个“对象”，需要带Toggle）")]
    public GameObject 国家对象模板;

    [Header("模板对象中的Text组件引用")]
    public Text 模板国号文本;
    public Text 模板国名文本;

    [Header("可选：ToggleGroup，用于互斥选择")]
    public ToggleGroup toggleGroup;

    /// <summary>
    /// 当前选中的国家（直接用国家信息库）
    /// </summary>
    private 国家信息库 当前选中国家 = null;

    /// <summary>
    /// 记录克隆出来的国家对象，方便刷新时清理
    /// </summary>
    private readonly List<GameObject> 已创建国家对象列表 = new List<GameObject>();

    private void OnEnable()
    {
        刷新国家列表();
    }

    /// <summary>
    /// 刷新国家列表：根据全局变量.所有国家列表 克隆UI对象
    /// </summary>
    public void 刷新国家列表()
    {
        清理已创建的对象();

        if (国家对象模板 == null)
        {
            Debug.LogError("更换国家脚本：国家对象模板未设置！");
            return;
        }

        if (国家列表父对象 == null)
        {
            Debug.LogError("更换国家脚本：国家列表父对象未设置！");
            return;
        }

        if (模板国号文本 == null || 模板国名文本 == null)
        {
            Debug.LogError("更换国家脚本：模板国号文本或模板国名文本未设置！");
            return;
        }

        // 模板只当作预制用，不直接显示
        国家对象模板.SetActive(false);

        int 国家数量 = 全局变量.所有国家列表.Count;
        if (国家数量 == 0)
        {
            Debug.LogWarning("更换国家脚本：全局变量.所有国家列表 为空！");
            return;
        }

        // 计算模板中文本在层级中的相对路径，方便克隆后查找
        string 国号路径 = 获取相对路径(国家对象模板.transform, 模板国号文本.transform);
        string 国名路径 = 获取相对路径(国家对象模板.transform, 模板国名文本.transform);

        for (int i = 0; i < 国家数量; i++)
        {
            国家信息库 国家 = 全局变量.所有国家列表[i];
            if (国家 == null)
            {
                Debug.LogError($"更换国家脚本：索引 {i} 的国家为 null！");
                continue;
            }

            GameObject 克隆对象 = Instantiate(国家对象模板, 国家列表父对象);
            克隆对象.SetActive(true);
            克隆对象.name = $"国家对象_{国家.国名}_{i}";

            // 设置国号文本
            Transform 克隆国号对象 = 克隆对象.transform.Find(国号路径);
            if (克隆国号对象 != null)
            {
                Text 克隆国号文本 = 克隆国号对象.GetComponent<Text>();
                if (克隆国号文本 != null)
                {
                    克隆国号文本.text = 国家.国号;
                }
            }

            // 设置国名文本
            Transform 克隆国名对象 = 克隆对象.transform.Find(国名路径);
            if (克隆国名对象 != null)
            {
                Text 克隆国名文本 = 克隆国名对象.GetComponent<Text>();
                if (克隆国名文本 != null)
                {
                    克隆国名文本.text = 国家.国名;
                }
            }

            已创建国家对象列表.Add(克隆对象);

            // 处理 Toggle 选择逻辑
            Toggle t = 克隆对象.GetComponent<Toggle>();
            if (t == null)
            {
                t = 克隆对象.AddComponent<Toggle>();
            }

            if (toggleGroup != null)
            {
                t.group = toggleGroup;
            }

            国家信息库 捕获国家 = 国家; // 闭包捕获
            t.onValueChanged.AddListener(isOn =>
            {
                if (isOn)
                {
                    当前选中国家 = 捕获国家;
                    Debug.Log($"更换国家脚本：选中国家 {捕获国家.国名}({捕获国家.国号})");
                }
            });
        }
    }

    /// <summary>
    /// 点击“更换国家”按钮时调用
    /// </summary>
    public void 更换国家()
    {
        if (当前选中国家 == null)
        {
            Debug.LogWarning("更换国家脚本：尚未选择任何国家！");
            return;
        }

        玩家数据 当前玩家 = 全局变量.所有玩家数据表[全局变量.当前身份];
        // 使用已有的加入一个国家逻辑，内部会处理成员表等
        当前玩家.加入一个国家(当前选中国家.国名);

        Debug.Log($"更换国家脚本：已将玩家 {当前玩家.姓名} 的国家更换为 {当前选中国家.国名}({当前选中国家.国号})");
    }

    /// <summary>
    /// 清理之前克隆出来的对象
    /// </summary>
    private void 清理已创建的对象()
    {
        for (int i = 已创建国家对象列表.Count - 1; i >= 0; i--)
        {
            if (已创建国家对象列表[i] != null)
            {
                Destroy(已创建国家对象列表[i]);
            }
        }
        已创建国家对象列表.Clear();
        当前选中国家 = null;
    }

    /// <summary>
    /// 获取子对象相对于父对象的路径（用于克隆后查找相同层级结构）
    /// </summary>
    private string 获取相对路径(Transform 父对象, Transform 子对象)
    {
        if (子对象 == null || 父对象 == null)
        {
            return string.Empty;
        }

        if (子对象 == 父对象)
        {
            return string.Empty;
        }

        string 路径 = 子对象.name;
        Transform 当前 = 子对象.parent;

        while (当前 != null && 当前 != 父对象)
        {
            路径 = 当前.name + "/" + 路径;
            当前 = 当前.parent;
        }

        return 路径;
    }
}
