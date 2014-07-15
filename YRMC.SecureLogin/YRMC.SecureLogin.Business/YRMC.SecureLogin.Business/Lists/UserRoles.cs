using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;

namespace YRMC.SecureLogin.Business.Lists
{
    [Serializable]
    public class UserRoles : Csla.ReadOnlyListBase<UserRoles, UserRole>
    {
        #region [ Authorization Rules ]

        private static void AddObjectAuthorizationRules()
        {
            string[] roles = new string[] { "Administrators" };

            Csla.Rules.BusinessRules.AddRule(typeof(UserRoles), new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.GetObject, roles));
        }

        #endregion

        #region [ Factory Methods ]

        public static UserRoles GetByUserID(Guid userID)
        {
            return DataPortal.Fetch<UserRoles>(userID);
        }

        #endregion

        #region [ Data Access ]
        protected virtual void DataPortal_Fetch(Guid userID)
        {
            Data.UserRole[] results = null;

            using (Data.SecurePasswordEntities entities = new Data.SecurePasswordEntities())
            {
                results =
                    (from l in entities.UserRoles
                     where l.PID == userID
                     select l).ToArray();
            }

            RaiseListChangedEvents = false;

            foreach (var entity in results)
                Add(new UserRole(entity));

            RaiseListChangedEvents = true;
        }

        #endregion
    }
}
