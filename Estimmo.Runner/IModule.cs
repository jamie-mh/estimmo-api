using System.Threading.Tasks;

namespace Estimmo.Runner
{
    internal interface IModule
    {
        public Task RunAsync(string[] args);
    }
}
