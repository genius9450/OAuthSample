using OAuth.Sample.Domain.Shared;
using OAuth.Sample.Service.Interface;
using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Threading.Tasks;

namespace OAuth.Sample.Service.Service
{
    public class SQLService : ISQLService
    {
        /// <summary>
        /// DB 連線字串
        /// </summary>
        public string DefaultConnectionString { get; set; }

        /// <summary>
        /// 執行SP
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="procedureName"></param>
        /// <param name="model"></param>
        /// <param name="timeoutSecond"></param>
        /// <returns></returns>
        public async Task<SPResponseModel<T>> ExecuteStoredProcedure<T>(string procedureName, object model, int? timeoutSecond = null)
        {
            var outParam = new DynamicParameters();
            outParam.Add("@Result", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);
            outParam.Add("@ErrMessage", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 4000);
            var parameters = GenerateSQLParameters(model);
            parameters.AddDynamicParams(outParam);

            using (SqlConnection sqlConnObj = new SqlConnection(DefaultConnectionString))
            {
                await sqlConnObj.OpenAsync();
                var data = await sqlConnObj.QuerySingleOrDefaultAsync<T>(procedureName, param: (object)parameters, commandType: CommandType.StoredProcedure, commandTimeout: timeoutSecond);

                return new SPResponseModel<T>()
                {
                    Result = outParam.Get<int>("Result"),
                    ErrMessage = outParam.Get<string>("ErrMessage"),
                    Data = data
                };
            }
        }

        private DynamicParameters GenerateSQLParameters(object model)
        {
            var paramList = new DynamicParameters();
            Type modelType = model.GetType();
            var properties = modelType.GetProperties();
            foreach (var property in properties)
            {
                if (property.GetValue(model) == null)
                {
                    paramList.Add(property.Name, DBNull.Value);
                }
                else
                {
                    paramList.Add(property.Name, property.GetValue(model));
                }
            }
            return paramList;
        }

    }
}

