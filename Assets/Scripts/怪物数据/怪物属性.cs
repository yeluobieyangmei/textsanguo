using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using 国家系统;

namespace 怪物数据结构
{
    public class 怪物属性
    {
        public int 生命值 { get; set; }
        public int 当前生命值 { get; set; }
        public int 攻击力 { get; set; }
        public int 防御力 { get; set; }

        public 家族信息库 归属家族;
}
}
