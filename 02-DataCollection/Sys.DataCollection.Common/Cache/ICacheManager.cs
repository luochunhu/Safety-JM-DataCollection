using Sys.DataCollection.Common.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.DataCollection.Common.Cache
{
    /// <summary>
    /// 通用缓存接口
    /// </summary>
    public interface ICacheManager
    {
        /// <summary>
        /// 新增一个缓存对象
        /// </summary>
        /// <typeparam name="TEntity">缓存对象类型</typeparam>
        /// <param name="item">新增的缓存对象</param>
        /// <returns>操作结果；True:成功；False:失败</returns>
        bool AddItem<TEntity>(TEntity item) where TEntity : CacheInfo;
        /// <summary>
        /// 更新一个缓存对象
        /// </summary>
        /// <typeparam name="TEntity">缓存对象类型</typeparam>
        /// <param name="item">修改的缓存对象</param>
        /// <returns>操作结果；True:成功；False:失败</returns>
        bool UpdateItem<TEntity>(TEntity item) where TEntity : CacheInfo;
        /// <summary>
        /// 删除一个缓存对象
        /// </summary>
        /// <typeparam name="TEntity">缓存对象类型</typeparam>
        /// <param name="item">删除的缓存对象</param>
        /// <returns>操作结果；True:成功；False:失败</returns>
        bool DeleteItem<TEntity>(TEntity item) where TEntity : CacheInfo;
        /// <summary>
        /// 根据Key获取一个缓存项
        /// </summary>
        /// <typeparam name="TEntity">缓存对象类型</typeparam>
        /// <param name="key">主键名称</param>
        /// <typeparam name="isCopy">查询时是否需要深复制（默认false）</typeparam>
        /// <returns>查询结果；返回为Null则表示未获取到缓存</returns>
        TEntity GetItemByKey<TEntity>(string key, bool isCopy = false) where TEntity : CacheInfo;
        /// <summary>
        /// 查询缓存所有集合
        /// </summary>
        /// <typeparam name="TEntity">缓存对象类型</typeparam>
        /// <typeparam name="isCopy">查询时是否需要深复制（默认false）</typeparam>
        /// <returns>查询结果</returns>
        List<TEntity> GetAll<TEntity>(bool isCopy = false) where TEntity : CacheInfo;
        /// <summary>
        /// 根据表达式查询集合
        /// </summary>
        /// <typeparam name="TEntity">缓存对象类型</typeparam>
        /// <param name="predicate">条件表达式</param>
        /// <typeparam name="isCopy">查询时是否需要深复制（默认false）</typeparam>
        /// <returns>查询结果；返回为空或者list.Count=0则表示未查询到结果</returns>
        List<TEntity> Query<TEntity>(Func<TEntity, bool> predicate, bool isCopy = false) where TEntity : CacheInfo;
        /// <summary>
        /// 根据表达式查询一个缓存
        /// </summary>
        /// <typeparam name="TEntity">缓存对象类型</typeparam>
        /// <param name="predicate">条件表达式</param>
        /// <typeparam name="isCopy">查询时是否需要深复制（默认false）</typeparam>
        /// <returns>查询结果；返回为Null则表示未获取到缓存</returns>
        TEntity QueryFrist<TEntity>(Func<TEntity, bool> predicate, bool isCopy = false) where TEntity : CacheInfo;
    }
}
