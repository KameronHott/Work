﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Code.Utilities.Extensions;

namespace YRMC.SecureLogin.Business.Edits
{
    [Serializable]
    public class Log : Common.BusinessBase<Log, Data.SecurePasswordEntities, Data.Log>
    {
        #region [ Properties ]

        public static PropertyInfo<string> ActionProperty = RegisterProperty<string>(c => c.Action);
        public string Action
        {
            get { return GetProperty(ActionProperty); }
            set { SetProperty(ActionProperty, value); }
        }

        public static PropertyInfo<Guid> UserIDProperty = RegisterProperty<Guid>(c => c.UserID);
        public Guid UserID
        {
            get { return GetProperty(UserIDProperty); }
            set { SetProperty(UserIDProperty, value); }
        }

        public static PropertyInfo<Guid> RoleIDProperty = RegisterProperty<Guid>(c => c.RoleID);
        public Guid RoleID
        {
            get { return GetProperty(RoleIDProperty); }
            set { SetProperty(RoleIDProperty, value); }
        }

        public static PropertyInfo<Guid> PasswordIDProperty = RegisterProperty<Guid>(c => c.PasswordID);
        public Guid PasswordID
        {
            get { return GetProperty(PasswordIDProperty); }
            set { SetProperty(PasswordIDProperty, value); }
        }

        public static PropertyInfo<Guid> PasswordEntryIDProperty = RegisterProperty<Guid>(c => c.PasswordEntryID);
        public Guid PasswordEntryID
        {
            get { return GetProperty(PasswordEntryIDProperty); }
            set { SetProperty(PasswordEntryIDProperty, value); }
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

            BusinessRules.AddRule(new Csla.Rules.CommonRules.Required(ActionProperty));
        }

        #endregion

        #region [ Factory Methods ]

        public static Log WriteEntry(string action, Guid roleID, Guid passwordID, Guid passwordEntryID)
        {
            Business.Security.SecurityIdentity user = (Business.Security.SecurityIdentity)Csla.ApplicationContext.User.Identity;

            Log log = Create();

            log.Action = action;
            log.UserID = user.ID;
            log.RoleID = roleID;
            log.PasswordID = passwordID;
            log.PasswordEntryID = passwordEntryID;
            log.ModifiedDate = DateTime.UtcNow;

            return log.Save();
        }

        #endregion

        #region [ Events ]

        protected override void OnSaveProperties(Data.Log entity)
        {
            entity.Action = ReadProperty(Log.ActionProperty);
            entity.UserID = ReadProperty(Log.UserIDProperty);
            entity.RoleID = ReadProperty(Log.RoleIDProperty);
            entity.LoginID = ReadProperty(Log.PasswordIDProperty);
            entity.LoginEntryID = ReadProperty(Log.PasswordEntryIDProperty);
            entity.ModifiedDate = ReadProperty(Log.ModifiedDateProperty);
        }

        protected override void OnLoadProperties(Data.Log entity)
        {
            base.OnLoadProperties(entity);

            LoadProperty(Log.ActionProperty, entity.Action);
            LoadProperty(Log.UserIDProperty, entity.UserID);
            LoadProperty(Log.RoleIDProperty, entity.RoleID);
            LoadProperty(Log.PasswordIDProperty, entity.LoginID);
            LoadProperty(Log.PasswordEntryIDProperty, entity.LoginEntryID);
            LoadProperty(Log.ModifiedDateProperty, entity.ModifiedDate);
        }

        #endregion
    }
}
