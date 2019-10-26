using Sys.DataCollection.Common.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sys.DataCollection.Common.Protocols;
using System.Threading;
using Basic.Framework.Common;
using Basic.Framework.Logging;

namespace Sys.DataCollection.Cache
{
    /// <summary>
    /// 缓存管理器
    /// </summary>
    public partial class CacheManager : ICacheManager
    {
        //缓存字典，以一个缓存对象类型做为key
        Dictionary<Type, Dictionary<string, CacheInfo>> _cache;

        //读写锁集合，以一个缓存类型为一个单位
        Dictionary<Type, ReaderWriterLock> _rwLockers;

        public CacheManager()
        {
            _cache = new Dictionary<Type, Dictionary<string, CacheInfo>>();
            _rwLockers = new Dictionary<Type, ReaderWriterLock>();
        }        

        /// <summary>
        /// 新增多个缓存集合（主要用于初始化）
        /// </summary>
        /// <typeparam name="TEntity">缓存对象类型</typeparam>
        /// <param name="list">新增的缓存集合</param>
        /// <returns>操作结果；True:成功；False:失败</returns>
        public bool AddItems<TEntity>(List<TEntity> list) where TEntity : CacheInfo
        {
            Type type = typeof(TEntity);
            if (!_cache.ContainsKey(type))
            {
                //判断有无此类型的缓存，如果没有，则直接实例化新的缓存集合及新增一个读写锁
                _cache.Add(type, new Dictionary<string, CacheInfo>());
                _rwLockers.Add(type, new ReaderWriterLock());
            }

            _rwLockers[type].AcquireWriterLock(-1);

            try
            {
                foreach (var item in list)
                {
                    if (!_cache[type].ContainsKey(item.UniqueKey))
                    {
                        _cache[type].Add(item.UniqueKey, item);
                    }
                    else
                    {
                        //待确认key已经存在时，是否做更新处理
                    }
                }               
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.ToString());
                return false;
            }
            finally
            {
                _rwLockers[type].ReleaseWriterLock();
            }

            return true;

        }

        /// <summary>
        /// 新增一个缓存对象
        /// </summary>
        /// <typeparam name="TEntity">缓存对象类型</typeparam>
        /// <param name="item">新增的缓存对象</param>
        /// <returns>操作结果；True:成功；False:失败</returns>
        public bool AddItem<TEntity>(TEntity item) where TEntity : CacheInfo
        {
            Type type = typeof(TEntity);
            if (!_cache.ContainsKey(type))
            {
                //判断有无此类型的缓存，如果没有，则直接实例化新的缓存集合及新增一个读写锁
                _cache.Add(type, new Dictionary<string, CacheInfo>());
                _rwLockers.Add(type, new ReaderWriterLock());
            }

            _rwLockers[type].AcquireWriterLock(-1);

            try
            {
                if (!_cache[type].ContainsKey(item.UniqueKey))
                {
                    _cache[type].Add(item.UniqueKey, item);
                }
                else
                {
                    //待确认key已经存在时，是否做更新处理
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.ToString());
                return false;
            }
            finally
            {
                _rwLockers[type].ReleaseWriterLock();
            }

            return true;
        }
        /// <summary>
        /// 更新一个缓存对象
        /// </summary>
        /// <typeparam name="TEntity">缓存对象类型</typeparam>
        /// <param name="item">修改的缓存对象</param>
        /// <returns>操作结果；True:成功；False:失败</returns>
        public bool UpdateItem<TEntity>(TEntity item) where TEntity : CacheInfo
        {
            Type type = typeof(TEntity);
            if (!_cache.ContainsKey(type))
            {
                return false;
            }

            _rwLockers[type].AcquireWriterLock(-1);

            try
            {
                if (_cache[type].ContainsKey(item.UniqueKey))
                {
                    _cache[type][item.UniqueKey] = item;
                }
                else
                {
                    //待确认更新无此对象时，是否做新增处理
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.ToString());
                return false;
            }
            finally
            {
                _rwLockers[type].ReleaseWriterLock();
            }

            return true;
        }
        /// <summary>
        /// 删除一个缓存对象
        /// </summary>
        /// <typeparam name="TEntity">缓存对象类型</typeparam>
        /// <param name="item">删除的缓存对象</param>
        /// <returns>操作结果；True:成功；False:失败</returns>
        public bool DeleteItem<TEntity>(TEntity item) where TEntity : CacheInfo
        {
            Type type = typeof(TEntity);           
            if (!_cache.ContainsKey(type))
            {
                return false;
            }
            _rwLockers[type].AcquireWriterLock(-1);

            try
            {
                if (_cache[type].ContainsKey(item.UniqueKey))
                {
                    _cache[type].Remove(item.UniqueKey);
                }
                else
                {
                    //待确认更新无此对象时，是否做新增处理
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.ToString());
                return false;
            }
            finally
            {
                _rwLockers[type].ReleaseWriterLock();
            }

            return true;
        }
        /// <summary>
        /// 根据Key获取一个缓存项
        /// </summary>
        /// <typeparam name="TEntity">缓存对象类型</typeparam>
        /// <param name="key">主键名称</param>
        /// <typeparam name="isCopy">查询时是否需要深复制（默认false）</typeparam>
        /// <returns>查询结果；返回为Null则表示未获取到缓存</returns>
        public TEntity GetItemByKey<TEntity>(string key, bool isCopy = false) where TEntity : CacheInfo
        {
            Type type = typeof(TEntity);           
            if (!_cache.ContainsKey(type))
            {
                return null;
            }

            _rwLockers[type].AcquireReaderLock(-1);

            try
            {
                if (_cache[type].ContainsKey(key))
                {
                    var entity = _cache[type][key] as TEntity;
                    if (entity != null && isCopy)
                    {
                        return ObjectConverter.DeepCopy<TEntity>(entity);
                    }
                    else
                    {
                        return entity;
                    }
                }
                else
                {                   
                    return null;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.ToString());
                return null;
            }
            finally
            {
                _rwLockers[type].ReleaseReaderLock();
            }
        }

        /// <summary>
        /// 查询缓存所有集合
        /// </summary>
        /// <typeparam name="TEntity">缓存对象类型</typeparam>
        /// <typeparam name="isCopy">查询时是否需要深复制（默认false）</typeparam>
        /// <returns>查询结果</returns>
        public List<TEntity> GetAll<TEntity>(bool isCopy = false) where TEntity : CacheInfo
        {
            Type type = typeof(TEntity);
            if (!_cache.ContainsKey(type))
            {
                return null;
            }
            _rwLockers[type].AcquireReaderLock(-1);

            try
            {
                //todo 待确认性能问题或者优化方案  20170531                 
                var listInfos = _cache[type].Values.ToList();
                List<TEntity> list = new List<TEntity>();
                foreach (var item in listInfos)
                {
                    list.Add(item as TEntity);
                }

                //todo 待确认是否需要判断为空处理   
                if (isCopy)
                {
                    //深复制
                    return ObjectConverter.DeepCopy<List<TEntity>>(list);
                }
                else
                {
                    return list;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.ToString());
                return new List<TEntity>();
            }
            finally
            {
                _rwLockers[type].ReleaseReaderLock();
            }

        }

        /// <summary>
        /// 根据表达式查询集合
        /// </summary>
        /// <typeparam name="TEntity">缓存对象类型</typeparam>
        /// <param name="predicate">条件表达式</param>
        /// <typeparam name="isCopy">查询时是否需要深复制（默认false）</typeparam>
        /// <returns>查询结果；返回为空或者list.Count=0则表示未查询到结果</returns>
        public List<TEntity> Query<TEntity>(Func<TEntity, bool> predicate, bool isCopy = false) where TEntity : CacheInfo
        {
            Type type = typeof(TEntity);
            if (!_cache.ContainsKey(type))
            {
                return null;
            }
            _rwLockers[type].AcquireReaderLock(-1);

            try
            {
                //todo 待确认性能问题或者优化方案  20170531                 
                var listInfos = _cache[type].Values.ToList();
                List<TEntity> list = new List<TEntity>();
                foreach (var item in listInfos)
                {
                    list.Add(item as TEntity);
                }
                //todo 待确认是否需要判断为空处理
                var result= list.Where(predicate).ToList();

                if (isCopy)
                {
                    //深复制
                    return ObjectConverter.DeepCopy<List<TEntity>>(result);
                }
                else
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.ToString());
                return new List<TEntity>();
            }
            finally
            {
                _rwLockers[type].ReleaseReaderLock();
            }            
        }
        /// <summary>
        /// 根据表达式查询一个缓存
        /// </summary>
        /// <typeparam name="TEntity">缓存对象类型</typeparam>
        /// <param name="predicate">条件表达式</param>
        /// <typeparam name="isCopy">查询时是否需要深复制（默认false）</typeparam>
        /// <returns>查询结果；返回为Null则表示未获取到缓存</returns>
        public TEntity QueryFrist<TEntity>(Func<TEntity, bool> predicate, bool isCopy = false) where TEntity : CacheInfo
        {
            var queryList = Query<TEntity>(predicate, isCopy);
            if (queryList != null && queryList.Count > 0)
            {
                return queryList.FirstOrDefault();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 清空所有缓存
        /// </summary>
        public void ClearCache()
        {
            try
            {
                //todo 待确认这里不加锁是否有并发问题
                _cache.Clear();
                _rwLockers.Clear();
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.ToString());
            }
        }

    }
}
