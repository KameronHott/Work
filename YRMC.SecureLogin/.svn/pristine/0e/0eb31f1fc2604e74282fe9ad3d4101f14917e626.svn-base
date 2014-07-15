using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Code.Utilities.Extensions;

namespace YRMC.SecureLogin.Business.Lists
{
    [Serializable]
    public class Login : Common.ReadOnlyBase<Login, Data.SecurePasswordEntities, Data.Login>
    {
        #region [ Constructors ]

        private Login()
            : base()
        {
        }

        internal Login(Data.Login entity)
            : base(entity)
        {
        }

        #endregion

        #region [ Properties ]

        public static PropertyInfo<Guid> EntryIDProperty = RegisterProperty<Guid>(c => c.EntryID);
        public Guid EntryID
        {
            get { return GetProperty(EntryIDProperty); }
        }

        public static PropertyInfo<Guid> CategoryIDProperty = RegisterProperty<Guid>(c => c.CategoryID);
        public Guid CategoryID
        {
            get { return GetProperty(CategoryIDProperty); }
        }

        public static PropertyInfo<Category> CategoryProperty = RegisterProperty<Category>(c => c.Category);
        public Category Category
        {
            get
            {
                if (!FieldManager.FieldExists(CategoryProperty))
                    LoadProperty(CategoryProperty, Category.GetByID(CategoryID));

                return GetProperty(CategoryProperty);
            }
        }

        public static PropertyInfo<Guid> RoleIDProperty = RegisterProperty<Guid>(c => c.RoleID);
        public Guid RoleID
        {
            get { return GetProperty(RoleIDProperty); }
        }

        public static PropertyInfo<Role> RoleProperty = RegisterProperty<Role>(c => c.Role);
        public Role Role
        {
            get
            {
                if (!FieldManager.FieldExists(RoleProperty))
                    LoadProperty(RoleProperty, Role.GetByID(RoleID));

                return GetProperty(RoleProperty);
            }
        }

        public static PropertyInfo<string> DescriptionProperty = RegisterProperty<string>(c => c.Description);
        public string Description
        {
            get { return GetProperty(DescriptionProperty); }
        }

        public static PropertyInfo<string> UsernameProperty = RegisterProperty<string>(c => c.Username);
        public string Username
        {
            get { return GetProperty(UsernameProperty); }
        }

        public static PropertyInfo<string> PasswordProperty = RegisterProperty<string>(c => c.Password);
        public string Password
        {
            get { return GetProperty(PasswordProperty).Decrypt(); }
        }

        public static PropertyInfo<bool> ActiveProperty = RegisterProperty<bool>(c => c.Active);
        public bool Active
        {
            get { return GetProperty(ActiveProperty); }
        }

        public static PropertyInfo<DateTime> ModifiedDateProperty = RegisterProperty<DateTime>(c => c.ModifiedDate);
        public DateTime ModifiedDate
        {
            get { return GetProperty(ModifiedDateProperty); }
        }

        public bool IsAdmin { get; set; }

        #endregion

        #region [ Factory Methods ]

        public static Login GetByID(Guid id)
        {
            try
            {
                return DataPortal.Fetch<Login>(id);
            }
            catch (Csla.DataPortalException ex)
            {
                throw new Csla.Rules.ValidationException(ex.Message);
            }
        }

        public static Login GetByEntryID(Guid id)
        {
            try
            {
                return DataPortal.Fetch<Login>(new EntryCriteria { EntryID = id });
            }
            catch (Csla.DataPortalException ex)
            {
                throw new Csla.Rules.ValidationException(ex.Message);
            }
        }

        #endregion

        #region [ Events ]

        protected override void OnLoadProperties(Data.Login entity)
        {
            base.OnLoadProperties(entity);

            try
            {
                LoadProperty(EntryIDProperty, entity.EntryID);
                LoadProperty(CategoryIDProperty, entity.CategoryID);
                LoadProperty(RoleIDProperty, entity.RoleID);
                LoadProperty(DescriptionProperty, entity.Description);
                LoadProperty(UsernameProperty, entity.Username);
                LoadProperty(PasswordProperty, entity.Password);
                LoadProperty(ActiveProperty, entity.Active);
                LoadProperty(ModifiedDateProperty, entity.ModifiedDate);
            }
            catch { }
        }

        #endregion

        #region [ Data Access ]

        [Serializable]
        protected class EntryCriteria : CriteriaBase<EntryCriteria>
        {
            public Guid EntryID { get; set; }
        }

        protected void DataPortal_Fetch(Guid id)
        {
            using (Data.SecurePasswordEntities entities = new Data.SecurePasswordEntities())
            {
                Business.Security.SecurityIdentity user = (Business.Security.SecurityIdentity)Csla.ApplicationContext.User.Identity;
                Data.Login entity;

                if ((from l in entities.Logins
                     where l.ID == id
                     select l).Count() == 1)
                {
                    if (user.IsAdmin)
                        entity =
                            (from l in entities.Logins
                             where l.ID == id
                             select l).FirstOrDefault();
                    else
                        entity =
                            (from l in entities.Logins
                             join ur in entities.UserRoles
                             on l.RoleID equals ur.ID
                             where ur.PID == user.ID && l.ID == id
                             select l).FirstOrDefault();

                    if (entity != null)
                        LoadProperties(entity);
                    else
                        throw new Csla.DataPortalException("User not authorized to Get object type Login", typeof(Login));
                }
            }
        }

        protected void DataPortal_Fetch(EntryCriteria criteria)
        {
            using (Data.SecurePasswordEntities entities = new Data.SecurePasswordEntities())
            {
                Business.Security.SecurityIdentity user = (Business.Security.SecurityIdentity)Csla.ApplicationContext.User.Identity;
                Data.Login entity;

                if ((from l in entities.Logins
                        where l.EntryID == criteria.EntryID && l.Active == true
                        select l).Count() == 1)
                {
                    if (user.IsAdmin)
                        entity =
                            (from l in entities.Logins
                             where l.EntryID == criteria.EntryID && l.Active == true
                             select l).FirstOrDefault();
                    else
                        entity =
                            (from l in entities.Logins
                             join ur in entities.UserRoles
                             on l.RoleID equals ur.ID
                             where ur.PID == user.ID && l.EntryID == criteria.EntryID && l.Active == true
                             select l).FirstOrDefault();

                    if (entity != null)
                        LoadProperties(entity);
                    else
                        throw new Csla.DataPortalException("User not authorized to Get object type Login", typeof(Login));
                }
            }
        }

        #endregion
    }
}
