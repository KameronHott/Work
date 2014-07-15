﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Code.Utilities.Extensions;

namespace YRMC.SecureLogin.Business.Edits
{
    [Serializable]
    public class User : Common.BusinessBase<User, Data.SecurePasswordEntities, Data.User>
    {
        #region [ Properties ]

        public static PropertyInfo<string> SIDProperty = RegisterProperty<string>(c => c.SID);
        public string SID
        {
            get { return GetProperty(SIDProperty); }
            set { SetProperty(SIDProperty, value); }
        }

        public static PropertyInfo<Boolean> ActiveProperty = RegisterProperty<Boolean>(c => c.Active);
        public Boolean Active
        {
            get { return GetProperty(ActiveProperty); }
            set { SetProperty(ActiveProperty, value); }
        }

        #endregion

        #region [ Business Rules ]

        protected override void AddBusinessRules()
        {
            base.AddBusinessRules();

            BusinessRules.AddRule(new Csla.Rules.CommonRules.Required(SIDProperty));
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
            return Csla.DataPortal.Fetch<User>(id);
        }

        public User GetBySID(string sid)
        {
            return Csla.DataPortal.Fetch<User>(sid);
        }

        #endregion

        #region [ Events ]

        protected override void OnSaveProperties(Data.User entity)
        {
            base.OnSaveProperties(entity);

            entity.SID = ReadProperty(User.SIDProperty);
            entity.Active = ReadProperty(User.ActiveProperty);
        }

        protected override void OnLoadProperties(Data.User entity)
        {
            base.OnLoadProperties(entity);

            LoadProperty(User.SIDProperty, entity.SID);
            LoadProperty(User.ActiveProperty, entity.Active);
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

                MarkOld();
            }
        }

        protected void DataPortal_Fetch(string sid)
        {
            using (Data.SecurePasswordEntities entities = new Data.SecurePasswordEntities())
            {
                var entity =
                    (from user in entities.Users
                     where user.SID == sid
                     select user).First();

                LoadProperties(entity);

                MarkOld();
            }
        }

        #endregion
    }
}