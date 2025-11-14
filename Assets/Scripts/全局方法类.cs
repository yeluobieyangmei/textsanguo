using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 全局方法类 : MonoBehaviour
{
    public static int 计算玩家生命值(int 玩家等级)
    {
        return 玩家等级 * 200;
    }

    public static int 计算玩家最终攻击力(int 玩家等级)
    {
        return 玩家等级 * 25;
    }

    public static int 计算玩家最终防御力(int 玩家等级)
    {
        return 玩家等级 * 25;
    }
}
