using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using 玩家数据结构;

namespace 玩家数据结构
{
    public class 玩家数据
    {
        public int ID;
        public int 等级;
        public int 铜钱;
        public int 黄金;
        public string 姓名;
        public string 称号名;
        public 玩家属性 玩家属性 = new 玩家属性();
        public List<称号信息> 称号表 = new List<称号信息>();

        public 玩家数据(string 姓名)
        {
            初始化一个玩家(姓名);  // ✅ 直接调用已有的初始化方法
        }

        public void 初始化一个玩家(string 姓名)
        {
            this.ID = 全局变量.所有玩家数据表.Count;
            this.等级 = 1;
            this.铜钱 = 10000;
            this.黄金 = 2000;
            this.姓名 = 姓名;
            this.称号名 = "九五至尊";
            this.玩家属性.生命值 = 全局方法类.计算玩家生命值(this.等级);
            this.玩家属性.攻击力 = 全局方法类.计算玩家最终攻击力(this.等级);
            this.玩家属性.防御力 = 全局方法类.计算玩家最终防御力(this.等级);
            this.称号表.Add(new 称号信息("散人", 1, 0));
            this.称号表.Add(new 称号信息("精英", 2, 0));
            this.称号表.Add(new 称号信息("大神", 3, 0));
            this.称号表.Add(new 称号信息("九五至尊", 4, 0));
            计算最终属性();  // ✅ 确保在初始化完成后计算属性
        }

        public void 计算最终属性()
        {
            int 称号值 = (int)称号加成();
            this.玩家属性.攻击力 = this.等级 * 25 * (int)称号加成();
            Debug.Log($"称号值是:{称号值},称号名是：{this.称号名}");
        }

        public double 称号加成()
        {
            switch (this.称号名)
            {
                case "无":
                    return 1;
                case "散人":
                    return 1.5;
                case "精英":
                    return 2;
                case "大神":
                    return 2.5;
                case "九五至尊":
                    return 3.5;
                default:
                    return 1;
            }
        }
    }
}

