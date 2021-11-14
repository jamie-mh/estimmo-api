using System;
using System.Collections.Generic;

namespace Estimmo.Runner
{
    public class ModuleArg
    {
        public ModuleArg()
        {
            Args = new List<string>();
        }

        public Type Type { get; set; }
        public List<string> Args { get; set; }
    }
}
