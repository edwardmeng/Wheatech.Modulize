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
        /// <summary>
        /// 缺少依赖组件
        /// </summary>
        MissingDependency = 0x02,
        /// <summary>
        /// 存在不兼容的组件
        /// </summary>
        IncompatibleDependency = 0x04,
        /// <summary>
        /// 存在禁用的依赖组件
        /// </summary>
        ForbiddenDependency = 0x08,
        DisabledDependency = 0x10,
        /// <summary>
        /// Module尚未安装
        /// </summary>
        UninstallModule = 0x20,
        /// <summary>
        /// 禁用
        /// </summary>
        Forbidden = 0x40,
        /// <summary>
        /// 需要安装
        /// </summary>
        RequiresInstall = 0x80,
        /// <summary>
        /// 需要启用
        /// </summary>
        RequiresEnable = 0x100,
    }
}
