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
        /// 所有的Feature都已经被禁用
        /// </summary>
        ForbiddenFeatures = 0x02
    }
}
