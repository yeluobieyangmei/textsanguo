using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using 国家系统;
using 玩家数据结构;

public class 国家成员列表 : MonoBehaviour
{
    public Transform 父对象;
    public GameObject 要克隆的对象;
    List<GameObject> 克隆池 = new List<GameObject>();
    public 玩家数据 当前选中玩家 = null;
    public 国家功能显示 国家功能显示;
    public 国家信息库 当前国家 { get; set; }
    public 显示类型 当前显示类型;
    public 官员类型 当前官员类型;
    public Button 任命按钮;
    public enum 显示类型
    {
        任命官员,
        不任命官员
    }

    public enum 官员类型
    {
        大都督,
        丞相,
        太尉,
        御史大夫,
        金吾卫
    }
    public void OnEnable()
    {
        刷新显示();
    }

    public void 刷新显示()
    {
        任命按钮.gameObject.SetActive(当前显示类型 == 显示类型.任命官员);
        要克隆的对象.gameObject.SetActive(false);
        int count = 当前国家.国家成员表.Count;

        foreach (var obj in 克隆池)//遍历每个被克隆出来的对象
        {
            if (obj != null) Destroy(obj);//如果这个对象在unity中还在不是空的 就Destroy(obj)销毁这个对象
        }
        克隆池.Clear();

        for (int i = 0; i < count; i++)
        {
            GameObject 克隆对象 = Instantiate(要克隆的对象, 父对象);

            if (当前显示类型 == 显示类型.不任命官员)
            {
                克隆对象.transform.GetChild(0).GetComponent<Text>().text = $"LV.{全局变量.所有玩家数据表[当前国家.国家成员表[i].ID].等级}";
                克隆对象.transform.GetChild(1).GetComponent<Text>().text = $"{全局变量.所有玩家数据表[当前国家.国家成员表[i].ID].姓名}";
                克隆对象.gameObject.SetActive(true);
                克隆池.Add(克隆对象);
            }
            else if(当前显示类型 == 显示类型.任命官员)
            {
                if (全局变量.所有玩家数据表[当前国家.国家成员表[i].ID].ID != 当前国家.国王ID)
                {
                    克隆对象.transform.GetChild(0).GetComponent<Text>().text = $"LV.{全局变量.所有玩家数据表[当前国家.国家成员表[i].ID].等级}";
                    克隆对象.transform.GetChild(1).GetComponent<Text>().text = $"{全局变量.所有玩家数据表[当前国家.国家成员表[i].ID].姓名}";
                    克隆对象.gameObject.SetActive(true);
                    克隆池.Add(克隆对象);
                }
            }
            

            // 处理 Toggle 选择逻辑
            Toggle t = 克隆对象.GetComponent<Toggle>();//获取每个克隆对象上的Toggle组件
            玩家数据 捕获玩家 = 当前国家.国家成员表[i]; // 闭包捕获
            t.onValueChanged.AddListener(isOn => //如果这个对象被点击了，就把当前选中国家赋值为当前点中的国家
            {
                if (isOn)
                {
                    当前选中玩家 = 捕获玩家;
                    Debug.Log($"当前选中玩家：{当前选中玩家.姓名}");
                }
            });
        }
    }

    public void 任命官员()
    {
        if (当前国家.大都督ID != -1)
        {
            全局变量.所有玩家数据表[当前国家.大都督ID].官职 = "国民";
            当前国家.大都督ID = 当前选中玩家.ID;
            当前选中玩家.官职 = "大都督";
            this.gameObject.SetActive(false);
            国家功能显示.刷新显示();
            通用提示框.显示($"已任命{当前选中玩家.姓名}为本国大都督!");
            return;
        }
        当前国家.大都督ID = 当前选中玩家.ID;
        当前选中玩家.官职 = "大都督";
        this.gameObject.SetActive(false);
        国家功能显示.刷新显示();
        通用提示框.显示($"已任命{当前选中玩家.姓名}为本国大都督!");
    }
}
