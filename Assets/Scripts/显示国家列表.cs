using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class 显示国家列表 : MonoBehaviour
{
    public Transform 父对象;
    public GameObject 要克隆的对象;

    public void OnEnable()
    {
        刷新显示();
    }

    public void 刷新显示()
    {
        要克隆的对象.gameObject.SetActive(false);
        int count = 全局变量.所有国家列表.Count;

        for (int i = 0; i < count; i++)
        {
            GameObject 克隆对象 = Instantiate(要克隆的对象, 父对象);
            克隆对象.transform.GetChild(0).GetComponent<Text>().text = 全局变量.所有国家列表[i].国号;
            克隆对象.transform.GetChild(1).GetComponent<Text>().text = 全局变量.所有国家列表[i].国名;
            克隆对象.gameObject.SetActive(true);
        }
    }
}
