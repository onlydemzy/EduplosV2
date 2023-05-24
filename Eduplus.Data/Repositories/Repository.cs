using KS.Core;
using KS.Data.Contract;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace KS.Data.Repositories
{
    public class Repository<T> : IRepository<T>
        where T : class
    {
        #region Members

        internal readonly IDbSet<T> _dbSet;

        #endregion

        #region Constructor

        /// <summary>
        /// Create a new instance of repository
        /// </summary>
        /// <param name="unitOfWork">Associated Unit Of Work</param>
        public Repository(IDbSet<T> dbSet)
        {
            if (dbSet == null)
                throw new ArgumentNullException("dbSet");

            _dbSet = dbSet;
        }

        #endregion

        #region IRepository Members
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public virtual T Add(T item)
        {

            if (item != (T)null)
            {
                _dbSet.Add(item);
                return item;
            }
                 // add new item in this set
            else
            {
                //LoggerFactory.CreateLog()
                //.LogInfo(Message.info_CannotAddNullEntity, typeof(T).ToString());

                return null; //letter implementation

            }

        }
        public virtual T AddValue(T item)
        {

            if (item != (T)null)
            {
                var t =_dbSet.Add(item);
                

                return t;
            }
                // add new item in this set
            else
            {
                //LoggerFactory.CreateLog()
                //.LogInfo(Message.info_CannotAddNullEntity, typeof(T).ToString());
                //letter implementation
                return null;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public virtual void Remove(T item)
        {
            if (item != (T)null)
            {
                //attach item if not exist
                _dbSet.Attach(item);

                //set as "removed"
                _dbSet.Remove(item);
            }
            else
            {
                //LoggerFactory.CreateLog()
                          //.LogInfo(Message.info_CannotRemoveNullEntity, typeof(T).ToString());
                          //letter implementaiton
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public virtual void TrackItem(T item)
        {
            if (item != (T)null)
                _dbSet.Attach(item);
            else
            {
                //LoggerFactory.CreateLog()
                          //.LogInfo(Message.info_CannotRemoveNullEntity, typeof(T).ToString());
            }
        }

       
        public virtual T Get(long id)
        {
            if (id != 0)
                return _dbSet.Find(id);

            else
                return null;

        }
        public virtual T Get(string id)
        {
            if (!string.IsNullOrEmpty(id))
                return _dbSet.Find(id);

            else
                return null;

        }

        public T GetSingle(Expression<Func<T, bool>> filter)
        {
            if (filter != null)
                return _dbSet.Where(filter).SingleOrDefault();
            else return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<T> GetAll()
        {
            return _dbSet;
        }

       

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="KProperty"></typeparam>
        /// <param name="pageIndex"></param>
        /// <param name="pageCount"></param>
        /// <param name="orderByExpression"></param>
        /// <param name="ascending"></param>
        /// <returns></returns>
        public virtual IEnumerable<T> GetPaged<KProperty>(int pageIndex, int pageCount, Expression<Func<T, KProperty>> orderByExpression, bool ascending)
        {
            var set = _dbSet;

            if (ascending)
            {
                return set.OrderBy(orderByExpression)
                          .Skip(pageCount * pageIndex)
                          .Take(pageCount);
            }
            else
            {
                return set.OrderByDescending(orderByExpression)
                          .Skip(pageCount * pageIndex)
                          .Take(pageCount);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public virtual  IEnumerable<T> GetFiltered<KProperty>(Expression<Func<T, bool>> filter,Expression<Func<T,KProperty>> orderByExpression,bool ascending)
        {
            if (ascending)
                return _dbSet.Where(filter).OrderBy(orderByExpression);
            else return _dbSet.Where(filter).OrderByDescending(orderByExpression);
            
        }
        public IEnumerable<T> GetFiltered(Expression<Func<T, bool>> filter)
        {
            return _dbSet.Where(filter);
        }
        public IDbSet<T> GetSet()
        {

            return _dbSet;
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// <see cref="M:System.IDisposable.Dispose"/>
        /// </summary>
        public void Dispose()
        {
            //if (_dbSet != null)
                //_dbSet.Dispose();
        }

        #endregion

        #region Private Methods

        
        #endregion

    }
}
