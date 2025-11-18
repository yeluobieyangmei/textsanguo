using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using 玩家数据结构;
using 国家系统;

public class 国家功能显示 : MonoBehaviour
{
    public 建国脚本 建国脚本;
    public 更换国家脚本 更换国家脚本;
    public Text 国名;
    public Text 国王;
    public Text 大都督;
    public Text 丞相;
    public Text 成员;
    public Text 排名;
    public Text 科技;
    public Button 建国按钮;
    public Button 换国按钮;

    private void OnEnable()
    {
        刷新显示();
    }
    public void 刷新显示()
    {
        玩家数据 当前玩家 = 全局变量.所有玩家数据表[全局变量.当前身份];
        国家信息库 当前国家 = 当前玩家.国家;
        国名.text = $"国 名：{当前国家.国名}({当前国家.国号})";
        国王.text = $"国 王：{全局变量.所有玩家数据表[当前国家.国王ID].姓名}";
        大都督.text = 当前国家.大都督ID == -1 ? "大都督：无" : $"大都督：{全局变量.所有玩家数据表[当前国家.大都督ID].姓名}";
        丞相.text = 当前国家.丞相ID == -1 ? "丞 相：无" : $"丞 相：{全局变量.所有玩家数据表[当前国家.丞相ID].姓名}";
        建国按钮.gameObject.SetActive(全局变量.所有玩家数据表[全局变量.当前身份].官职 != "国王");
        换国按钮.gameObject.SetActive(全局变量.所有玩家数据表[全局变量.当前身份].官职 != "国王");
    }

    public void 点击建国按钮()
    {
        建国脚本.gameObject.SetActive(true);
    }

    public void 点击换国按钮()
    {
        更换国家脚本.gameObject.SetActive(true);
    }
}
