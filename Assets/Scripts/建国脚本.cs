using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using 玩家数据结构;
using 国家系统;

public class 建国脚本 : MonoBehaviour
{
    public 国家功能显示 国家功能显示;
    public InputField 国名输入框;
    public InputField 国号输入框;
    public InputField 公告输入框;
    public InputField 宣言输入框;
    public 通用提示框 通用提示框;
    private bool 通过;
    private void 信息检测()
    {
        if (国名输入框.text.Length <= 0)
        {
            通用提示框.提示文本.text = "请输入国家名称!";
            通用提示框.gameObject.SetActive(true);
            Debug.Log("测试  国名错误");
            通过 = false;
            return;
        }
        else if(国名输入框.text.Length > 4)
        {
            通用提示框.提示文本.text = "国家名称不可超过4字!";
            通用提示框.gameObject.SetActive(true);
            通过 = false;
            return;
        }

        if (国号输入框.text.Length <= 0)
        {
            通用提示框.提示文本.text = "请输入国家国号!";
            通用提示框.gameObject.SetActive(true);
            通过 = false;
            return;
        }
        else if (国号输入框.text.Length > 1)
        {
            通用提示框.提示文本.text = "国家国号不可超过1字!";
            通用提示框.gameObject.SetActive(true);
            通过 = false;
            return;
        }

        if (公告输入框.text.Length <= 0)
        {
            通用提示框.提示文本.text = "请输入国家公告!";
            通用提示框.gameObject.SetActive(true);
            通过 = false;
            return;
        }
        else if (公告输入框.text.Length > 20)
        {
            通用提示框.提示文本.text = "国家国号不可超过20字!";
            通用提示框.gameObject.SetActive(true);
            通过 = false;
            return;
        }

        if (宣言输入框.text.Length <= 0)
        {
            通用提示框.提示文本.text = "请输入国家宣言!";
            通用提示框.gameObject.SetActive(true);
            通过 = false;
            return;
        }
        else if (宣言输入框.text.Length > 20)
        {
            通用提示框.提示文本.text = "国家宣言不可超过20字!";
            通用提示框.gameObject.SetActive(true);
            通过 = false;
            return;
        }
        通过 = true;
    }

    private void 财产检测()
    {
        玩家数据 当前玩家 = 全局变量.所有玩家数据表[全局变量.当前身份];
        if (当前玩家.铜钱 < 10000000)
        {
            通用提示框.提示文本.text = "铜钱不足1千万！";
            通用提示框.提示文本.gameObject.SetActive(true);
            通过 = false;
            return;
        }
        if (当前玩家.黄金 < 200000)
        {
            通用提示框.提示文本.text = "黄金不足20万！";
            通用提示框.提示文本.gameObject.SetActive(true);
            通过 = false;
            return;
        }
    }

    public void 确定建国()
    {
        信息检测();
        财产检测();
        if (!通过)
        {
            return;
        }
        玩家数据 当前玩家 = 全局变量.所有玩家数据表[全局变量.当前身份];
        当前玩家.铜钱 -= 10000000;
        当前玩家.黄金 -= 200000;

        国家信息库 国家信息库 = new 国家信息库();
        国家信息库.创建一个新的国家(国名输入框.text, 国号输入框.text, 公告输入框.text, 宣言输入框.text);
        全局变量.所有国家列表.Add(国家信息库);

        当前玩家.国家.国家成员表.Remove(当前玩家);
        当前玩家.国家 = 国家信息库;
        国家信息库.国王ID = 当前玩家.ID;

        当前玩家.官职 = "国王";
        国家功能显示.刷新显示();
        this.gameObject.SetActive(false);
    }
}
