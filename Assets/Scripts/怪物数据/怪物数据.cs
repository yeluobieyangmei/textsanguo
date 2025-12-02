using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace 怪物数据
{
    public enum 怪物类型
    {
        哥布林 = 1,
        高级哥布林 = 2,
        哥布林精英 = 3,
        哥布林长老 = 4
    }
    public class 怪物数据
    {
        private static int 全局ID计数器 = 1;

        // 怪物唯一标识
        public int ID { get; private set; }
        public 怪物类型 类型 { get; private set; }
        public string 名称 { get; private set; }
        public int 等级 { get; private set; }
        public 怪物属性 属性 { get; private set; } = new 怪物属性();

        // 私有构造函数
        private 怪物数据() { }

        // 工厂方法
        public static 怪物数据 创建(怪物类型 类型, int 等级 = 1)
        {
            var 怪物 = new 怪物数据
            {
                ID = 获取下一个ID(),
                类型 = 类型,
                名称 = 获取怪物名称(类型),
                等级 = 等级
            };

            return 怪物.初始化();
        }

        private static int 获取下一个ID()
        {
            return 全局ID计数器++;
        }

        private static string 获取怪物名称(怪物类型 类型)
        {
            return 类型 switch
            {
                怪物类型.哥布林 => "哥布林",
                怪物类型.高级哥布林 => "高级哥布林",
                怪物类型.哥布林精英 => "哥布林精英",
                怪物类型.哥布林长老 => "哥布林长老",
                _ => "未知怪物"
            };
        }

        private 怪物数据 初始化()
        {
            // 基础属性计算
            int 类型倍率 = (int)类型;
            属性.生命值 = 等级 * 100 * 类型倍率;
            属性.攻击力 = 等级 * 5 * 类型倍率;
            属性.防御力 = 等级 * 2 * 类型倍率;
            return this;
        }

        // 重置怪物状态（用于对象池）
        public void 重置(怪物类型? 新类型 = null, int? 新等级 = null)
        {
            if (新类型.HasValue) 类型 = 新类型.Value;
            if (新等级.HasValue) 等级 = 新等级.Value;

            名称 = 获取怪物名称(类型);
            初始化();
        }
    }
}
