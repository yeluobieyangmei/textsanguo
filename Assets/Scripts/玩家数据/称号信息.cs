using System;
using UnityEngine;

namespace 玩家数据结构
{
    public class 称号信息
    {
        public 称号信息(string str, int dj, int lx)
        {
            this.名字 = str;
            this.类型 = lx;
            this.等级 = dj;
            this.状态 = 1;
        }

        public Color 获取称号颜色()
        {
            if (this.状态 == 0)
            {
                return 颜色类.GetColor("#C8C8C8");
            }
            if (this.等级 == 1)
            {
                return 颜色类.GetColor("#00FF00");
            }
            if (this.等级 == 2)
            {
                return 颜色类.GetColor("#FDA400");
            }
            if (this.等级 == 3)
            {
                return 颜色类.GetColor("#F000ED");
            }
            if (this.等级 == 4)
            {
                return 颜色类.GetColor("#FF0000");
            }
            return 颜色类.GetColor("#C8C8C8");
        }

        public string 名字;

        public int 状态;

        public int 类型;

        public int 等级;
    }
}
