using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using 国家系统;

public class 显示国家列表 : MonoBehaviour
{
    public Transform 父对象;
    public GameObject 要克隆的对象;
    List<GameObject> 克隆池 = new List<GameObject>();
    public 国家信息库 当前选中国家 = null;
    public 国家功能显示 国家功能显示;
    public 通用提示框 通用提示框;
    public void OnEnable()
    {
        刷新显示();
    }

    public void 刷新显示()
    {
        要克隆的对象.gameObject.SetActive(false);
        int count = 全局变量.所有国家列表.Count;

        foreach (var obj in 克隆池)//遍历每个被克隆出来的对象
        {
            if (obj != null) Destroy(obj);//如果这个对象在unity中还在不是空的 就Destroy(obj)销毁这个对象
        }
        克隆池.Clear();

        for (int i = 0; i < count; i++)
        {
            GameObject 克隆对象 = Instantiate(要克隆的对象, 父对象);
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

    public void 进入游戏()
    {
        全局变量.所有玩家数据表[全局变量.当前身份].加入一个国家(当前选中国家.国名);
        SceneManager.LoadScene("主界面");
    }

    public void 更换国家()
    {
        if (当前选中国家.国名 == 全局变量.所有玩家数据表[全局变量.当前身份].国家.国名)
        {
            通用提示框.提示文本.text = "已在当前国家!";
            通用提示框.gameObject.SetActive(true);
            return;
        }
        全局变量.所有玩家数据表[全局变量.当前身份].加入一个国家(当前选中国家.国名);
        国家功能显示.刷新显示();
        this.gameObject.SetActive(false);
    }
}
