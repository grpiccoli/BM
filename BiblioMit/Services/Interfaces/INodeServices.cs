using System;
using System.Threading;
using System.Threading.Tasks;

namespace BiblioMit.Services
{
    public interface INodeServices : IDisposable
    {
        Task<T> InvokeAsync<T>(string moduleName, params object[] args);
        Task<T> InvokeAsync<T>(string moduleName, CancellationToken cancellationToken, params object[] args);
        Task<T> InvokeExportAsync<T>(string moduleName, string exportedFunctionName, params object[] args);
        Task<T> InvokeExportAsync<T>(string moduleName, string exportedFunctionName, CancellationToken cancellationToken, params object[] args);
    }
}
