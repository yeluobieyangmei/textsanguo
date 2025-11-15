using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using 玩家数据结构;

public class 主场景界面显示 : MonoBehaviour
{
    public Image 性别图片;
    public Text 角色信息;
    public Text 家族信息;
    public Text 国家信息;
    public Text 铜钱信息;
    public Text 黄金信息;
    public Text 版本号;

    public void Update()
    {
        刷新显示();
    }
    public void 刷新显示()
    {
        玩家数据 玩家 = 全局变量.所有玩家数据表[0];
        this.角色信息.text = $"Lv.{玩家.等级} {玩家.姓名}(0.00%)";
        this.家族信息.text = $"家族:暂无";
        this.国家信息.text = $"国家:暂无";
        this.铜钱信息.text = $"铜钱:{玩家.铜钱}";
        this.黄金信息.text = $"黄金:{玩家.黄金}";
        this.版本号.text = $"1.0.0";
    }
}
