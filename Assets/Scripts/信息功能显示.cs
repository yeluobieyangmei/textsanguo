using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class 信息功能显示 : MonoBehaviour
{
    public Text 生命值文本;
    public Text 攻击力文本;
    public Text 防御力文本;
    public Text 当前称号文本;
    public Text 成就文本;
    public Text 家族文本;
    public Text 国家文本;

    public void OnEnable()
    {
        刷新显示();
    }
    public void 刷新显示()
    {
        生命值文本.text = $"生命值：{全局变量.所有玩家数据表[全局变量.当前身份].玩家属性.生命值}";
        攻击力文本.text = $"攻击力：{全局变量.所有玩家数据表[全局变量.当前身份].玩家属性.攻击力}";
        防御力文本.text = $"攻击力：{全局变量.所有玩家数据表[全局变量.当前身份].玩家属性.防御力}";
        Debug.Log("刷新功能显示");
    }
}
