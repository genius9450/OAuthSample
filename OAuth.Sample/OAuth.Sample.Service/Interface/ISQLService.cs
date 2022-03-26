using OAuth.Sample.Domain.Shared;
using System.Threading.Tasks;

namespace OAuth.Sample.Service.Interface
{
    public interface ISQLService
    {
        /// <summary>
        /// 執行SP
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="procedureName"></param>
        /// <param name="model"></param>
        /// <param name="timeoutSecond"></param>
        /// <returns></returns>
        Task<SPResponseModel<T>> ExecuteStoredProcedure<T>(string procedureName, object model, int? timeoutSecond = null);
    }
}

