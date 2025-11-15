using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using 国家系统;

public class 全局方法类 : MonoBehaviour
{
    public static void 初始化国家()
    {
        国家信息库 国家信息库 = new 国家信息库();
        国家信息库.创建一个新的国家("魏国", "魏", "无", "无");

        国家信息库 国家信息库1 = new 国家信息库();
        国家信息库1.创建一个新的国家("蜀国", "蜀", "无", "无");

        国家信息库 国家信息库2 = new 国家信息库();
        国家信息库2.创建一个新的国家("吴国", "吴", "无", "无");
    }

    public static 国家信息库 获取指定名字的国家(string 名字)
    {
        int count = 全局变量.所有国家列表.Count;
        for (int i = 0; i < count; i++)
        {
            if (全局变量.所有国家列表[i].国名 == 名字)
            {
                return 全局变量.所有国家列表[i];
            }
        }
        return null;
    }
}
