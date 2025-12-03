using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using 玩家数据结构;
using 怪物数据结构;

public class 战斗展示界面 : MonoBehaviour
{
    public 攻击对象查看 攻击对象查看;
    public Transform 父对象;
    public GameObject 要克隆的对象;
    List<GameObject> 克隆池 = new List<GameObject>();
    public 怪物数据 当前选中怪物 = null;
    private List<怪物数据> 哥布林列表 = new List<怪物数据>();
    public void OnEnable()
    {
        //刷新显示();
    }

    public void 刷新显示()
    {
        要克隆的对象.gameObject.SetActive(false);
        int count = 哥布林列表.Count;

        foreach (var obj in 克隆池)//遍历每个被克隆出来的对象
        {
            if (obj != null) Destroy(obj);//如果这个对象在unity中还在不是空的 就Destroy(obj)销毁这个对象
        }
        克隆池.Clear();

        for (int i = 0; i < count; i++)
        {
            GameObject 克隆对象 = Instantiate(要克隆的对象, 父对象);
            克隆对象.transform.GetChild(0).GetComponent<Text>().text = $"LV.{哥布林列表[i].等级} {哥布林列表[i].名称}";
            克隆对象.gameObject.SetActive(true);
            克隆池.Add(克隆对象);

            // 处理 Toggle 选择逻辑
            Toggle t = 克隆对象.GetComponent<Toggle>();//获取每个克隆对象上的Toggle组件
            怪物数据 捕获怪物 = 哥布林列表[i]; // 闭包捕获
            t.onValueChanged.AddListener(isOn => //如果这个对象被点击了，就把当前选中国家赋值为当前点中的国家
            {
                if (isOn)
                {
                    当前选中怪物 = 捕获怪物;
                    攻击对象查看.当前怪物 = 捕获怪物;
                    攻击对象查看.gameObject.SetActive(true);
                }
            });
        }
    }

    public void 移除已击败怪物(怪物数据 被击败的怪物)
    {
        // 从哥布林列表中移除被击败的怪物
        哥布林列表.RemoveAll(怪物 => 怪物.ID == 被击败的怪物.ID);

        // 重新刷新UI
        刷新显示();
    }

    public void 点击哥布林按钮()
    {
        if (哥布林列表.Count <= 0)
        {
            生成哥布林();
            刷新显示();
        }
        else
        {
            刷新显示();
        }
    }
    private void 生成哥布林()
    {
        for (int i = 0; i < 10; i++)
        {
            var 哥布林 = 怪物管理器.生成怪物(怪物类型.哥布林, 5, 100, 2, 2);
            哥布林列表.Add(哥布林);
        }
    }

    
}
