using System.Collections.Generic;
using System.Threading.Tasks;

namespace Estimmo.Runner
{
    internal interface IModule
    {
        public Task RunAsync(List<string> args);
    }
}
