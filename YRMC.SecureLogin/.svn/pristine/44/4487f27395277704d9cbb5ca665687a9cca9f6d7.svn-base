using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;

namespace YRMC.SecureLogin.Business.Lists
{
    [Serializable]
    public class Category : Common.ReadOnlyBase<Category, Data.SecurePasswordEntities, Data.Category>
    {
        #region [ Constructors ]

        private Category()
            : base()
        {
        }

        internal Category(Data.Category entity)
            : base(entity)
        {
        }

        #endregion

        #region [ Properties ]

        public static PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);
        public string Name
        {
            get { return GetProperty(NameProperty); }
        }
        
        #endregion

        #region [ Factory Methods ]

        public static Category GetByID(Guid id)
        {
            return DataPortal.Fetch<Category>(id);
        }

        #endregion

        #region [ Events ]

        protected override void OnLoadProperties(Data.Category entity)
        {
            base.OnLoadProperties(entity);

            LoadProperty(NameProperty, entity.Name);
        }

        #endregion

        #region [ Data Access ]

        protected void DataPortal_Fetch(Guid id)
        {
            using (Data.SecurePasswordEntities entities = new Data.SecurePasswordEntities())
            {
                var entity =
                    (from i in entities.Categories
                     where i.ID == id
                     select i).First();

                LoadProperties(entity);
            }
        }

        #endregion
    }
}
