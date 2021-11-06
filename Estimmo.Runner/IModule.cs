using System.Threading.Tasks;

namespace Estimmo.Runner
{
    interface IModule
    {
        public Task RunAsync(string[] args);
    }
}
