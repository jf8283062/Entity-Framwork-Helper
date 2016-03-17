namespace Cn.Steelv.Trade.DBEntity
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Linq.Expressions;
    public static class SimpleHelp
    {
        /// <summary>
        /// 创建连接
        /// </summary>
        /// <returns></returns>
        public static DbContext CreateConnection()
        {

            var _TContext = new tradedbEntities() as System.Data.Entity.DbContext;
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)_TContext).ObjectContext.CommandTimeout = 1000;
            return _TContext;
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static T Add<T>(T entity) where
            T : class
        {
            using (var db = CreateConnection())
            {
                var dbSet = db.Set<T>();
                var temp = dbSet.Add(entity);
                db.SaveChanges();
                return temp;
            }
        }

        /// <summary>
        /// 根据ID删除数据
        /// </summary>
        /// <typeparam name="TModel">Entity类型</typeparam>
        /// <param name="id">唯一主键</param>
        public static void Delete<TModel>(int id) where
            TModel : class
        {
            using (var ctx = CreateConnection())
            {
                var entity = Get<TModel>(id);
                ctx.Entry(entity).State = EntityState.Deleted;
                ctx.Set<TModel>().Remove(entity);
                ctx.SaveChanges();
            }
        }

        /// <summary>
        /// 获取实体对象
        /// </summary>
        /// <typeparam name="TModel">Entity类型</typeparam>
        /// <param name="id">唯一主键</param>
        /// <returns>实体对象</returns>
        public static TModel Get<TModel>(int id) where
            TModel : class
        {
            using (var _context = CreateConnection())
            {
                return _context.Set<TModel>().Find(id);
            }
        }


        /// <summary>
        /// 更新实体,全部字段全部更新
        /// </summary>
        /// <typeparam name="TModel">Entity类型</typeparam>
        /// <param name="entity">更新的对象实体,必须传唯一主键</param>
        /// <returns>更新数量</returns>
        public static int Update<TModel>(TModel entity)
           where TModel : class
        {
            using (var _context = CreateConnection())
            {
                var dbSet = _context.Set<TModel>();
                _context.Entry(entity).State = EntityState.Modified;
                return _context.SaveChanges();
            }
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <typeparam name="TModel">Entity类型</typeparam>
        /// <param name="model">更新的对象实体,必须传唯一主键</param>
        /// <param name="filed">更新那些列</param>
        /// <returns>更新数量</returns>
        public static int Update<TModel>(TModel model, List<Expression<Func<TModel, object>>> filed)
            where TModel : class
        {
            using (var _context = CreateConnection())
            {

                System.Data.Entity.Infrastructure.DbEntityEntry<TModel> entry = _context.Entry<TModel>(model);
                entry.State = System.Data.Entity.EntityState.Unchanged;
                foreach (var item in filed)
                {
                    entry.Property(item).IsModified = true;
                }
                return _context.SaveChanges();
            }
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <typeparam name="TModel">Entity类型</typeparam>
        /// <param name="model">更新的对象实体,必须传唯一主键</param>
        /// <param name="filed">更新那些列</param>
        /// <returns>更新数量</returns>
        public static int Update<TModel>(TModel model, params Expression<Func<TModel, object>>[] filed)
           where TModel : class
        {
            using (var _context = CreateConnection())
            {

                System.Data.Entity.Infrastructure.DbEntityEntry<TModel> entry = _context.Entry<TModel>(model);

                entry.State = System.Data.Entity.EntityState.Unchanged;
                if (filed.Count() > 0)
                {
                    foreach (var item in filed)
                    {
                        entry.Property(item).IsModified = true;
                    }
                }
                _context.Configuration.ValidateOnSaveEnabled = false;
                int number = _context.SaveChanges();
                _context.Configuration.ValidateOnSaveEnabled = true;
                return number;
            }
        }

        /// <summary>
        /// 获取List接口对象
        /// </summary>
        /// <typeparam name="TModel">Entity类型</typeparam>
        /// <param name="query">查询参数</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="count">查询总数</param>
        /// <param name="_context">EF上下文</param>
        /// <returns>查询出的List接口对象</returns>
        public static List<TModel> GetList<TModel, TKey>(IEnumerable<Expression<Func<TModel, bool>>> query, int pageIndex, int pageSize, out int count, Expression<Func<TModel, TKey>> order, bool desc = true, DbContext _context = null)
            where TModel : class
        {
            if (_context == null)
            {
                using (_context = CreateConnection())
                {

                    IQueryable<TModel> dbSet = _context.Set<TModel>();
                    dbSet = dbSet.AsNoTracking();
                    if (query.Count() > 0)
                    {
                        foreach (var item in query)
                        {
                            dbSet = dbSet.Where(item);
                        }
                    }
                    count = dbSet.Count();
                    if (desc)
                        return dbSet.OrderByDescending(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                    else
                        return dbSet.OrderBy(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                }
            }
            else
            {
                IQueryable<TModel> dbSet = _context.Set<TModel>();
                dbSet = dbSet.AsNoTracking();
                if (query.Count() > 0)
                {
                    foreach (var item in query)
                    {
                        dbSet = dbSet.Where(item);
                    }
                }
                count = dbSet.Count();
                if (desc)
                    return dbSet.OrderByDescending(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                else
                    return dbSet.OrderBy(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }

        }


        /// <summary>
        /// 获取list
        /// </summary>
        /// <typeparam name="TModel">Entity类型</typeparam>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="query">查询条件</param>
        /// <param name="selector">查询结果</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="count">查询总数</param>
        /// <param name="_context">EF上下文</param>
        /// <returns>查询出的List接口对象</returns>
        public static List<T> GetList<TModel, T, TKey>(IEnumerable<Expression<Func<TModel, bool>>> query, Expression<Func<TModel, T>> selector, int pageIndex, int pageSize, out int count, Expression<Func<TModel, TKey>> order, bool desc = true, DbContext _context = null)
            where TModel : class
        {
            if (_context == null)
            {
                using (_context = CreateConnection())
                {
                    IQueryable<TModel> dbSet = _context.Set<TModel>();
                    dbSet = dbSet.AsNoTracking();
                    if (query.Count() > 0)
                    {
                        foreach (var item in query)
                        {
                            dbSet = dbSet.Where(item);
                        }
                    }
                    count = dbSet.Count();
                    if (desc)
                        return dbSet.OrderByDescending(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).Select(selector).ToList();
                    else
                        return dbSet.OrderBy(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).Select(selector).ToList();
                }
            }
            else
            {
                IQueryable<TModel> dbSet = _context.Set<TModel>();
                dbSet = dbSet.AsNoTracking();
                if (query.Count() > 0)
                {
                    foreach (var item in query)
                    {
                        dbSet = dbSet.Where(item);
                    }
                }
                count = dbSet.Count();
                if (desc)
                    return dbSet.OrderByDescending(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).Select(selector).ToList();
                else
                    return dbSet.OrderBy(order).Skip((pageIndex - 1) * pageSize).Take(pageSize).Select(selector).ToList();
            }

        }

        /// <summary>
        /// 获取单个实体对象
        /// </summary>
        /// <typeparam name="TModel">Entity类型</typeparam>
        /// <param name="query">查询参数</param>
        /// <param name="_context">EF上下文</param>
        /// <returns>查询出的实体对象</returns>
        public static TModel Get<TModel>(IEnumerable<Expression<Func<TModel, bool>>> query, DbContext _context=null)
            where TModel : class
        {
            if (_context == null)
            {
                using (_context = CreateConnection())
                {

                    IQueryable<TModel> dbSet = _context.Set<TModel>();
                    dbSet = dbSet.AsNoTracking();
                    foreach (var item in query)
                    {
                        dbSet = dbSet.Where(item);
                    }

                    return dbSet.FirstOrDefault();
                }
            }
            else
            {
                IQueryable<TModel> dbSet = _context.Set<TModel>();
                dbSet = dbSet.AsNoTracking();
                foreach (var item in query)
                {
                    dbSet = dbSet.Where(item);
                }
                return dbSet.FirstOrDefault();
            }

        }

        /// <summary>
        /// 获取单条数据部分信息
        /// </summary>
        /// <typeparam name="TModel">entity 实体类型</typeparam>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="query">查询条件</param>
        /// <param name="select">查询字段</param>
        /// <param name="_context">EF上下文</param>
        /// <returns></returns>
        public static T GetSelectort<TModel, T>(IEnumerable<Expression<Func<TModel, bool>>> query, Expression<Func<TModel, T>> select, DbContext _context = null)
            where TModel : class
        {
            if (_context == null)
            {
                using (_context = CreateConnection())
                {
                    IQueryable<TModel> dbSet = _context.Set<TModel>();
                    dbSet = dbSet.AsNoTracking();
                    foreach (var item in query)
                    {
                        dbSet = dbSet.Where(item);
                    }

                    return dbSet.Select(select).FirstOrDefault();
                }
            }
            else
            {
                IQueryable<TModel> dbSet = _context.Set<TModel>();
                dbSet = dbSet.AsNoTracking();
                foreach (var item in query)
                {
                    dbSet = dbSet.Where(item);
                }

                return dbSet.Select(select).FirstOrDefault();
            }

        }
        /// <summary>
        /// 获取单条数据部分信息
        /// </summary>
        /// <typeparam name="TModel">entity实体</typeparam>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="query">查询参数</param>
        /// <param name="selector">查询字段</param>
        /// <param name="_context">EF上下文</param>
        /// <returns></returns>
        public static T GetSelectort<TModel, T>(Expression<Func<TModel, bool>> query, Expression<Func<TModel, T>> selector, DbContext _context=null)
           where TModel : class
        {
            if (_context==null)
            {
                using (_context = CreateConnection())
                {

                    IQueryable<TModel> dbSet = _context.Set<TModel>();
                    dbSet = dbSet.AsNoTracking();
                    dbSet = dbSet.Where(query);
                    return dbSet.Select(selector).FirstOrDefault();
                }
            }
            else
            {
                IQueryable<TModel> dbSet = _context.Set<TModel>();
                dbSet = dbSet.AsNoTracking();
                dbSet = dbSet.Where(query);
                return dbSet.Select(selector).FirstOrDefault();
            }
            
        }

        /// <summary>
        /// 获取单个实体对象
        /// </summary>
        /// <typeparam name="TModel">Entity类型</typeparam>
        /// <param name="query">查询参数</param>
        /// <returns>查询出的实体对象</returns>
        public static TModel Get<TModel>(params Expression<Func<TModel, bool>>[] query)
            where TModel : class
        {
            using (var _context = CreateConnection())
            {

                IQueryable<TModel> dbSet = _context.Set<TModel>();
                dbSet = dbSet.AsNoTracking();
                foreach (var item in query)
                {
                    dbSet = dbSet.Where(item);
                }

                return dbSet.FirstOrDefault();
            }
        }


        /// <summary>
        /// 获取单个实体对象
        /// </summary>
        /// <typeparam name="TModel">Entity类型</typeparam>
        /// <param name="query">查询参数</param>
        /// <param name="_context">EF上下文</param>
        /// <returns>查询出的实体对象</returns>
        public static TModel Get<TModel>(Expression<Func<TModel, bool>> query, DbContext _context=null)
            where TModel : class
        {
            if (_context==null)
            {
                using (_context = CreateConnection())
                {

                    IQueryable<TModel> dbSet = _context.Set<TModel>();
                    dbSet = dbSet.AsNoTracking();

                    dbSet = dbSet.Where(query);

                    return dbSet.FirstOrDefault();
                }
            }
            else
            {
                IQueryable<TModel> dbSet = _context.Set<TModel>();
                dbSet = dbSet.AsNoTracking();

                dbSet = dbSet.Where(query);

                return dbSet.FirstOrDefault();
            }
            
        }


        /// <summary>
        /// 获取查询数据总数
        /// </summary>
        /// <typeparam name="TModel">Entity类型</typeparam>
        /// <param name="query">参数</param>
        /// <param name="_context">EF上下文</param>
        /// <returns></returns>
        public static int GetCount<TModel>(IEnumerable<Expression<Func<TModel, bool>>> query, DbContext _context= null)
            where TModel : class
        {
            if (_context==null)
            {
                using (_context = CreateConnection())
                {
                    IQueryable<TModel> dbSet = _context.Set<TModel>();
                    dbSet = dbSet.AsNoTracking();
                    foreach (var item in query)
                    {
                        dbSet = dbSet.Where(item);
                    }
                    return dbSet.Count();
                }
            }
            else
            {
                IQueryable<TModel> dbSet = _context.Set<TModel>();
                dbSet = dbSet.AsNoTracking();
                foreach (var item in query)
                {
                    dbSet = dbSet.Where(item);
                }
                return dbSet.Count();
            }
           
        }


        /// <summary>
        /// 批量添加数据
        /// </summary>
        /// <typeparam name="TModel">Entity类型</typeparam>
        /// <param name="entities">添加对象实体</param>
        /// <returns>添加数量</returns>
        public static int Add<TModel>(params TModel[] entities)
            where TModel : class
        {
            using (var _context = CreateConnection())
            {
                foreach (var entity in entities)
                {
                    _context.Entry(entity).State = EntityState.Added;
                    _context.Set<TModel>().Add(entity);
                }
                return _context.SaveChanges();
            }
        }

        /// <summary>
        /// 批量修改有唯一主键的实体
        /// </summary>
        /// <typeparam name="TModel">Entity类型</typeparam>
        /// <param name="entities">修改的对象实体</param>
        /// <returns>修改数量</returns>
        public static int UpdateMany<TModel>(IEnumerable<TModel> entities)
            where TModel : class
        {
            using (var _context = CreateConnection())
            {
                foreach (var entity in entities)
                {
                    _context.Entry(entity).State = EntityState.Modified;

                }
                return _context.SaveChanges();
            }
        }

        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <typeparam name="T">Entity类型</typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">sql参数</param>
        /// <returns></returns>
        public static List<T> ExecSqlQuery<T>(string sql, params SqlParameter[] parameters)
        {
            return ExecSqlQuery<T, int>(sql, null, false, 0, 0, parameters);
        }

        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <typeparam name="T">Entity类型</typeparam>
        /// <typeparam name="TKey">返回类型</typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="orderby">排序</param>
        /// <param name="asc">排序方式是否是正序</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="PageSize">每页数据量</param>
        /// <param name="parameters">sql参数</param>
        /// <returns></returns>
        public static List<T> ExecSqlQuery<T, TKey>(string sql, Func<T, TKey> orderby, bool asc, int pageIndex, int PageSize, params SqlParameter[] parameters)
        {
            using (var _context = CreateConnection())
            {
                ((System.Data.Entity.Infrastructure.IObjectContextAdapter)_context).ObjectContext.CommandTimeout = 0;
                System.Data.Entity.Infrastructure.DbRawSqlQuery<T> query = _context.Database.SqlQuery<T>(sql, parameters);
                IOrderedEnumerable<T> data = null;
                if (orderby != null)
                {
                    if (asc)
                        data = query.OrderBy(orderby);
                    else
                        data = query.OrderByDescending(orderby);
                }
                if (pageIndex > 0 && PageSize > 0)
                {
                    return data.Skip((pageIndex - 1) * PageSize).Take(PageSize).ToList();

                }
                return query.ToList();
            }
        }

        /// <summary>
        /// 执行无返回值sql语句
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">sql参数</param>
        /// <returns>影响行数</returns>
        public static int ExecSqlCommand(string sql, params SqlParameter[] parameters)
        {
            using (var _context = CreateConnection())
            {
                ((System.Data.Entity.Infrastructure.IObjectContextAdapter)_context).ObjectContext.CommandTimeout = 0;
                return _context.Database.ExecuteSqlCommand(sql, parameters);
            }
        }

        /// <summary>
        /// 获取全部数据
        /// </summary>
        /// <typeparam name="TModel">Entity对象类型</typeparam>
        /// <param name="_context">EF上下文</param>
        /// <returns></returns>
        public static object GetAll<TModel>(DbContext _context =null)
            where TModel : class
        {
            if (_context==null)
            {
                using (_context = CreateConnection())
                {
                    IQueryable<TModel> dbSet = _context.Set<TModel>();
                    dbSet = dbSet.AsNoTracking();
                    return dbSet.ToList();
                }
            }
            else
            {
                IQueryable<TModel> dbSet = _context.Set<TModel>();
                dbSet = dbSet.AsNoTracking();
                return dbSet.ToList();
            }
           
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <typeparam name="TModel">Entity实体</typeparam>
        /// <param name="db">EF上下文</param>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public static TModel Get<TModel>(DbContext _context, int id)
            where TModel : class
        {
            return _context.Set<TModel>().Find(id);
        }
    }
}
