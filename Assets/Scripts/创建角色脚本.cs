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

    private void Start()
    {
        全局方法类.初始化国家();
    }
    public void 创建角色()
    {
        //全局方法类.初始化国家();
        全局变量.所有玩家数据表.Add(new 玩家数据(角色名输入框.text));
        Debug.Log("点击创建角色");
        //SceneManager.LoadScene(游戏场景名称);
        this.gameObject.SetActive(false);
        显示国家列表.gameObject.SetActive(true);
    }

    
}
