using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using 玩家数据结构;

public class 创建角色脚本 : MonoBehaviour
{
    public InputField 角色名输入框;
    private string 游戏场景名称 = "主界面";
    public 显示国家列表 显示国家列表;
    public 通用提示框 通用提示框;
    private void Start()
    {
        全局方法类.初始化国家();
    }
    public void 创建角色()
    {
        if (角色名输入框.text == null || 角色名输入框.text == "" || 角色名输入框.text == " " || 角色名输入框.text == "  " || 角色名输入框.text == "   ")
        {
            通用提示框.提示文本.text = "请输入正确角色名!";
            通用提示框.gameObject.SetActive(true);
            return;
        }

        if (角色名输入框.text.Length > 4)
        {
            通用提示框.提示文本.text = "角色名不可超过4个字";
            通用提示框.gameObject.SetActive(true);
            return;
        }

        全局变量.所有玩家数据表.Add(new 玩家数据(角色名输入框.text));
        全局方法类.初始化AI玩家();
        Debug.Log("点击创建角色");
        //SceneManager.LoadScene(游戏场景名称);
        this.gameObject.SetActive(false);
        显示国家列表.gameObject.SetActive(true);
    }

    
}
