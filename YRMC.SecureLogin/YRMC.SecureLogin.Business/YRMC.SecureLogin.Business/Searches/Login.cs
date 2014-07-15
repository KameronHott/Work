﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;

namespace YRMC.SecureLogin.Business.Searches
{
    [Serializable]
    public class Login : Common.ReadOnlyBase<Login, Data.SecurePasswordEntities, Data.Login>
    {
        #region [ Constructors ]

        private Login()
            : base()
        {
        }

        internal Login(object entity)
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

        public static PropertyInfo<string> CategoryNameProperty = RegisterProperty<string>(c => c.CategoryName);
        public string CategoryName
        {
            get { return GetProperty(CategoryNameProperty); }
        }

        public static PropertyInfo<string> RoleNameProperty = RegisterProperty<string>(c => c.RoleName);
        public string RoleName
        {
            get { return GetProperty(RoleNameProperty); }
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

        #endregion

        #region [ Events ]

        protected override void OnLoadProperties(dynamic entity)
        {
            LoadProperty(IDProperty, entity.ID);
            LoadProperty(EntryIDProperty, entity.EntryID);
            LoadProperty(CategoryIDProperty, entity.CategoryID);
            LoadProperty(CategoryNameProperty, entity.CategoryName);
            LoadProperty(RoleNameProperty, entity.RoleName);
            LoadProperty(DescriptionProperty, entity.Description);
            LoadProperty(UsernameProperty, entity.Username);
            LoadProperty(ActiveProperty, entity.Active);
            LoadProperty(ModifiedDateProperty, entity.ModifiedDate);
        }

        #endregion
    }
}