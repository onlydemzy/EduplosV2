using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KS.Common
{
    /// <summary>
    /// Base class for entities
    /// </summary>
    public abstract class EntityBase
    {
        #region Members

        int? _requestedHashCode;

        #endregion

        #region Properties

        #endregion

        #region Public Methods


        #endregion

        #region Overrides Methods


        public static bool operator ==(EntityBase left, EntityBase right)
        {
            if (Object.Equals(left, null))
                return (Object.Equals(right, null)) ? true : false;
            else
                return left.Equals(right);
        }

        public static bool operator !=(EntityBase left, EntityBase right)
        {
            return !(left == right);
        }

        #endregion
    }
}
 

