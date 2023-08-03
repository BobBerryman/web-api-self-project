﻿/********************************************************************************
** Company：  
** auth：    lingyc
** date：    2018/11/28 11:22:34
** desc：    SqlSugarBase类
** Ver.:     V1.0.0
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SqlSugar;

namespace Infrastructure.Service.Base
{
    public class SqlSugarBase
    {
        #region 静态变量

        protected static SqlSugarClient GetInstance()
        {
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig() { ConnectionString = DbConfig.ConnectionString, DbType = DbType.MySql, IsAutoCloseConnection = true });
            db.Ado.IsEnableLogEvent = false;
            db.Ado.LogEventStarting = (sql, pars) =>
            {
                Console.WriteLine(sql + "\r\n" + db.Utilities.SerializeObject(pars.ToDictionary(it => it.ParameterName, it => it.Value)));
                Console.WriteLine();
            };
            return db;
        }

        #endregion

        #region 基本数据操作同步

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Insert<T>(T model) where T : class, new()
        {
            return GetInstance().Insertable(model).ExecuteCommand() > 0;
        }

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="models"></param>
        /// <returns></returns>
        public bool InsertBatch<T>(List<T> models) where T : class, new()
        {
            return GetInstance().Insertable(models).ExecuteCommand() == models.Count;
        }

        /// <summary>
        /// 插入数据返回自增ID
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public int InsertReturnId<T>(T model) where T : class, new()
        {
            return GetInstance().Insertable(model).ExecuteReturnIdentity();
        }

        /// <summary>
        /// 插入数据返回自增ID
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public long InsertReturnLongId<T>(T model) where T : class, new()
        {
            return GetInstance().Insertable(model).ExecuteReturnBigIdentity();
        }

        /// <summary>
        /// 根据主键查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public T FindById<T>(object id)
        {
            return GetInstance().Queryable<T>().InSingle(id);
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="orderByStr">排序方式</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount">总页数</param>
        /// <returns></returns>
        public IList<T> FindByPage<T>(string orderByStr, ref int totalCount, int pageIndex, int pageSize)
        {
            return GetInstance().Queryable<T>().OrderBy(orderByStr).ToPageList(pageIndex, pageSize, ref totalCount);
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">表达式</param>
        /// <param name="orderByStr">排序方式</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount">总页数</param>
        /// <returns></returns>
        public IList<T> FindByPage<T>(Expression<Func<T, bool>> expression, string orderByStr, ref int totalCount, int pageIndex, int pageSize)
        {
            return GetInstance().Queryable<T>().Where(expression).OrderBy(orderByStr).ToPageList(pageIndex, pageSize, ref totalCount);
        }

        /// <summary>
        /// 按条件查询数量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public int FindCountByFunc<T>(Expression<Func<T, bool>> expression)
        {
            return GetInstance().Queryable<T>().Where(expression).Count();
        }

        /// <summary>
        /// 按条件查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public T FindFirstByFunc<T>(Expression<Func<T, bool>> expression)
        {
            return GetInstance().Queryable<T>().Where(expression).First();
        }

        /// <summary>
        /// 按条件查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public IList<T> FindByFunc<T>(Expression<Func<T, bool>> expression)
        {
            return GetInstance().Queryable<T>().Where(expression).ToList();
        }

        /// <summary>
        /// 按条件查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">表达式</param>
        /// <param name="select">指定列</param>
        /// <returns></returns>
        public IList<T> FindByFunc<T>(Expression<Func<T, bool>> expression, string select)
        {
            return GetInstance().Queryable<T>().Where(expression).Select(select).ToList();
        }

        /// <summary>
        /// 根据主键更新数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Update<T>(T model) where T : class, new()
        {
            return GetInstance().Updateable(model).ExecuteCommand() > 0;
        }

        /// <summary>
        /// 指定字段根据条件更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columns">更新字段对象</param>
        /// <param name="expression">更新条件</param>
        /// <returns></returns>
        public bool Update<T>(dynamic columns, Expression<Func<T, bool>> expression) where T : class, new()
        {
            return GetInstance().Updateable<T>(columns).Where(expression).ExecuteCommand() > 0;
        }

        /// <summary>
        /// 根据主键删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Delete<T>(object id) where T : class, new()
        {
            return GetInstance().Deleteable<T>().In(id).ExecuteCommand() > 0;
        }

        /// <summary>
        /// 根据主键批量删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ids"></param>
        /// <returns></returns>
        public bool DeleteBatch<T>(object[] ids) where T : class, new()
        {
            return GetInstance().Deleteable<T>().In(ids).ExecuteCommand() == ids.Length;
        }

        /// <summary>
        /// 根据非主键批量删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">条件字段</param>
        /// <returns></returns>
        public bool DeleteBatchBySelf<T>(Expression<Func<T, bool>> expression) where T : class, new()
        {
            return GetInstance().Deleteable<T>().Where(expression).ExecuteCommand() > 0;
        }

        /// <summary>
        /// 根据非主键批量删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inField">条件字段</param>
        /// <param name="primaryKeyValues">包含的值</param>
        /// <returns></returns>
        public bool DeleteBatchBySelf<T>(Expression<Func<T, object>> inField, List<object> primaryKeyValues) where T : class, new()
        {
            return GetInstance().Deleteable<T>().In(inField, primaryKeyValues).ExecuteCommand() == primaryKeyValues.Count;
        }

        #endregion

        #region 基本数据操作异步

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public Task<int> InsertAsync<T>(T model) where T : class, new()
        {
            return GetInstance().Insertable(model).ExecuteCommandAsync();
        }

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="models"></param>
        /// <returns></returns>
        public Task<int> InsertBatchAsync<T>(List<T> models) where T : class, new()
        {
            return GetInstance().Insertable(models).ExecuteCommandAsync();
        }

        /// <summary>
        /// 插入数据返回自增ID
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public Task<int> InsertReturnIdAsync<T>(T model) where T : class, new()
        {
            return GetInstance().Insertable(model).ExecuteReturnIdentityAsync();
        }

        /// <summary>
        /// 插入数据返回自增ID
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public Task<long> InsertReturnLongIdAsync<T>(T model) where T : class, new()
        {
            return GetInstance().Insertable(model).ExecuteReturnBigIdentityAsync();
        }

        /// <summary>
        /// 根据主键查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public T FindByIdAsync<T>(object id)
        {
            return GetInstance().Queryable<T>().InSingle(id);
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="orderByStr">排序方式</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount">总页数</param>
        /// <returns></returns>
        public Task<KeyValuePair<List<T>, int>> FindByPageAsync<T>(string orderByStr, int totalCount, int pageIndex, int pageSize)
        {
            return GetInstance().Queryable<T>().OrderBy(orderByStr).ToPageListAsync(pageIndex, pageSize, totalCount);
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">表达式</param>
        /// <param name="orderByStr">排序方式</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount">总页数</param>
        /// <returns></returns>
        public Task<KeyValuePair<List<T>, int>> FindByPageAsync<T>(Expression<Func<T, bool>> expression, string orderByStr, ref int totalCount, int pageIndex, int pageSize)
        {
            return GetInstance().Queryable<T>().Where(expression).OrderBy(orderByStr).ToPageListAsync(pageIndex, pageSize, totalCount);
        }

        /// <summary>
        /// 按条件查询数量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public Task<int> FindCountByFuncAsync<T>(Expression<Func<T, bool>> expression)
        {
            return GetInstance().Queryable<T>().Where(expression).CountAsync();
        }

        /// <summary>
        /// 按条件查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public Task<T> FindFirstByFuncAsync<T>(Expression<Func<T, bool>> expression)
        {
            return GetInstance().Queryable<T>().Where(expression).FirstAsync();
        }

        /// <summary>
        /// 按条件查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public Task<List<T>> FindByFuncAsync<T>(Expression<Func<T, bool>> expression)
        {
            return GetInstance().Queryable<T>().Where(expression).ToListAsync();
        }

        /// <summary>
        /// 按条件查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">表达式</param>
        /// <param name="select">指定列</param>
        /// <returns></returns>
        public Task<List<T>> FindByFuncAsync<T>(Expression<Func<T, bool>> expression, string select)
        {
            return GetInstance().Queryable<T>().Where(expression).Select(select).ToListAsync();
        }

        /// <summary>
        /// 根据主键更新数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public Task<int> UpdateAsync<T>(T model) where T : class, new()
        {
            return GetInstance().Updateable(model).ExecuteCommandAsync();
        }

        /// <summary>
        /// 指定字段根据条件更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columns">更新字段对象</param>
        /// <param name="expression">更新条件</param>
        /// <returns></returns>
        public Task<int> UpdateAsync<T>(dynamic columns, Expression<Func<T, bool>> expression) where T : class, new()
        {
            return GetInstance().Updateable<T>(columns).Where(expression).ExecuteCommandAsync();
        }

        /// <summary>
        /// 根据主键删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<int> DeleteAsync<T>(object id) where T : class, new()
        {
            return GetInstance().Deleteable<T>().In(id).ExecuteCommandAsync();
        }

        /// <summary>
        /// 根据主键批量删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Task<int> DeleteBatchAsync<T>(object[] ids) where T : class, new()
        {
            return GetInstance().Deleteable<T>().In(ids).ExecuteCommandAsync();
        }

        /// <summary>
        /// 根据非主键批量删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">条件字段</param>
        /// <returns></returns>
        public Task<int> DeleteBatchBySelfAsync<T>(Expression<Func<T, bool>> expression) where T : class, new()
        {
            return GetInstance().Deleteable<T>().Where(expression).ExecuteCommandAsync();
        }

        /// <summary>
        /// 根据非主键批量删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inField">条件字段</param>
        /// <param name="primaryKeyValues">包含的值</param>
        /// <returns></returns>
        public Task<int> DeleteBatchBySelfAsync<T>(Expression<Func<T, object>> inField, List<object> primaryKeyValues) where T : class, new()
        {
            return GetInstance().Deleteable<T>().In(inField, primaryKeyValues).ExecuteCommandAsync();
        }

        #endregion
    }
}
