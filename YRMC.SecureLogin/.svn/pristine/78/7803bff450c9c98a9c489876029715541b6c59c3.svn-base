using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Code.Utilities.Extensions;

namespace YRMC.SecureLogin.Business.Edits
{
    [Serializable]
    public class Login : Common.BusinessBase<Login, Data.SecurePasswordEntities, Data.Login>
    {
        #region [ Properties ]

        public static PropertyInfo<Guid> EntryIDProperty = RegisterProperty<Guid>(c => c.EntryID);
        public Guid EntryID
        {
            get { return GetProperty(EntryIDProperty); }
            set { SetProperty(EntryIDProperty, value); }
        }

        public static PropertyInfo<Guid> CategoryIDProperty = RegisterProperty<Guid>(c => c.CategoryID);
        public Guid CategoryID
        {
            get { return GetProperty(CategoryIDProperty); }
            set { SetProperty(CategoryIDProperty, value); }
        }

        public static PropertyInfo<Guid> RoleIDProperty = RegisterProperty<Guid>(c => c.RoleID);
        public Guid RoleID
        {
            get { return GetProperty(RoleIDProperty); }
            set { SetProperty(RoleIDProperty, value); }
        }

        public static PropertyInfo<string> DescriptionProperty = RegisterProperty<string>(c => c.Description);
        public string Description
        {
            get { return GetProperty(DescriptionProperty); }
            set { SetProperty(DescriptionProperty, value); }
        }

        public static PropertyInfo<string> UsernameProperty = RegisterProperty<string>(c => c.Username);
        public string Username
        {
            get { return GetProperty(UsernameProperty); }
            set { SetProperty(UsernameProperty, value); }
        }

        public static PropertyInfo<string> PasswordProperty = RegisterProperty<string>(c => c.Password);
        public string Password
        {
            get { return GetProperty(PasswordProperty).Decrypt(); }
            set { SetProperty(PasswordProperty, value.Encrypt()); }
        }

        public static PropertyInfo<Boolean> ActiveProperty = RegisterProperty<Boolean>(c => c.Active);
        public Boolean Active
        {
            get { return GetProperty(ActiveProperty); }
            set { SetProperty(ActiveProperty, value); }
        }

        public static PropertyInfo<DateTime> ModifiedDateProperty = RegisterProperty<DateTime>(c => c.ModifiedDate);
        public DateTime ModifiedDate
        {
            get { return GetProperty(ModifiedDateProperty); }
            set { SetProperty(ModifiedDateProperty, value); }
        }

        #endregion

        #region [ Business Rules ]

        protected override void AddBusinessRules()
        {
            base.AddBusinessRules();

            BusinessRules.AddRule(new Csla.Rules.CommonRules.Required(CategoryIDProperty));
            BusinessRules.AddRule(new Csla.Rules.CommonRules.Required(RoleIDProperty));
            BusinessRules.AddRule(new Csla.Rules.CommonRules.Required(UsernameProperty));
            BusinessRules.AddRule(new Csla.Rules.CommonRules.Required(PasswordProperty));

            BusinessRules.AddRule(new Csla.Rules.CommonRules.Lambda(UsernameProperty, (context) =>
            {
                var target = (Login)context.Target;
                Business.Security.SecurityIdentity user = (Business.Security.SecurityIdentity)Csla.ApplicationContext.User.Identity;

                if (!user.IsAdmin && !user.Roles.ContainsKey(target.RoleID))
                {
                    context.AddErrorResult("No matching Role found.");
                }
            }));

            BusinessRules.AddRule(new Csla.Rules.CommonRules.Lambda(UsernameProperty, (context) =>
            {
                var target = (Login)context.Target;
                ExistsByCategoryRoleUsernameCommand command =
                    DataPortal.Execute<ExistsByCategoryRoleUsernameCommand>(new ExistsByCategoryRoleUsernameCommand(target.CategoryID, target.RoleID, target.Username));

                if (command.Exists && command.EntryID != target.EntryID)
                    context.AddErrorResult("A Login with the Category, Role and Username specified already exists.");
            }));
        }

        #endregion

        #region [ Authorization Rules ]

        private static void AddObjectAuthorizationRules()
        {
        }

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

        #endregion

        #region [ Events ]

        protected override void OnCreate()
        {
            base.OnCreate();

            LoadProperty(ModifiedDateProperty, DateTime.UtcNow);
        }

        protected override void OnSaveProperties(Data.Login entity)
        {
            entity.CategoryID = ReadProperty(Login.CategoryIDProperty);
            entity.EntryID = ReadProperty(Login.EntryIDProperty);
            entity.Description = ReadProperty(Login.DescriptionProperty);
            entity.RoleID = ReadProperty(Login.RoleIDProperty);
            entity.Username = ReadProperty(Login.UsernameProperty);
            entity.Password = ReadProperty(Login.PasswordProperty);
            entity.Active = ReadProperty(Login.ActiveProperty);
            entity.ModifiedDate = ReadProperty(Login.ModifiedDateProperty);
        }

        protected override void OnLoadProperties(Data.Login entity)
        {
            base.OnLoadProperties(entity);

            LoadProperty(Login.CategoryIDProperty, entity.CategoryID);
            LoadProperty(Login.EntryIDProperty, entity.EntryID);
            LoadProperty(Login.DescriptionProperty, entity.Description);
            LoadProperty(Login.RoleIDProperty, entity.RoleID);
            LoadProperty(Login.UsernameProperty, entity.Username);
            LoadProperty(Login.PasswordProperty, entity.Password);
            LoadProperty(Login.ActiveProperty, entity.Active);
            LoadProperty(Login.ModifiedDateProperty, entity.ModifiedDate);
        }

        #endregion

        #region [ Data Access ]

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

        [Serializable]
        private class ExistsByCategoryRoleUsernameCommand : Csla.CommandBase<ExistsByCategoryRoleUsernameCommand>
        {
            #region [ Constructors ]

            public ExistsByCategoryRoleUsernameCommand(Guid categoryID, Guid roleID, string username)
            {
                CategoryID = categoryID;
                RoleID = roleID;
                Username = username;
            }

            #endregion

            #region [ Properties ]

            public Guid CategoryID { get; set; }
            public Guid RoleID { get; set; }
            public string Username { get; set; }
            public Guid EntryID { get; set; }
            public bool Exists { get; set; }

            #endregion

            #region [ Data Access ]

            protected override void DataPortal_Execute()
            {
                using (Data.SecurePasswordEntities entities = new Data.SecurePasswordEntities())
                {
                    EntryID =
                        (from item in entities.Logins
                         where item.CategoryID == CategoryID &&
                            item.RoleID == RoleID &&
                            item.Username == Username &&
                            item.Active == true
                         select item.EntryID).FirstOrDefault();

                    Exists = EntryID != default(Guid);
                }
            }

            #endregion
        }

        #endregion
    }
}
