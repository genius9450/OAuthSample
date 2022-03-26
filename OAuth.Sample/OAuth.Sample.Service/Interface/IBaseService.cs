using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace OAuth.Sample.Service.Interface
{
    public interface IBaseService
    {
        /// <summary>
        /// 建立Entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        TEntry Create<TEntry>(TEntry entity);

        /// <summary>
        /// 建立Entity(非同步)
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<TEntry> CreateAsync<TEntry>(TEntry entity);

        /// <summary>
        /// 建立多筆Entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> CreateManyAsync(IEnumerable<object> entity);

        /// <summary>
        /// 取得單一Entity
        /// </summary>
        /// <typeparam name="TEntry"></typeparam>
        /// <param name="id">Key值</param>
        /// <param name="keyColumnName">Key值欄位名稱</param>
        /// <returns></returns>
        TEntry GetSingle<TEntry>(object id, string keyColumnName = "Id") where TEntry : class;

        /// <summary>
        /// 取得單一Entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        TEntry GetSingle<TEntry>(Expression<Func<TEntry, bool>> expression) where TEntry : class;

        /// <summary>
        /// 取得多筆Entity
        /// </summary>
        /// <typeparam name="TEntry"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        IEnumerable<TEntry> GetList<TEntry>(Expression<Func<TEntry, bool>> expression) where TEntry : class;

        /// <summary>
        /// 更新單一Entity
        /// </summary>
        /// <typeparam name="TEntry"></typeparam>
        /// <param name="entity"></param>
        /// <param name="keyColumnName"></param>
        /// <returns></returns>
        int Update<TUpdate, TEntry>(TUpdate entity, string keyColumnName = "Id") where TEntry : class;

        /// <summary>
        /// 更新單一Entity(非同步)
        /// </summary>
        /// <typeparam name="TEntry"></typeparam>
        /// <param name="entity"></param>
        /// <param name="keyColumnName"></param>
        /// <returns></returns>
        Task<int> UpdateAsync<TUpdate, TEntry>(TUpdate entity, string keyColumnName = "Id") where TEntry : class;

        /// <summary>
        /// 更新單一Entity
        /// </summary>
        /// <typeparam name="TEntry"></typeparam>
        /// <param name="entity"></param>
        /// <param name="keyColumnName"></param>
        /// <returns></returns>
        int Update<TEntry>(TEntry entity, string keyColumnName = "Id") where TEntry : class;

        /// <summary>
        /// 更新單一Entity(非同步)
        /// </summary>
        /// <typeparam name="TEntry"></typeparam>
        /// <param name="entity"></param>
        /// <param name="keyColumnName"></param>
        /// <returns></returns>
        Task<int> UpdateAsync<TEntry>(TEntry entity, string keyColumnName = "Id") where TEntry : class;

        /// <summary>
        /// 刪除單一Entity(非同步)
        /// </summary>
        /// <typeparam name="TEntry"></typeparam>
        /// <param name="entity"></param>
        /// <param name="keyColumnName"></param>
        /// <returns></returns>
        Task DeleteAsync<TEntry>(object id, string keyColumnName = "Id") where TEntry : class;

        /// <summary>
        /// 刪除單一Entity
        /// </summary>
        /// <typeparam name="TEntry"></typeparam>
        /// <param name="entity"></param>
        /// <param name="keyColumnName"></param>
        /// <returns></returns>
        void Delete<TEntry>(object id, string keyColumnName = "Id") where TEntry : class;

        /// <summary>
        /// 刪除多筆Entity
        /// </summary>
        /// <typeparam name="TEntry"></typeparam>
        /// <param name="entity"></param>
        /// <param name="keyColumnName"></param>
        /// <returns></returns>
        void DeleteMany<TEntry>(Expression<Func<TEntry, bool>> expression) where TEntry : class;

        /// <summary>
        /// 刪除多筆Entity
        /// </summary>
        /// <typeparam name="TEntry"></typeparam>
        /// <param name="entity"></param>
        /// <param name="keyColumnName"></param>
        /// <returns></returns>
        Task DeleteManyAsync<TEntry>(Expression<Func<TEntry, bool>> expression) where TEntry : class;

        /// <summary>
        /// 新增或是更新單筆Entity
        /// </summary>
        /// <typeparam name="TEntry"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        TEntry InsertOrUpdate<TEntry>(TEntry entity) where TEntry : class;

        /// <summary>
        /// 新增或是更新單筆Entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="db"></param>
        Task<TEntry> InsertOrUpdateAsync<TEntry>(TEntry entity) where TEntry : class;

        /// <summary>
        /// 新增或是更新單筆Entity
        /// </summary>
        /// <typeparam name="TEntry"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        TEntry InsertOrUpdate<TUpdate, TEntry>(TUpdate entity, Expression<Func<TEntry, bool>> expression) where TEntry : class;

        /// <summary>
        /// 新增或是更新單筆Entity(非同步)
        /// </summary>
        /// <typeparam name="TEntry"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<TEntry> InsertOrUpdateAsync<TUpdate, TEntry>(TUpdate updateModel, Expression<Func<TEntry, bool>> expression) where TEntry : class;

    }
}

