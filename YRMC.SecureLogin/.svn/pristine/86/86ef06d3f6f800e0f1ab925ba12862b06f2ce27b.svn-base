using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;

namespace YRMC.SecureLogin.Business.Lists
{
    [Serializable]
    public class Log : Common.ReadOnlyBase<Log, Data.SecurePasswordEntities, Data.Log>
    {
        #region [ Constructors ]

        private Log()
            : base()
        {
        }

        internal Log(Data.Log entity)
            : base(entity)
        {
        }

        #endregion

        #region [ Properties ]

        public static PropertyInfo<string> ActionProperty = RegisterProperty<string>(c => c.Action);
        public string Action
        {
            get { return GetProperty(ActionProperty); }
        }

        public static PropertyInfo<Guid> UserIDProperty = RegisterProperty<Guid>(c => c.UserID);
        public Guid UserID
        {
            get { return GetProperty(UserIDProperty); }
        }

        public static PropertyInfo<Guid> RoleIDProperty = RegisterProperty<Guid>(c => c.RoleID);
        public Guid RoleID
        {
            get { return GetProperty(RoleIDProperty); }
        }

        public static PropertyInfo<Guid> LoginIDProperty = RegisterProperty<Guid>(c => c.LoginID);
        public Guid LoginID
        {
            get { return GetProperty(LoginIDProperty); }
        }

        public static PropertyInfo<Guid> LoginEntryIDProperty = RegisterProperty<Guid>(c => c.LoginEntryID);
        public Guid LoginEntryID
        {
            get { return GetProperty(LoginEntryIDProperty); }
        }

        public static PropertyInfo<DateTime> ModifiedDateProperty = RegisterProperty<DateTime>(c => c.ModifiedDate);
        public DateTime ModifiedDate
        {
            get { return GetProperty(ModifiedDateProperty); }
        }

        #endregion

        #region [ Factory Methods ]

        public static Log GetByID(Guid id)
        {
            return DataPortal.Fetch<Log>(id);
        }

        #endregion

        #region [ Events ]

        protected override void OnLoadProperties(Data.Log entity)
        {
            base.OnLoadProperties(entity);

            LoadProperty(ActionProperty, entity.Action);
            LoadProperty(UserIDProperty, entity.UserID);
            LoadProperty(RoleIDProperty, entity.RoleID);
            LoadProperty(LoginIDProperty, entity.LoginID);
            LoadProperty(LoginEntryIDProperty, entity.LoginEntryID);
            LoadProperty(ModifiedDateProperty, entity.ModifiedDate);
        }

        #endregion

        #region [ Data Access ]

        protected void DataPortal_Fetch(Guid id)
        {
            using (Data.SecurePasswordEntities entities = new Data.SecurePasswordEntities())
            {
                var entity =
                    (from i in entities.Logs
                     where i.ID == id
                     select i).First();

                LoadProperties(entity);
            }
        }

        #endregion
    }
}
