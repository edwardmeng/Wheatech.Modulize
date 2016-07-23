using System;

namespace Wheatech.Modulize
{
    public class ModuleEventArgs : EventArgs
    {
        public ModuleEventArgs(ModuleDescriptor module)
        {
            Module = module;
        }

        public ModuleDescriptor Module { get; }
    }
}
