using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;

namespace YRMC.SecureLogin.Business.Lists
{
    [Serializable]
    public class Users : Csla.ReadOnlyListBase<Users, User>
    {
        #region [ Authorization Rules ]

        private static void AddObjectAuthorizationRules()
        {
            string[] roles = new string[] { "Administrators" };

            Csla.Rules.BusinessRules.AddRule(typeof(Users), new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.GetObject, roles));
        }

        #endregion

        #region [ Factory Methods ]

        public static Users GetAll()
        {
            return DataPortal.Fetch<Users>();
        }

        #endregion

        #region [ Data Access ]
        protected virtual void DataPortal_Fetch()
        {
            Data.User[] results = null;

            using (Data.SecurePasswordEntities entities = new Data.SecurePasswordEntities())
            {
                results =
                    (from user in entities.Users
                     select user).ToArray();
            }

            RaiseListChangedEvents = false;

            foreach (var entity in results)
                Add(new User(entity));

            RaiseListChangedEvents = true;
        }

        #endregion
    }
}
