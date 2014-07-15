using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Serialization;
using Code.Utilities.Extensions;

namespace YRMC.SecureLogin.Business.Lists
{
    [Serializable]
    public class Role : Common.ReadOnlyBase<Role, Data.SecurePasswordEntities, Data.Role>
    {
        #region [ Constructors ]

        private Role()
            : base()
        {
        }

        internal Role(Data.Role entity)
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

        public static PropertyInfo<Guid> EntryIdProperty = RegisterProperty<Guid>(c => c.EntryId);
        public Guid EntryId
        {
            get { return GetProperty(EntryIdProperty); }
        }

        public static PropertyInfo<Guid> UserIDProperty = RegisterProperty<Guid>(c => c.UserID);
        public Guid UserID
        {
            get { return GetProperty(UserIDProperty); }
        }

        public static PropertyInfo<Boolean> IsAdminProperty = RegisterProperty<Boolean>(c => c.IsAdmin);
        public Boolean IsAdmin
        {
            get { return GetProperty(IsAdminProperty); }
        }

        public static PropertyInfo<Boolean> ActiveProperty = RegisterProperty<Boolean>(c => c.Active);
        public Boolean Active
        {
            get { return GetProperty(ActiveProperty); }
        }

        #endregion

        #region [ Business Rules ]

        protected override void AddBusinessRules()
        {
            base.AddBusinessRules();
        }

        #endregion

        #region [ Authorization Rules ]

        private static void AddObjectAuthorizationRules()
        {
            string[] roles = new string[] { "Administrators" };

            Csla.Rules.BusinessRules.AddRule(typeof(Role), new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.GetObject, roles));
        }

        #endregion

        #region [ Factory Methods ]

        public static Role GetByID(Guid id)
        {
            return DataPortal.Fetch<Role>(id);
        }

        public static Role GetByID(Guid id, Boolean active)
        {
            return DataPortal.Fetch<Role>(new RoleCriteria { EntryID = id, Active = active });
        }

        #endregion

        #region [ Events ]

        protected override void OnLoadProperties(Data.Role entity)
        {
            base.OnLoadProperties(entity);

            LoadProperty(NameProperty, entity.Name);
        }

        #endregion

        #region [ Data Access ]

        [Serializable]
        public class RoleCriteria : CriteriaBase<RoleCriteria>
        {
            public Guid EntryID { get; set; }
            public Boolean Active { get; set; }
        }

        protected void DataPortal_Fetch(Guid id)
        {
            using (Data.SecurePasswordEntities entities = new Data.SecurePasswordEntities())
            {
                var entity =
                    (from i in entities.Roles
                     join l in entities.Logins
                     on i.ID equals l.RoleID
                     where l.ID == id
                     select i).First();

                LoadProperties(entity);
            }
        }

        protected void DataPortal_Fetch(RoleCriteria criteria)
        {
            using (Data.SecurePasswordEntities entities = new Data.SecurePasswordEntities())
            {
                var entity =
                    (from i in entities.Logins
                     join r in entities.Roles
                     on i.RoleID equals r.ID
                     where i.EntryID == criteria.EntryID && i.Active == criteria.Active
                     select r).First();

                LoadProperties(entity);
            }
        }
        #endregion
    }
}
