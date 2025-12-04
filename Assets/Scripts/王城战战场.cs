using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using 怪物数据结构;
using 玩家数据结构;
using 国家系统;

public class 王城战战场 : MonoBehaviour
{
    private 怪物数据 王城战Boss = 怪物管理器.生成怪物(怪物类型.王城战Boss, 100, 1000000, 2, 100);
    public Text Boss血量;
    public Text Boss归属;
    public Text 家族1名字;
    public Text 家族2名字;
    public Text 家族1积分;
    public Text 家族2积分;

    private 家族信息库 家族1;
    private 家族信息库 家族2;

    public void OnEnable()
    {
        
    }
}
