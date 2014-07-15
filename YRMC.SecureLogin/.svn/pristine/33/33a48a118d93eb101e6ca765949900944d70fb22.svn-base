using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Code.Utilities.Extensions;

namespace YRMC.SecureLogin.Business.Edits
{
    [Serializable]
    public class UserRole : Common.BusinessLinkBase<UserRole, Data.SecurePasswordEntities, Data.UserRole>
    {
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

            Csla.Rules.BusinessRules.AddRule(typeof(UserRole), new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.GetObject, roles));
        }

        #endregion

        #region [ Factory Methods ]

        public static UserRole GetByPIDRoleID(Guid pid, Guid roleid)
        {
            return DataPortal.Fetch<UserRole>(new UserRoleCriteria { PID = pid, RoleID = roleid });
        }
        #endregion

        #region [ Data Access ]

        [Serializable]
        protected class UserRoleCriteria : CriteriaBase<UserRoleCriteria>
        {
            public Guid PID { get; set; }
            public Guid RoleID { get; set; }
        }

        protected void DataPortal_Fetch(UserRoleCriteria criteria)
        {
            using (Data.SecurePasswordEntities entities = new Data.SecurePasswordEntities())
            {
                var entity =
                    (from user in entities.UserRoles
                     where user.PID == criteria.PID && user.ID == criteria.RoleID
                     select user).FirstOrDefault();

                LoadProperties(entity);
            }
        }

        #endregion
    }
}
