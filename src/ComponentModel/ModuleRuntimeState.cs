using System;

namespace Wheatech.Modulize
{
    [Flags]
    public enum ModuleRuntimeState
    {
        None = 0x00,
        /// <summary>
        /// 不兼容当前运行的软件平台
        /// </summary>
        IncompatibleHost = 0x01,
        /// <summary>
        /// 反射加载程序集失败
        /// </summary>
        ReflectionFailed = 0x02,
        /// <summary>
        /// 所有的Feature都已经被禁用
        /// </summary>
        ForbiddenFeatures = 0x04,
        /// <summary>
        /// 禁用
        /// </summary>
        Forbidden = 0x08,
        /// <summary>
        /// 需要安装
        /// </summary>
        RequireInstall = 0x10,
    }
}
