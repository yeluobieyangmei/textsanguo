using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using 玩家数据结构;

public class 创建角色脚本 : MonoBehaviour
{
    public InputField 角色名输入框;
    public 显示国家列表 显示国家列表;
    private void Start()
    {
        全局方法类.初始化国家();
    }
    public void 创建角色()
    {
        if (角色名输入框.text == null || 角色名输入框.text == "" || 角色名输入框.text == " " || 角色名输入框.text == "  " || 角色名输入框.text == "   ")
        {
            通用提示框.显示("请输入正确角色名!");
            return;
        }

        if (角色名输入框.text.Length > 4)
        {
            通用提示框.显示("角色名不可超过4个字!");
            return;
        }

        全局变量.所有玩家数据表.Add(new 玩家数据(角色名输入框.text));
        全局方法类.初始化AI玩家();
        //SceneManager.LoadScene(游戏场景名称);
        this.gameObject.SetActive(false);
        显示国家列表.gameObject.SetActive(true);
    }

    
}
