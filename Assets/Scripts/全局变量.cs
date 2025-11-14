using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using 玩家数据结构;

public class 全局变量
{

    public static void SetActive(GameObject go, bool state)
    {
        if (go == null)
        {
            return;
        }
        if (go.activeSelf != state)
        {
            go.SetActive(state);
        }
    }
    public static List<玩家数据> 所有玩家数据表 = new List<玩家数据>();

    public static int 当前身份 = 0;
}
