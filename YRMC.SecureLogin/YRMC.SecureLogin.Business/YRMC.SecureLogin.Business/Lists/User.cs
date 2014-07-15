﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;

namespace YRMC.SecureLogin.Business.Lists
{
    [Serializable]
    public class User : Common.ReadOnlyBase<User, Data.SecurePasswordEntities, Data.User>
    {
        #region [ Constructors ]

        private User()
            : base()
        {
        }

        internal User(Data.User entity)
            : base(entity)
        {
        }

        #endregion

        #region [ Properties ]

        public static PropertyInfo<string> SIDProperty = RegisterProperty<string>(c => c.SID);
        public string SID
        {
            get { return GetProperty(SIDProperty); }
        }

        public static PropertyInfo<bool> ActiveProperty = RegisterProperty<bool>(c => c.Active);
        public bool Active
        {
            get { return GetProperty(ActiveProperty); }
        }

        public static PropertyInfo<string> UsernameProperty = RegisterProperty<string>(c => c.Username);
        public string Username
        {
            get
            {
                if (!FieldManager.FieldExists(UsernameProperty))
                {
                    // Try to convert the SID to NT Account Username, might fail if the account was deleted.
                    string username;

                    try
                    {
                        username = new System.Security.Principal.SecurityIdentifier(ReadProperty(SIDProperty))
                            .Translate(typeof(System.Security.Principal.NTAccount)).ToString();
                    }
                    catch
                    {
                        username = ReadProperty(SIDProperty);
                    }

                    LoadProperty(UsernameProperty, username);
                }

                return GetProperty(UsernameProperty);
            }
        }

        public static PropertyInfo<Roles> RolesProperty = RegisterProperty<Roles>(c => c.Roles);
        public Roles Roles
        {
            get
            {
                if (!FieldManager.FieldExists(RolesProperty))
                    LoadProperty(RolesProperty, Roles.GetByUserID(ID));

                return GetProperty(RolesProperty);
            }
        }

        public static PropertyInfo<bool> IsAdminProperty = RegisterProperty<bool>(c => c.IsAdmin);
        public bool IsAdmin
        {
            get
            {
                if (!FieldManager.FieldExists(IsAdminProperty))
                    LoadProperty(IsAdminProperty, DataPortal.Execute<IsAdminCommand>(
                        new IsAdminCommand(GetProperty(IDProperty))).IsAdmin);

                return GetProperty(IsAdminProperty);
            }
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

            Csla.Rules.BusinessRules.AddRule(typeof(User), new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.GetObject, roles));
        }

        #endregion

        #region [ Factory Methods ]

        public static User GetByID(Guid id)
        {
            return DataPortal.Fetch<User>(id);
        }

        public static bool ExistsBySID(string sid)
        {
            return DataPortal.Execute<ExistsBySIDCommand>(
                new ExistsBySIDCommand(sid)).Exists;
        }

        #endregion

        #region [ Events ]

        protected override void OnLoadProperties(Data.User entity)
        {
            base.OnLoadProperties(entity);

            LoadProperty(SIDProperty, entity.SID);
            LoadProperty(ActiveProperty, entity.Active);
        }

        #endregion

        #region [ Data Access ]

        protected void DataPortal_Fetch(Guid id)
        {
            using (Data.SecurePasswordEntities entities = new Data.SecurePasswordEntities())
            {
                var entity =
                    (from user in entities.Users
                     where user.ID == id
                     select user).First();

                LoadProperties(entity);
            }
        }

        [Serializable]
        private class IsAdminCommand : Csla.CommandBase<IsAdminCommand>
        {
            #region [ Constructors ]

            public IsAdminCommand(Guid userID)
            {
                UserID = userID;
            }

            #endregion

            #region [ Properties ]

            public Guid UserID { get; set; }
            public bool IsAdmin { get; set; }

            #endregion

            #region [ Data Access ]

            protected override void DataPortal_Execute()
            {
                using (Data.SecurePasswordEntities entities = new Data.SecurePasswordEntities())
                {
                    IsAdmin = 
                        (from ur in entities.UserRoles
                         join r in entities.Roles
                         on ur.ID equals r.ID
                         where ur.PID == UserID &&
                               r.Name == "administrators"
                         select ur).Count() > 0;
                }
            }

            #endregion
        }

        [Serializable]
        private class ExistsBySIDCommand : Csla.CommandBase<ExistsBySIDCommand>
        {
            #region [ Constructors ]

            public ExistsBySIDCommand(string sid)
            {
                SID = sid;
            }

            #endregion

            #region [ Properties ]

            public string SID { get; set; }
            public bool Exists { get; set; }

            #endregion

            #region [ Data Access ]

            protected override void DataPortal_Execute()
            {
                using (Data.SecurePasswordEntities entities = new Data.SecurePasswordEntities())
                {
                    Exists =
                        (from user in entities.Users
                         where user.SID == SID
                         select user).Count() > 0;
                }
            }

            #endregion
        }

        #endregion
    }
}