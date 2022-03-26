using OAuth.Sample.EF;
using OAuth.Sample.EF.Helper;
using OAuth.Sample.Service.Interface;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace OAuth.Sample.Service.Service
{
    public class BaseService : IBaseService
    {
        public IMapper Mapper { get; set; }

        public OAuthSampleDBContext DbContext { get; set; }

        /// <summary>
        /// 建立Entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public TEntry Create<TEntry>(TEntry entity)
        {
            DbContext.Add(entity);
            DbContext.SaveChanges();
            return entity;
        }

        /// <summary>
        /// 建立Entity(非同步)
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<TEntry> CreateAsync<TEntry>(TEntry entity)
        {
            await DbContext.AddAsync(entity);
            await DbContext.SaveChangesAsync();
            
            return entity;
        }

        /// <summary>
        /// 建立多筆Entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> CreateManyAsync(IEnumerable<object> entity)
        {
            await DbContext.AddRangeAsync(entity);
            var count = DbContext.SaveChanges();
            return count;
        }

        /// <summary>
        /// 取得單一Entity
        /// </summary>
        /// <typeparam name="TEntry"></typeparam>
        /// <param name="id">Key值</param>
        /// <param name="keyColumnName">Key值欄位名稱</param>
        /// <returns></returns>
        public TEntry GetSingle<TEntry>(object id, string keyColumnName = "Id") where TEntry : class
        {
            var parameter = Expression.Parameter(typeof(TEntry), "x");
            var property = Expression.Property(parameter, keyColumnName);
            var constant = Expression.Constant(id);
            var expr = Expression.Lambda<Func<TEntry, bool>>(Expression.Equal(property, constant), parameter);

            return (DbContext.GetDbSet<TEntry>()).Where(expr).SingleOrDefault();
        }

        /// <summary>
        /// 取得單一Entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public TEntry GetSingle<TEntry>(Expression<Func<TEntry, bool>> expression) where TEntry : class
        {
            return (DbContext.GetDbSet<TEntry>()).Where(expression).SingleOrDefault();
        }

        /// <summary>
        /// 取得多筆Entity
        /// </summary>
        /// <typeparam name="TEntry"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public IEnumerable<TEntry> GetList<TEntry>(Expression<Func<TEntry, bool>> expression) where TEntry : class
        {
            var result = DbContext.GetDbSet<TEntry>().Where(expression).ToList();
            return result;
        }

        /// <summary>
        /// 更新單一Entity
        /// </summary>
        /// <typeparam name="TEntry"></typeparam>
        /// <param name="entity"></param>
        /// <param name="keyColumnName"></param>
        /// <returns></returns>
        public int Update<TUpdate, TEntry>(TUpdate entity, string keyColumnName = "Id") where TEntry : class
        {
            var key = entity.GetType().GetProperty(keyColumnName).GetValue(entity);
            var oriEntry = GetSingle<TEntry>(key, keyColumnName);

            if (oriEntry != null)
            {
                var updateEntity = Mapper.Map<TUpdate, TEntry>(entity, oriEntry);
                DbContext.Entry(oriEntry).CurrentValues.SetValues(updateEntity);
                return DbContext.SaveChanges();
            }

            return 0;
        }

        /// <summary>
        /// 更新單一Entity(非同步)
        /// </summary>
        /// <typeparam name="TEntry"></typeparam>
        /// <param name="entity"></param>
        /// <param name="keyColumnName"></param>
        /// <returns></returns>
        public async Task<int> UpdateAsync<TEntry>(TEntry entity, string keyColumnName = "Id") where TEntry : class
        {
            var key = entity.GetType().GetProperty(keyColumnName).GetValue(entity);
            var oriEntry = GetSingle<TEntry>(key, keyColumnName);
            if (oriEntry != null)
            {
                DbContext.Entry(oriEntry).CurrentValues.SetValues(entity);
                return await DbContext.SaveChangesAsync();
            }

            return 0;
        }

        /// <summary>
        /// 更新單一Entity
        /// </summary>
        /// <typeparam name="TEntry"></typeparam>
        /// <param name="entity"></param>
        /// <param name="keyColumnName"></param>
        /// <returns></returns>
        public int Update<TEntry>(TEntry entity, string keyColumnName = "Id") where TEntry : class
        {
            var key = entity.GetType().GetProperty(keyColumnName).GetValue(entity);
            var oriEntry = GetSingle<TEntry>(key, keyColumnName);

            if (oriEntry != null)
            {
                DbContext.Entry(oriEntry).CurrentValues.SetValues(entity);
                return DbContext.SaveChanges();
            }

            return 0;
        }

        /// <summary>
        /// 更新單一Entity(非同步)
        /// </summary>
        /// <typeparam name="TEntry"></typeparam>
        /// <param name="entity"></param>
        /// <param name="keyColumnName"></param>
        /// <returns></returns>
        public async Task<int> UpdateAsync<TUpdate, TEntry>(TUpdate entity, string keyColumnName = "Id") where TEntry : class
        {
            var key = entity.GetType().GetProperty(keyColumnName).GetValue(entity);
            var oriEntry = GetSingle<TEntry>(key, keyColumnName);
            if (oriEntry != null)
            {
                var updateEntity = Mapper.Map<TUpdate, TEntry>(entity, oriEntry);
                DbContext.Entry(oriEntry).CurrentValues.SetValues(updateEntity);
                return await DbContext.SaveChangesAsync();
            }

            return 0;
        }

        /// <summary>
        /// 刪除單一Entity(非同步)
        /// </summary>
        /// <typeparam name="TEntry"></typeparam>
        /// <param name="entity"></param>
        /// <param name="keyColumnName"></param>
        /// <returns></returns>
        public async Task DeleteAsync<TEntry>(object id, string keyColumnName = "Id") where TEntry : class
        {
            DbContext.Remove<TEntry>(GetSingle<TEntry>(id, keyColumnName));
            await DbContext.SaveChangesAsync();
        }

        /// <summary>
        /// 刪除單一Entity
        /// </summary>
        /// <typeparam name="TEntry"></typeparam>
        /// <param name="entity"></param>
        /// <param name="keyColumnName"></param>
        /// <returns></returns>
        public void Delete<TEntry>(object id, string keyColumnName = "Id") where TEntry : class
        {
            DbContext.Remove<TEntry>(GetSingle<TEntry>(id, keyColumnName));
            DbContext.SaveChanges();
        }

        /// <summary>
        /// 刪除多筆Entity
        /// </summary>
        /// <typeparam name="TEntry"></typeparam>
        /// <param name="entity"></param>
        /// <param name="keyColumnName"></param>
        /// <returns></returns>
        public void DeleteMany<TEntry>(Expression<Func<TEntry, bool>> expression) where TEntry : class
        {
            var delEntities = DbContext.GetDbSet<TEntry>().Where(expression).ToList();
            DbContext.RemoveRange(delEntities);
            DbContext.SaveChanges();
        }

        /// <summary>
        /// 刪除多筆Entity
        /// </summary>
        /// <typeparam name="TEntry"></typeparam>
        /// <param name="entity"></param>
        /// <param name="keyColumnName"></param>
        /// <returns></returns>
        public async Task DeleteManyAsync<TEntry>(Expression<Func<TEntry, bool>> expression) where TEntry : class
        {
            var delEntities = DbContext.GetDbSet<TEntry>().Where(expression).ToList();
            DbContext.RemoveRange(delEntities);
            await DbContext.SaveChangesAsync();
        }

        /// <summary>
        /// 新增或是更新單筆Entity
        /// </summary>
        /// <typeparam name="TEntry"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public TEntry InsertOrUpdate<TEntry>(TEntry entity) where TEntry : class
        {            
            if (DbContext.Entry(entity).State == EntityState.Detached)
                DbContext.Set<TEntry>().Add(entity);

            // If an immediate save is needed, can be slow though
            // if iterating through many entities:
            DbContext.SaveChanges();

            return entity;
        }

        /// <summary>
        /// 新增或是更新單筆Entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="db"></param>
        public async Task<TEntry> InsertOrUpdateAsync<TEntry>(TEntry entity) where TEntry : class
        {
            if (DbContext.Entry(entity).State == EntityState.Detached)
                DbContext.Set<TEntry>().Add(entity);

            // If an immediate save is needed, can be slow though
            // if iterating through many entities:
            await DbContext.SaveChangesAsync();

            return entity;
        }

        /// <summary>
        /// 新增或是更新單筆Entity
        /// </summary>
        /// <typeparam name="TEntry"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public TEntry InsertOrUpdate<TUpdate, TEntry>(TUpdate updateModel, Expression<Func<TEntry, bool>> expression) where TEntry : class
        {
            TEntry updateEntity;

            var signle = DbContext.GetDbSet<TEntry>().Where(expression).FirstOrDefault();          
            if(signle != null)
            {
                updateEntity = Mapper.Map<TUpdate, TEntry>(updateModel, signle);
                DbContext.Entry(signle).CurrentValues.SetValues(updateEntity);
            }
            else
            {
                updateEntity = Mapper.Map<TUpdate, TEntry>(updateModel, signle);
                DbContext.Set<TEntry>().Add(updateEntity);
            }
            
            DbContext.SaveChanges();

            return updateEntity;
        }

        /// <summary>
        /// 新增或是更新單筆Entity(非同步)
        /// </summary>
        /// <typeparam name="TEntry"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<TEntry> InsertOrUpdateAsync<TUpdate, TEntry>(TUpdate updateModel, Expression<Func<TEntry, bool>> expression) where TEntry : class
        {
            TEntry updateEntity;

            var signle = DbContext.GetDbSet<TEntry>().Where(expression).FirstOrDefault();
            if (signle != null)
            {
                updateEntity = Mapper.Map<TUpdate, TEntry>(updateModel, signle);
                DbContext.Entry(signle).CurrentValues.SetValues(updateEntity);
            }
            else
            {
                updateEntity = Mapper.Map<TUpdate, TEntry>(updateModel, signle);
                DbContext.Set<TEntry>().Add(updateEntity);
            }
            await DbContext.SaveChangesAsync();

            return updateEntity;
        }

    }
}

