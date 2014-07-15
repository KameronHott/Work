using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;

namespace YRMC.SecureLogin.Business.Lists
{
    [Serializable]
    public class Categories : Csla.ReadOnlyListBase<Categories, Category>
    {
        #region [ Factory Methods ]

        public static Categories GetAll()
        {
            return DataPortal.Fetch<Categories>();
        }

        #endregion

        #region [ Data Access ]
        protected virtual void DataPortal_Fetch()
        {
            Data.Category[] results = null;

            using (Data.SecurePasswordEntities entities = new Data.SecurePasswordEntities())
            {
                results =
                    (from l in entities.Categories
                     orderby l.Name
                     select l).ToArray();
            }

            RaiseListChangedEvents = false;

            foreach (var entity in results)
                Add(new Category(entity));

            RaiseListChangedEvents = true;
        }

        #endregion
    }
}
