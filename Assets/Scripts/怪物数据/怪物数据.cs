using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace 怪物数据结构
{
    public enum 怪物类型
    {
        哥布林 = 1,
        高级哥布林 = 2,
        哥布林精英 = 3,
        哥布林长老 = 4,
        王城战Boss = 5,
    }
    public class 怪物数据
    {
        private static int 全局ID计数器 = 1;

        // 怪物唯一标识
        public int ID { get; private set; }
        public 怪物类型 类型 { get; private set; }
        public string 名称 { get; private set; }
        public int 等级 { get; private set; }
        public int 铜钱 { get; set; }
        public int 经验值 { get; set; }
        public 怪物属性 属性 { get; private set; } = new 怪物属性();

        // 私有构造函数
        private 怪物数据() { }

        // 工厂方法
        public static 怪物数据 创建(怪物类型 类型, int 等级, int 生命值, int 攻击力, int 防御力)
        {
            var 怪物 = new 怪物数据
            {
                ID = 获取下一个ID(),
                类型 = 类型,
                名称 = 获取怪物名称(类型),
                等级 = 等级
            };

            return 怪物.初始化(生命值, 攻击力, 防御力);
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
                怪物类型.王城战Boss => "王城战Boss",
                _ => "未知怪物"
            };
        }

        private 怪物数据 初始化(int 生命值, int 攻击力, int 防御力)
        {
            属性.生命值 = 生命值;
            属性.当前生命值 = 属性.生命值;
            属性.攻击力 = 攻击力;
            属性.防御力 = 防御力;
            return this;
        }

        // 重置怪物状态（用于对象池）
        // 在 怪物数据.cs 中修改 重置 方法
        public void 重置(怪物类型 类型, int 等级, int 生命值, int 攻击力, int 防御力)
        {
            this.类型 = 类型;
            this.等级 = 等级;
            名称 = 获取怪物名称(类型);
            初始化(生命值, 攻击力, 防御力);
        }
    }
}
