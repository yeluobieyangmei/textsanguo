using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using 玩家数据结构;
using 国家系统;

public class 创建家族脚本 : MonoBehaviour
{
    public InputField 家族名输入框;
    public InputField 家族公告输入框;
    public 家族信息显示 家族信息显示;

    public void 创建家族()
    {
        if (家族名输入框.text.Length > 4)
        {
            通用提示框.显示("名称不可超过4字!");
            return;
        }
        if (家族名输入框.text == null || 家族名输入框.text == "" || 家族名输入框.text == " " || 家族名输入框.text == "  " || 家族名输入框.text == "   ")
        {
            通用提示框.显示("请正确输入名称!");
            return;
        }
        玩家数据 当前玩家 = 全局变量.所有玩家数据表[全局变量.当前身份];
        国家信息库 当前国家 = 当前玩家.国家;
        if (当前国家.家族成员表.Exists(f => f.家族名字 == 家族名输入框.text))
        {
            通用提示框.显示("名称已被占用!");
            return;
        }
        家族信息库 家族信息库 = new 家族信息库();
        家族信息库.创建一个家族(家族名输入框.text, 当前玩家);
        当前国家.家族成员表.Add(家族信息库);
        全局变量.所有家族列表.Add(家族信息库);
        当前玩家.家族 = 家族信息库;
        通用提示框.显示("创建成功!");
        this.gameObject.SetActive(false);
        家族信息显示.刷新显示();
    }
}
