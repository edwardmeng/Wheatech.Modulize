using System;

namespace Wheatech.Modulize
{
    [Flags]
    public enum FeatureRuntimeState
    {
        None = 0x00,
        /// <summary>
        /// 当前功能所属的模块已经禁用
        /// </summary>
        ForbiddenModule = 0x01,
        UninstallModule = 0x02,
        /// <summary>
        /// 缺少依赖组件
        /// </summary>
        MissingDependency = 0x04,
        /// <summary>
        /// 存在不兼容的组件
        /// </summary>
        IncompatibleDependency = 0x08,
        /// <summary>
        /// 存在禁用的依赖组件
        /// </summary>
        ForbiddenDependency = 0x10,
        DisabledDependency = 0x20
    }
}
