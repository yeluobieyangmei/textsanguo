using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class 通用说明弹窗 : MonoBehaviour
{
    [Header("UI组件")]
    public Text 标题文本;
    public Text 内容文本;
    public Text 确定按钮文本;
    public Button 确定按钮;
    public Button 返回按钮;
    public GameObject 弹窗主体;
    
    // 静态实例，方便全局调用
    public static 通用说明弹窗 实例;
    
    // 确定按钮的回调事件
    private UnityAction 确定回调;
    
    private void Awake()
    {
        // 设置单例
        if (实例 == null)
        {
            实例 = this;
            DontDestroyOnLoad(gameObject); // 防止场景切换时销毁
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        // 确保按钮事件正确绑定
        初始化按钮事件();
    }
    
    private void 初始化按钮事件()
    {
        if (确定按钮 != null)
        {
            确定按钮.onClick.RemoveAllListeners();
            确定按钮.onClick.AddListener(确定);
        }
        
        if (返回按钮 != null)
        {
            返回按钮.onClick.RemoveAllListeners();
            返回按钮.onClick.AddListener(返回);
        }
    }

    public void 显示弹窗(string 标题, string 内容, string 确定按钮标题 = "确定", UnityAction 确定回调方法 = null)
    {
        // 设置文本内容
        if (标题文本 != null) 标题文本.text = 标题;
        if (内容文本 != null) 内容文本.text = 内容;
        if (确定按钮文本 != null) 确定按钮文本.text = 确定按钮标题;
        
        // 保存确定回调
        确定回调 = 确定回调方法;
        
        // 重新初始化按钮事件（确保事件正确绑定）
        初始化按钮事件();

        // 显示弹窗
        弹窗主体.gameObject.SetActive(true);
    }
    
    /// <summary>
    /// 静态方法，方便全局调用
    /// </summary>
    public static void 显示(string 标题, string 内容, string 确定按钮标题 = "确定", UnityAction 确定回调方法 = null)
    {
        if (实例 != null)
        {
            实例.显示弹窗(标题, 内容, 确定按钮标题, 确定回调方法);
        }
        else
        {
            Debug.LogError("通用说明弹窗实例未找到！请确保场景中有通用说明弹窗对象。");
        }
    }

    public void 确定()
    {
        // 执行确定回调
        if (确定回调 != null)
        {
            try
            {
                确定回调.Invoke();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"通用说明弹窗: 执行确定回调时发生错误: {e.Message}");
            }
        }
        else
        {
            Debug.Log("通用说明弹窗: 没有设置确定回调方法");
        }
        
        // 关闭弹窗
        关闭弹窗();
    }
    
    public void 返回()
    {
        关闭弹窗();
    }
    
    private void 关闭弹窗()
    {
        // 清空回调
        确定回调 = null;

        // 隐藏弹窗
        弹窗主体.gameObject.SetActive(false);
    }
    
    /// <summary>
    /// 检查组件是否正确设置
    /// </summary>
    private void OnValidate()
    {
        if (标题文本 == null) Debug.LogWarning("通用说明弹窗: 标题文本组件未设置");
        if (内容文本 == null) Debug.LogWarning("通用说明弹窗: 内容文本组件未设置");
        if (确定按钮文本 == null) Debug.LogWarning("通用说明弹窗: 确定按钮文本组件未设置");
        if (确定按钮 == null) Debug.LogWarning("通用说明弹窗: 确定按钮组件未设置");
        if (返回按钮 == null) Debug.LogWarning("通用说明弹窗: 返回按钮组件未设置");
    }
}
