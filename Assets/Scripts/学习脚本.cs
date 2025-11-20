using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using 玩家数据结构;
using 国家系统;

public class 学习脚本 : MonoBehaviour
{
    public Transform 父列表对象;
    public GameObject 克隆模板;
    public 国家信息库 当前选中国家 = null;
    List<GameObject> 克隆池 = new List<GameObject>();

    public void OnEnable()
    {
        刷新显示();
    }

    public void 刷新显示()
    {

        Debug.Log("刷新国家列表显示");
        克隆模板.gameObject.SetActive(false);

        int 国家数量 = 全局变量.所有国家列表.Count;

        //清空unity中已经生成的克隆对象
        foreach (var obj in 克隆池)//遍历每个被克隆出来的对象
        {
            if (obj != null) Destroy(obj);//如果这个对象在unity中还在不是空的 就Destroy(obj)销毁这个对象
        }
        克隆池.Clear();//这个是清空C#脚本里的克隆池，但是已经生成的unity克隆对象还在，所以要进行上部分的Destroy(obj)销毁克隆对象  单用克隆池.Clear()是无法实现清空全部克隆数据的

        for (int i = 0; i < 国家数量; i++)
        {
            GameObject 克隆对象 = Instantiate(克隆模板, 父列表对象);
            克隆对象.transform.GetChild(0).GetComponent<Text>().text = 全局变量.所有国家列表[i].国号;
            克隆对象.transform.GetChild(1).GetComponent<Text>().text = 全局变量.所有国家列表[i].国名;
            克隆对象.gameObject.SetActive(true);
            克隆池.Add(克隆对象);

            // 处理 Toggle 选择逻辑
            Toggle t = 克隆对象.GetComponent<Toggle>();//获取每个克隆对象上的Toggle组件
            国家信息库 捕获国家 = 全局变量.所有国家列表[i]; // 闭包捕获
            t.onValueChanged.AddListener(isOn => //如果这个对象被点击了，就把当前选中国家赋值为当前点中的国家
            {
                if (isOn)
                {
                    当前选中国家 = 捕获国家;
                    Debug.Log($"更换国家脚本：选中国家 {捕获国家.国名}({捕获国家.国号})");
                }
            });
        }
    }

}
