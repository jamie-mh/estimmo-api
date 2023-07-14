using System.Collections.Generic;
using System.Threading.Tasks;

namespace Estimmo.Runner
{
    public interface IModule
    {
        public Task RunAsync(Dictionary<string, string> args);
    }
}
