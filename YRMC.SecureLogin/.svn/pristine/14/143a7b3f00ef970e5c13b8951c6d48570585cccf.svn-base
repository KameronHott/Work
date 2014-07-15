using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;

namespace YRMC.SecureLogin.Business.Searches
{
    [Serializable]
    public class Log : Common.ReadOnlyBase<Log, Data.SecurePasswordEntities, Data.Log>
    {
        #region [ Constructors ]

        private Log()
            : base()
        {
        }

        internal Log(object entity)
            : base(entity)
        {
        }

        #endregion

        #region [ Properties ]

        public static PropertyInfo<string> ActionDoneProperty = RegisterProperty<string>(c => c.ActionDone);
        public string ActionDone
        {
            get { return GetProperty(ActionDoneProperty); }
        }

        public static PropertyInfo<string> LoginNameProperty = RegisterProperty<string>(c => c.LoginName);
        public string LoginName
        {
            get { return GetProperty(LoginNameProperty); }
        }

        public static PropertyInfo<string> PasswordProperty = RegisterProperty<string>(c => c.Password);
        public string Password
        {
            get { return GetProperty(PasswordProperty); }
        }

        public static PropertyInfo<string> DescriptionProperty = RegisterProperty<string>(c => c.Description);
        public string Description
        {
            get { return GetProperty(DescriptionProperty); }
        }

        public static PropertyInfo<Guid> LoginIDProperty = RegisterProperty<Guid>(c => c.LoginID);
        public Guid LoginID
        {
            get { return GetProperty(LoginIDProperty); }
        }

        public static PropertyInfo<DateTime> ModifiedDateProperty = RegisterProperty<DateTime>(c => c.ModifiedDate);
        public DateTime ModifiedDate
        {
            get { return GetProperty(ModifiedDateProperty); }
        }

        public static PropertyInfo<string> CategoryProperty = RegisterProperty<string>(c => c.Category);
        public string Category
        {
            get { return GetProperty(CategoryProperty); }
        }

        public static PropertyInfo<string> RoleProperty = RegisterProperty<string>(c => c.Role);
        public string Role
        {
            get { return GetProperty(RoleProperty); }
        }

        public static PropertyInfo<string> MadeByProperty = RegisterProperty<string>(c => c.MadeBy);
        public string MadeBy
        {
            get { return GetProperty(MadeByProperty); }
        }
        #endregion

        #region [ Events ]

        protected override void OnLoadProperties(dynamic entity)
        {
            LoadProperty(LoginIDProperty, entity.LoginID);
            LoadProperty(ModifiedDateProperty, entity.ModifiedDate);
            LoadProperty(ActionDoneProperty, entity.ActionDone);
            LoadProperty(LoginNameProperty, entity.LoginName);
            LoadProperty(PasswordProperty, entity.Password);
            LoadProperty(DescriptionProperty, entity.Description);
            LoadProperty(CategoryProperty, entity.Category);
            LoadProperty(RoleProperty, entity.Role);
            LoadProperty(MadeByProperty, entity.MadeBy);
        }

        #endregion
    }
}
