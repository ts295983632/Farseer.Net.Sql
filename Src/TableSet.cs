﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using FS.Cache;
using FS.Extends;
using FS.Infrastructure;
using FS.Sql.Data;
using FS.Utils.Common;

namespace FS.Sql
{
    /// <summary>
    ///     表操作
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public sealed class TableSet<TEntity> : ReadDbSet<TableSet<TEntity>, TEntity> where TEntity : class, new()
    {
        /// <summary>
        ///     使用属性类型的创建
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="pInfo">属性类型</param>
        internal TableSet(DbContext context, PropertyInfo pInfo)
        {
            SetContext(context, pInfo);
        }

        #region 条件

        /// <summary>
        ///     字段累加（字段 = 字段 + 值）
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="fieldName">字段选择器</param>
        /// <param name="fieldValue">值</param>
        public TableSet<TEntity> AddAssign<T>(Expression<Func<TEntity, T>> fieldName, T fieldValue)
        {
            Queue.ExpBuilder.AddAssign(fieldName, fieldValue);
            return this;
        }

        /// <summary>
        ///     字段累加（字段 = 字段 + 值）
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="fieldName">字段选择器</param>
        /// <param name="fieldValue">值</param>
        public TableSet<TEntity> AddAssign<T>(Expression<Func<TEntity, T?>> fieldName, T fieldValue) where T : struct
        {
            Queue.ExpBuilder.AddAssign(fieldName, fieldValue);
            return this;
        }

        /// <summary>
        ///     字段累加（字段 = 字段 + 值）
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="fieldName">字段选择器</param>
        /// <param name="fieldValue">值</param>
        public TableSet<TEntity> AddAssign<T>(Expression<Func<TEntity, object>> fieldName, T fieldValue) where T : struct
        {
            Queue.ExpBuilder.AddAssign(fieldName, fieldValue);
            return this;
        }

        #endregion

        #region Copy

        /// <summary>
        ///     复制数据
        /// </summary>
        /// <param name="acTEntity">对新职的赋值</param>
        public void Copy(Action<TEntity> acTEntity = null)
        {
            var lst = ToList();
            foreach (var info in lst)
            {
                acTEntity?.Invoke(info);
                Insert(info);
            }
        }
        /// <summary>
        ///     复制数据
        /// </summary>
        /// <param name="acTEntity">对新职的赋值</param>
        public Task CopyAsync(Action<TEntity> acTEntity = null)
        {
            return Task.Factory.StartNew(() => Copy(acTEntity));
        }

        /// <summary>
        ///     复制数据
        /// </summary>
        /// <param name="act">对新职的赋值</param>
        /// <typeparam name="T">ID</typeparam>
        /// <param name="ID">o => o.ID.Equals(ID)</param>
        /// <param name="memberName">条件字段名称，如为Null，默认为主键字段</param>
        public void Copy<T>(T ID, Action<TEntity> act = null, string memberName = null) where T : struct
        {
            Where(ID, memberName);
            Copy(act);
        }
        /// <summary>
        ///     复制数据
        /// </summary>
        /// <param name="act">对新职的赋值</param>
        /// <typeparam name="T">ID</typeparam>
        /// <param name="ID">o => o.ID.Equals(ID)</param>
        /// <param name="memberName">条件字段名称，如为Null，默认为主键字段</param>
        public Task CopyAsync<T>(T ID, Action<TEntity> act = null, string memberName = null) where T : struct
        {
            return Task.Factory.StartNew(() => Copy(ID, act, memberName));
        }

        /// <summary>
        ///     复制数据
        /// </summary>
        /// <param name="act">对新职的赋值</param>
        /// <typeparam name="T">ID</typeparam>
        /// <param name="lstIDs">o => IDs.Contains(o.ID)</param>
        /// <param name="memberName">条件字段名称，如为Null，默认为主键字段</param>
        public void Copy<T>(List<T> lstIDs, Action<TEntity> act = null, string memberName = null) where T : struct
        {
            Where(lstIDs, memberName);
            Copy(act);
        }
        /// <summary>
        ///     复制数据
        /// </summary>
        /// <param name="act">对新职的赋值</param>
        /// <typeparam name="T">ID</typeparam>
        /// <param name="lstIDs">o => IDs.Contains(o.ID)</param>
        /// <param name="memberName">条件字段名称，如为Null，默认为主键字段</param>
        public Task CopyAsync<T>(List<T> lstIDs, Action<TEntity> act = null, string memberName = null) where T : struct
        {
            return Task.Factory.StartNew(() => Copy(lstIDs, act, memberName));
        }

        #endregion

        #region Update

        /// <summary>
        ///     修改（支持延迟加载）
        ///     如果设置了主键ID，并且entity的ID设置了值，那么会自动将ID的值转换成条件 entity.ID == 值
        /// </summary>
        /// <param name="entity"></param>
        public int Update(TEntity entity)
        {
            Check.IsTure(entity == null && Queue.ExpBuilder.ExpAssign == null, "更新操作时，参数不能为空！");
            // 实体类的赋值，转成表达式树
            Queue.ExpBuilder.AssignUpdate(entity);
            // 加入队列
            return QueueManger.CommitLazy(SetMap, (queue) => Context.Executeor.Execute(queue.SqlBuilder.Update()), true);
        }
        /// <summary>
        ///     修改（支持延迟加载）
        ///     如果设置了主键ID，并且entity的ID设置了值，那么会自动将ID的值转换成条件 entity.ID == 值
        /// </summary>
        /// <param name="entity"></param>
        public Task<int> UpdateAsync(TEntity entity)
        {
            return Task.Factory.StartNew(() => Update(entity));
        }

        /// <summary>
        ///     更改实体类
        /// </summary>
        /// <param name="info">实体类</param>
        /// <typeparam name="T">ID</typeparam>
        /// <param name="ID">条件，等同于：o=>o.ID == ID 的操作</param>
        /// <param name="memberName">条件字段名称，如为Null，默认为主键字段</param>
        public int Update<T>(TEntity info, T ID, string memberName = null)
        {
            return Where(ID, memberName).Update(info);
        }
        /// <summary>
        ///     更改实体类
        /// </summary>
        /// <param name="info">实体类</param>
        /// <typeparam name="T">ID</typeparam>
        /// <param name="ID">条件，等同于：o=>o.ID == ID 的操作</param>
        /// <param name="memberName">条件字段名称，如为Null，默认为主键字段</param>
        public Task<int> UpdateAsync<T>(TEntity info, T ID, string memberName = null)
        {
            return Task.Factory.StartNew(() => Update(info, ID, memberName));
        }

        /// <summary>
        ///     更改实体类
        /// </summary>
        /// <param name="info">实体类</param>
        /// <typeparam name="T">ID</typeparam>
        /// <param name="lstIDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        /// <param name="memberName">条件字段名称，如为Null，默认为主键字段</param>
        public int Update<T>(TEntity info, List<T> lstIDs, string memberName = null)
        {
            return Where(lstIDs, memberName).Update(info);
        }
        /// <summary>
        ///     更改实体类
        /// </summary>
        /// <param name="info">实体类</param>
        /// <typeparam name="T">ID</typeparam>
        /// <param name="lstIDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        /// <param name="memberName">条件字段名称，如为Null，默认为主键字段</param>
        public Task<int> UpdateAsync<T>(TEntity info, List<T> lstIDs, string memberName = null) where T : struct
        {
            return Task.Factory.StartNew(() => Update(info, lstIDs, memberName));
        }

        #endregion

        #region Insert

        /// <summary>
        ///     插入
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="isReturnLastID">是否需要返回标识字段（如果设置的话）</param>
        public int Insert(TEntity entity, bool isReturnLastID = false)
        {
            Check.NotNull(entity, "插入操作时，参数不能为空！");

            // 实体类的赋值，转成表达式树
            Queue.ExpBuilder.AssignInsert(entity);

            // 需要返回值时，则不允许延迟提交
            if (isReturnLastID && SetMap.PhysicsMap.DbGeneratedFields.Key != null)
            {
                // 赋值标识字段
                return QueueManger.Commit(SetMap, (queue) =>
                {
                    PropertySetCacheManger.Cache(SetMap.PhysicsMap.DbGeneratedFields.Key, entity, ConvertHelper.ConvertType(Context.Executeor.GetValue<object>(queue.SqlBuilder.InsertIdentity()), SetMap.PhysicsMap.DbGeneratedFields.Key.PropertyType));
                    return 1;
                }, false);
            }

            // 不返回标识字段
            return QueueManger.CommitLazy(SetMap, (queue) => Context.Executeor.Execute(queue.SqlBuilder.Insert()), false);
        }
        /// <summary>
        ///     插入
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="isReturnLastID">是否需要返回标识字段（如果设置的话）</param>
        public Task<int> InsertAsync(TEntity entity, bool isReturnLastID = false)
        {
            return Task.Factory.StartNew(() => Insert(entity, isReturnLastID));
        }

        /// <summary>
        ///     插入
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <param name="identity">返回标识字段（如果设置的话）</param>
        public int Insert<T>(TEntity entity, out T identity)
        {
            Check.NotNull(entity, "插入操作时，entity参数不能为空！");

            var result = Insert(entity, true);
            // 获取标识字段
            identity = ConvertHelper.ConvertType(PropertyGetCacheManger.Cache(SetMap.PhysicsMap.DbGeneratedFields.Key, entity), default(T));
            return result;
        }

        /// <summary>
        ///     插入
        /// </summary>
        /// <param name="lst">实体类</param>
        public int Insert(List<TEntity> lst)
        {
            Check.NotNull(lst, "插入操作时，lst参数不能为空！");

            // 如果是MSSQLSER，则启用BulkCopy
            switch (Context.Executeor.DataBase.DataType)
            {
                case eumDbType.SqlServer:
                    QueueManger.CommitLazy(SetMap, (queue) => { Context.Executeor.DataBase.ExecuteSqlBulkCopy(SetMap.Name, lst.ToTable()); return lst.Count; }, false);
                    break;
                //case eumDbType.OleDb:
                //    lst.ForEach(entity => QueueManger.CommitLazy(SetMap, (queueLazy) => Context.Executeor.Execute(queueLazy.SqlBuilder.Insert()), false));
                //    break;
                default: lst.ForEach(entity => Insert(entity)); break;
            }
            return lst.Count;
        }

        /// <summary>
        ///     插入
        /// </summary>
        /// <param name="lst">实体类</param>
        public Task<int> InsertAsync(List<TEntity> lst)
        {
            return Task.Factory.StartNew(() => Insert(lst));
        }

        #endregion

        #region Delete

        /// <summary>
        ///     删除
        /// </summary>
        public int Delete()
        {
            if (SetMap.SortDelete != null)
            {
                Queue.ExpBuilder.AddAssign(SetMap.SortDelete.AssignExpression);
                return Update(null);
            }

            // 加入队列
            return QueueManger.CommitLazy(SetMap, (queue) => Context.Executeor.Execute(queue.SqlBuilder.Delete()), false);
        }
        /// <summary>
        ///     删除
        /// </summary>
        public Task<int> DeleteAsync()
        {
            return Task.Factory.StartNew(() => Delete());
        }

        /// <summary>
        ///     删除数据
        /// </summary>
        /// <param name="ID">条件，等同于：o=>o.ID.Equals(ID) 的操作</param>
        /// <typeparam name="T">ID</typeparam>
        /// <param name="memberName">条件字段名称，如为Null，默认为主键字段</param>
        public int Delete<T>(T ID, string memberName = null) where T : struct
        {
            return Where(ID, memberName).Delete();
        }

        /// <summary>
        ///     删除数据
        /// </summary>
        /// <param name="ID">条件，等同于：o=>o.ID.Equals(ID) 的操作</param>
        /// <typeparam name="T">ID</typeparam>
        /// <param name="memberName">条件字段名称，如为Null，默认为主键字段</param>
        public int Delete<T>(T? ID, string memberName = null) where T : struct
        {
            return Where(ID, memberName).Delete();
        }
        /// <summary>
        ///     删除数据
        /// </summary>
        /// <param name="ID">条件，等同于：o=>o.ID.Equals(ID) 的操作</param>
        /// <typeparam name="T">ID</typeparam>
        /// <param name="memberName">条件字段名称，如为Null，默认为主键字段</param>
        public Task<int> DeleteAsync<T>(T ID, string memberName = null) where T : struct
        {
            return Task.Factory.StartNew(() => Delete(ID, memberName));
        }

        /// <summary>
        ///     删除数据
        /// </summary>
        /// <param name="lstIDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        /// <typeparam name="T">ID</typeparam>
        /// <param name="memberName">条件字段名称，如为Null，默认为主键字段</param>
        public int Delete<T>(List<T> lstIDs, string memberName = null)
        {
            return Where(lstIDs, memberName).Delete();
        }
        /// <summary>
        ///     删除数据
        /// </summary>
        /// <param name="lstIDs">条件，等同于：o=> IDs.Contains(o.ID) 的操作</param>
        /// <typeparam name="T">ID</typeparam>
        /// <param name="memberName">条件字段名称，如为Null，默认为主键字段</param>
        public Task<int> DeleteAsync<T>(List<T> lstIDs, string memberName = null) where T : struct
        {
            return Task.Factory.StartNew(() => Delete(lstIDs, memberName));
        }

        #endregion

        #region AddUp

        /// <summary>
        ///     添加或者减少某个字段（支持延迟加载）
        /// </summary>
        public int AddUp()
        {
            Check.NotNull(Queue.ExpBuilder.ExpAssign, "+=字段操作时，必须先执行AddUp的另一个重载版本！");

            // 加入队列
            return QueueManger.CommitLazy(SetMap, (queue) => Context.Executeor.Execute(queue.SqlBuilder.AddUp()), true);
        }
        /// <summary>
        ///     添加或者减少某个字段（支持延迟加载）
        /// </summary>
        public Task<int> AddUpAsync()
        {
            return Task.Factory.StartNew(() => AddUp());
        }

        /// <summary>
        ///     添加或者减少某个字段（支持延迟加载）
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="fieldValue">要+=的值</param>
        public int AddUp<T>(Expression<Func<TEntity, T>> fieldName, T fieldValue) where T : struct
        {
            return AddAssign(fieldName, fieldValue).AddUp();
        }
        /// <summary>
        ///     添加或者减少某个字段（支持延迟加载）
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="fieldValue">要+=的值</param>
        public Task<int> AddUpAsync<T>(Expression<Func<TEntity, T>> fieldName, T fieldValue) where T : struct
        {
            return Task.Factory.StartNew(() => AddUp(fieldName, fieldValue));
        }

        /// <summary>
        ///     添加或者减少某个字段（支持延迟加载）
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="fieldValue">要+=的值</param>
        public int AddUp<T>(Expression<Func<TEntity, T?>> fieldName, T fieldValue) where T : struct
        {
            return AddAssign(fieldName, fieldValue).AddUp();
        }
        /// <summary>
        ///     添加或者减少某个字段（支持延迟加载）
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="fieldValue">要+=的值</param>
        public Task<int> AddUpAsync<T>(Expression<Func<TEntity, T?>> fieldName, T fieldValue) where T : struct
        {
            return Task.Factory.StartNew(() => AddUp(fieldName, fieldValue));
        }

        /// <summary>
        ///     更新单个字段值
        /// </summary>
        /// <typeparam name="T">更新的值类型</typeparam>
        /// <param name="select"></param>
        /// <param name="fieldValue">要更新的值</param>
        /// <param name="ID">o => o.ID.Equals(ID)</param>
        /// <param name="memberName">条件字段名称，如为Null，默认为主键字段</param>
        public int AddUp<T>(T? ID, Expression<Func<TEntity, T?>> select, T fieldValue, string memberName = null) where T : struct
        {
            return Where(ID, memberName).AddUp(select, fieldValue);
        }
        /// <summary>
        ///     更新单个字段值
        /// </summary>
        /// <typeparam name="T">更新的值类型</typeparam>
        /// <param name="select"></param>
        /// <param name="fieldValue">要更新的值</param>
        /// <param name="ID">o => o.ID.Equals(ID)</param>
        /// <param name="memberName">条件字段名称，如为Null，默认为主键字段</param>
        public Task<int> AddUpAsync<T>(T? ID, Expression<Func<TEntity, T?>> select, T fieldValue, string memberName = null) where T : struct
        {
            return Task.Factory.StartNew(() => AddUp(ID, select, fieldValue, memberName));
        }

        /// <summary>
        ///     更新单个字段值
        /// </summary>
        /// <typeparam name="T">更新的值类型</typeparam>
        /// <param name="select"></param>
        /// <param name="fieldValue">要更新的值</param>
        /// <param name="ID">o => o.ID.Equals(ID)</param>
        /// <param name="memberName">条件字段名称，如为Null，默认为主键字段</param>
        public int AddUp<T>(T ID, Expression<Func<TEntity, T>> select, T fieldValue, string memberName = null) where T : struct
        {
            return Where(ID, memberName).AddUp(select, fieldValue);
        }
        /// <summary>
        ///     更新单个字段值
        /// </summary>
        /// <typeparam name="T">更新的值类型</typeparam>
        /// <param name="select"></param>
        /// <param name="fieldValue">要更新的值</param>
        /// <param name="ID">o => o.ID.Equals(ID)</param>
        /// <param name="memberName">条件字段名称，如为Null，默认为主键字段</param>
        public Task<int> AddUpAsync<T>(T ID, Expression<Func<TEntity, T>> select, T fieldValue, string memberName = null) where T : struct
        {
            return Task.Factory.StartNew(() => AddUp(ID, select, fieldValue, memberName));
        }

        /// <summary>
        ///     更新单个字段值
        /// </summary>
        /// <param name="fieldValue">要更新的值</param>
        /// <typeparam name="T">ID</typeparam>
        /// <param name="ID">o => o.ID.Equals(ID)</param>
        public int AddUp<T>(T? ID, T fieldValue) where T : struct
        {
            return AddUp<T>(ID, null, fieldValue);
        }
        /// <summary>
        ///     更新单个字段值
        /// </summary>
        /// <param name="fieldValue">要更新的值</param>
        /// <typeparam name="T">ID</typeparam>
        /// <param name="ID">o => o.ID.Equals(ID)</param>
        public Task<int> AddUpAsync<T>(T? ID, T fieldValue) where T : struct
        {
            return Task.Factory.StartNew(() => AddUp(ID, fieldValue));
        }
        #endregion
    }
}