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
    public 通用提示框 通用提示框;
    public 家族信息显示 家族信息显示;

    public void 创建家族()
    {
        if (家族名输入框.text.Length > 4)
        {
            通用提示框.提示文本.text = "名称不可超过4字!";
            通用提示框.gameObject.SetActive(true);
            return;
        }
        if (家族名输入框.text == null || 家族名输入框.text == "" || 家族名输入框.text == " " || 家族名输入框.text == "  " || 家族名输入框.text == "   ")
        {
            通用提示框.提示文本.text = "请正确输入名称!";
            通用提示框.gameObject.SetActive(true);
            return;
        }
        玩家数据 当前玩家 = 全局变量.所有玩家数据表[全局变量.当前身份];
        国家信息库 当前国家 = 当前玩家.国家;
        for (int i = 0; i < 当前国家.家族成员表.Count; i++)
        {
            if (家族名输入框.text == 当前国家.家族成员表[i].家族名字)
            {
                通用提示框.提示文本.text = "名称已被占用!";
                通用提示框.gameObject.SetActive(true);
                return;
            }
        }
        家族信息库 家族信息库 = new 家族信息库();
        家族信息库.创建一个家族(家族名输入框.text, 当前玩家);
        当前国家.家族成员表.Add(家族信息库);
        全局变量.所有家族列表.Add(家族信息库);
        当前玩家.家族 = 家族信息库;
        通用提示框.提示文本.text = "创建成功!";
        通用提示框.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
        家族信息显示.刷新显示();
    }
}
