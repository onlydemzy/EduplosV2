using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace KS.Core
{
    /// <summary>
    /// Base interface to implement Repository Pattern
    /// </remarks>
    /// <typeparam name="T">Type of entity for this repository </typeparam>
    public interface IRepository<T> : IDisposable
        where T : class
    {
        
        /// <summary>
        /// Add item into repository
        /// </summary>
        /// <param name="item">Item to add to repository</param>
        T Add(T item);
        T AddValue(T item);
        /// <summary>
        /// Delete item 
        /// </summary>
        /// <param name="item">Item to delete</param>
        void Remove(T item);

        /// <summary>
        /// Set item as modified
        /// </summary>
        /// <param name="item">Item to modify</param>
        //void Modify(T item);

        /// <summary>
        ///Track entity into this repository, really in UnitOfWork. 
        /// </summary>
        /// <param name="item">Item to attach</param>
        void TrackItem(T item);

        /// <summary>
        /// Sets modified entity into the repository. 
        /// When calling Commit() method in UnitOfWork 
        /// these changes will be saved into the storage
        /// </summary>
        /// <param name="persisted">The persisted item</param>
        /// <param name="current">The current item</param>
        //void Merge(T persisted, T current);

        /// <summary>
        /// Get element by entity key
        /// </summary>
        /// <param name="id">Entity key value</param>
        /// <returns></returns>
        T Get(long id);
        T Get(string id);
        T GetSingle(Expression<Func<T, bool>> filter);

        /// <summary>
        /// Get all elements of type T in repository
        /// </summary>
        /// <returns>List of selected elements</returns>
        IEnumerable<T> GetAll();
        
        /// <summary>
        /// Get all elements of type T in repository
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageCount">Number of elements in each page</param>
        /// <param name="orderByExpression">Order by expression for this query</param>
        /// <param name="ascending">Specify if order is ascending</param>
        /// <returns>List of selected elements</returns>
        IEnumerable<T> GetPaged<Property>(int pageIndex, int pageCount, Expression<Func<T, Property>> orderByExpression, bool ascending);

        /// <summary>
        /// Get  elements of type T in repository
        /// </summary>
        /// <param name="filter">Filter that each element do match</param>
        /// <returns>List of selected elements</returns>
        IEnumerable<T> GetFiltered<KProperty>(Expression<Func<T, bool>> filter, Expression<Func<T, KProperty>> orderByExpression, bool ascending);
        IEnumerable<T> GetFiltered(Expression<Func<T, bool>> filter);
       

        //T GetSingle(object filter);
    }
}
