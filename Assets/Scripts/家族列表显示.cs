using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using 玩家数据结构;
using 国家系统;

public class 家族列表显示 : MonoBehaviour
{
    public GameObject 无家族界面;
    public GameObject 有家族界面;
    public Button 创建家族按钮;
    public Button 加入家族按钮;

    public 创建家族脚本 创建家族脚本;
    public void OnEnable()
    {
        刷新显示();
    }

    public void 刷新显示()
    {
        玩家数据 当前玩家 = 全局变量.所有玩家数据表[全局变量.当前身份];
        国家信息库 当前国家 = 当前玩家.国家;
        if (当前国家.家族成员表.Count <= 0)
        {
            无家族界面.gameObject.SetActive(true);
            有家族界面.gameObject.SetActive(false);
            创建家族按钮.gameObject.SetActive(true);
            加入家族按钮.gameObject.SetActive(false);
        }
        else
        {
            无家族界面.gameObject.SetActive(false);
            有家族界面.gameObject.SetActive(true);
            创建家族按钮.gameObject.SetActive(false);
            加入家族按钮.gameObject.SetActive(true);

        }
    }

    public void 点击创建家族()
    {
        创建家族脚本.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
