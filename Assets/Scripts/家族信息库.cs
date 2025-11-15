using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using 玩家数据结构;

namespace 国家系统
{
    public class 家族信息库 : MonoBehaviour
    {
        public int 家族ID;
        public int 族长ID;
        public int 副族长ID;
        public string 家族名字;
        public List<玩家数据> 家族成员 = new List<玩家数据> ();

        public void 创建一个家族(string 家族名字, 玩家数据 族长)
        {
            全局变量.家族ID记录++;
            this.家族ID = 全局变量.家族ID记录;
            this.族长ID = 族长.ID;
            this.副族长ID = 0;
            this.家族名字 = 家族名字;
            家族成员.Add(族长);
        }
    }

}
