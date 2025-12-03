using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using 玩家数据结构;
using 怪物数据结构;

public class 攻击对象查看 : MonoBehaviour
{
    public 怪物数据 当前怪物 { get; set; }
    public Text 标题;
    public Text 对象名字;
    public Text 对象详情;
    public void OnEnable()
    {
        对象信息显示();
    }
    public void 对象信息显示()
    {
        标题.text = "怪物信息";
        对象名字.text = $"LV.{当前怪物.等级} {当前怪物.名称}";
        对象详情.text = $"生命值：{当前怪物.属性.当前生命值}\n" +
            $"攻击力：{当前怪物.属性.攻击力}\n" +
            $"防御力：{当前怪物.属性.防御力}\n" +
            $"铜钱：{当前怪物.铜钱}\n经验：{当前怪物.经验值}";
    }

    public void 开始战斗()
    {
        if (战斗系统.开始战斗(全局变量.所有玩家数据表[全局变量.当前身份], 当前怪物))
        {
            通用提示框.显示("战斗胜利！");
        }
        else
        {
            通用提示框.显示("战斗失败！");
        }
    }
}
