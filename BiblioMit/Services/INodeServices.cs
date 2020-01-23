using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BiblioMit.Services
{
    public interface INodeServices : IDisposable
    {
        Task<T> InvokeAsync<T>(string moduleName, params object[] args);
        Task<T> InvokeAsync<T>(CancellationToken cancellationToken, string moduleName, params object[] args);

        Task<T> InvokeExportAsync<T>(string moduleName, string exportedFunctionName, params object[] args);
        Task<T> InvokeExportAsync<T>(CancellationToken cancellationToken, string moduleName, string exportedFunctionName, params object[] args);
    }
}
