using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using 国家系统;
using 玩家数据结构;

public class 显示国家列表 : MonoBehaviour
{
    [Header("UI引用")]
    /// <summary>
    /// 国家对象模板（需要克隆的对象）
    /// </summary>
    public GameObject 国家对象模板;

    /// <summary>
    /// 国家列表父对象（克隆的对象将添加到这里）
    /// </summary>
    public Transform 国家列表父对象;

    [Header("模板对象中的Text组件引用（从模板对象中拖拽赋值）")]
    /// <summary>
    /// 模板对象中的国号Text组件
    /// </summary>
    public Text 模板国号文本;

    /// <summary>
    /// 模板对象中的国名Text组件
    /// </summary>
    public Text 模板国名文本;

    [Header("按钮引用")]
    /// <summary>
    /// 加入国家按钮
    /// </summary>
    public Button 加入国家按钮;

    /// <summary>
    /// 存储已创建的国家对象列表，用于刷新时清理
    /// </summary>
    private List<GameObject> 已创建的国家对象列表 = new List<GameObject>();

    /// <summary>
    /// 存储国家对象和对应的国家信息的映射关系
    /// </summary>
    private Dictionary<GameObject, 国家信息库> 国家对象映射 = new Dictionary<GameObject, 国家信息库>();

    /// <summary>
    /// 当前选中的国家
    /// </summary>
    private 国家信息库 当前选中的国家 = null;

    /// <summary>
    /// 当前选中的国家对象（用于高亮显示）
    /// </summary>
    private GameObject 当前选中的国家对象 = null;

    void Start()
    {
        // 绑定加入国家按钮的点击事件
        if (加入国家按钮 != null)
        {
            加入国家按钮.onClick.AddListener(点击加入国家);
        }
        else
        {
            Debug.LogWarning("加入国家按钮未设置！");
        }
        
        // 延迟刷新，确保国家初始化完成
        StartCoroutine(延迟刷新国家列表());
    }

    /// <summary>
    /// 延迟刷新国家列表，确保国家初始化完成
    /// </summary>
    private IEnumerator 延迟刷新国家列表()
    {
        // 等待一帧，确保所有初始化完成
        yield return null;
        
        // 如果国家列表为空，尝试初始化
        if (全局变量.所有国家列表 == null || 全局变量.所有国家列表.Count == 0)
        {
            Debug.LogWarning("国家列表为空，尝试初始化国家...");
            全局方法类.初始化国家();
            yield return null;  // 再等待一帧
        }
        
        刷新国家列表();
    }

    /// <summary>
    /// 延迟更新布局
    /// </summary>
    private IEnumerator 延迟更新布局(RectTransform 父RectTransform)
    {
        yield return null;  // 等待一帧
        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(父RectTransform);
        Canvas.ForceUpdateCanvases();
    }

    /// <summary>
    /// 刷新国家列表显示
    /// </summary>
    public void 刷新国家列表()
    {
        // 清理之前创建的对象
        清理已创建的对象();

        // 检查必要的引用
        if (国家对象模板 == null)
        {
            Debug.LogError("国家对象模板未设置！请在Inspector中设置。");
            return;
        }

        if (国家列表父对象 == null)
        {
            Debug.LogError("国家列表父对象未设置！请在Inspector中设置。");
            return;
        }

        if (模板国号文本 == null)
        {
            Debug.LogError("模板国号文本未设置！请从模板对象中拖拽国号Text组件。");
            return;
        }

        if (模板国名文本 == null)
        {
            Debug.LogError("模板国名文本未设置！请从模板对象中拖拽国名Text组件。");
            return;
        }

        // 遍历所有国家列表
        int 国家数量 = 全局变量.所有国家列表.Count;
        Debug.Log($"开始显示国家列表，共有 {国家数量} 个国家");
        
        // 详细输出每个国家的信息
        for (int j = 0; j < 国家数量; j++)
        {
            Debug.Log($"国家[{j}]: {全局变量.所有国家列表[j].国名}({全局变量.所有国家列表[j].国号})");
        }

        // 获取模板中Text组件的路径（相对于模板对象）
        string 国号路径 = 获取相对路径(国家对象模板.transform, 模板国号文本.transform);
        string 国名路径 = 获取相对路径(国家对象模板.transform, 模板国名文本.transform);

        for (int i = 0; i < 国家数量; i++)
        {
            国家信息库 国家 = 全局变量.所有国家列表[i];
            
            if (国家 == null)
            {
                Debug.LogError($"国家列表索引 {i} 的国家对象为 null！");
                continue;
            }
            
            // 克隆国家对象
            GameObject 克隆对象 = Instantiate(国家对象模板, 国家列表父对象);
            克隆对象.SetActive(true);  // 确保克隆的对象是激活的
            
            // 确保克隆对象有正确的名称（用于调试）
            克隆对象.name = $"国家对象_{国家.国名}_{i}";

            // 通过路径在克隆对象中找到对应的Text组件并设置文本
            Transform 克隆国号对象 = 克隆对象.transform.Find(国号路径);
            if (克隆国号对象 != null)
            {
                Text 克隆国号文本 = 克隆国号对象.GetComponent<Text>();
                if (克隆国号文本 != null)
                {
                    克隆国号文本.text = 国家.国号;
                }
            }

            Transform 克隆国名对象 = 克隆对象.transform.Find(国名路径);
            if (克隆国名对象 != null)
            {
                Text 克隆国名文本 = 克隆国名对象.GetComponent<Text>();
                if (克隆国名文本 != null)
                {
                    克隆国名文本.text = 国家.国名;
                }
            }

            // 将克隆的对象添加到列表中
            已创建的国家对象列表.Add(克隆对象);
            
            // 存储国家对象和国家的映射关系
            国家对象映射[克隆对象] = 国家;

            // 检查是否有Toggle组件
            Toggle 国家Toggle = 克隆对象.GetComponent<Toggle>();
            if (国家Toggle != null)
            {
                // 如果有Toggle组件，使用Toggle的onValueChanged事件
                国家Toggle.onValueChanged.AddListener((bool 是否选中) => 
                {
                    if (是否选中)
                    {
                        选择国家(克隆对象, 国家);
                    }
                });
            }
            else
            {
                // 如果没有Toggle组件，使用Button组件
                Button 国家按钮 = 克隆对象.GetComponent<Button>();
                if (国家按钮 == null)
                {
                    国家按钮 = 克隆对象.AddComponent<Button>();
                }

                // 绑定点击事件
                int 索引 = i;  // 捕获循环变量
                国家按钮.onClick.AddListener(() => 选择国家(克隆对象, 国家));
            }

            Debug.Log($"已创建国家对象[{i}]：{国家.国名}({国家.国号})，对象名称：{克隆对象.name}，位置：{克隆对象.transform.localPosition}");
        }

        // 强制更新整个布局（在循环外统一更新，避免性能问题）
        if (国家列表父对象 is RectTransform)
        {
            RectTransform 父RectTransform = 国家列表父对象 as RectTransform;
            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(父RectTransform);
            
            // 等待一帧后再次更新，确保布局正确
            StartCoroutine(延迟更新布局(父RectTransform));
        }
        
        Canvas.ForceUpdateCanvases();  // 强制更新Canvas

        Debug.Log($"国家列表刷新完成，共显示 {已创建的国家对象列表.Count} 个国家");
        Debug.Log($"国家列表父对象的子对象数量：{国家列表父对象.childCount}");
    }

    /// <summary>
    /// 清理已创建的国家对象
    /// </summary>
    private void 清理已创建的对象()
    {
        for (int i = 已创建的国家对象列表.Count - 1; i >= 0; i--)
        {
            if (已创建的国家对象列表[i] != null)
            {
                Destroy(已创建的国家对象列表[i]);
            }
        }
        已创建的国家对象列表.Clear();
        国家对象映射.Clear();
        当前选中的国家 = null;
        当前选中的国家对象 = null;
    }

    /// <summary>
    /// 当对象启用时刷新列表
    /// </summary>
    void OnEnable()
    {
        // 延迟刷新，避免在初始化完成前执行
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(延迟刷新国家列表());
        }
    }

    /// <summary>
    /// 获取子对象相对于父对象的路径
    /// </summary>
    /// <param name="父对象">父对象Transform</param>
    /// <param name="子对象">子对象Transform</param>
    /// <returns>相对路径字符串</returns>
    private string 获取相对路径(Transform 父对象, Transform 子对象)
    {
        if (子对象 == null || 父对象 == null)
        {
            return "";
        }

        // 如果子对象就是父对象，返回空字符串
        if (子对象 == 父对象)
        {
            return "";
        }

        // 构建路径
        string 路径 = 子对象.name;
        Transform 当前 = 子对象.parent;

        while (当前 != null && 当前 != 父对象)
        {
            路径 = 当前.name + "/" + 路径;
            当前 = 当前.parent;
        }

        return 路径;
    }

    /// <summary>
    /// 选择国家
    /// </summary>
    /// <param name="国家对象">被点击的国家对象</param>
    /// <param name="国家">对应的国家信息</param>
    private void 选择国家(GameObject 国家对象, 国家信息库 国家)
    {
        // 取消之前选中对象的高亮（如果有的话）
        if (当前选中的国家对象 != null && 当前选中的国家对象 != 国家对象)
        {
            // 可以在这里添加取消高亮的逻辑，比如恢复原始颜色
            // 例如：当前选中的国家对象.GetComponent<Image>().color = Color.white;
        }

        // 设置当前选中的国家
        当前选中的国家 = 国家;
        当前选中的国家对象 = 国家对象;

        // 添加高亮效果（可选）
        // 例如：国家对象.GetComponent<Image>().color = Color.yellow;

        Debug.Log($"已选择国家：{国家.国名}({国家.国号})");
    }

    /// <summary>
    /// 点击加入国家按钮
    /// </summary>
    public void 点击加入国家()
    {
        if (当前选中的国家 == null)
        {
            Debug.LogWarning("请先选择一个国家！");
            return;
        }

        玩家数据 当前玩家 = 全局变量.所有玩家数据表[全局变量.当前身份];
        当前玩家.加入一个国家(当前选中的国家.国名);
        SceneManager.LoadScene(游戏场景名称);
    }

    private string 游戏场景名称 = "主界面";
}

