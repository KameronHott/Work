using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;

namespace YRMC.SecureLogin.Business.Security
{
    [Serializable]
    public class SecurityIdentity : Csla.ReadOnlyBase<SecurityIdentity>, System.Security.Principal.IIdentity
    {
        #region [ Properties ]

        public string AuthenticationType
        {
            get { return "Csla"; }
        }

        public static PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);
        public string Name
        {
            get { return GetProperty(NameProperty); }
        }

        public static PropertyInfo<Boolean> IsAuthenticatedProperty = RegisterProperty<Boolean>(c => c.IsAuthenticated);
        public Boolean IsAuthenticated
        {
            get { return GetProperty(IsAuthenticatedProperty); }
        }

        public static PropertyInfo<Boolean> IsAdminProperty = RegisterProperty<Boolean>(c => c.IsAdmin);
        public Boolean IsAdmin
        {
            get { return GetProperty(IsAdminProperty); }
        }

        public static PropertyInfo<Guid> IDProperty = RegisterProperty<Guid>(c => c.ID);
        public Guid ID
        {
            get { return GetProperty(IDProperty); }
        }

        public static PropertyInfo<string> SIDProperty = RegisterProperty<string>(c => c.SID);
        public string SID
        {
            get { return GetProperty(SIDProperty); }
        }

        public static PropertyInfo<Dictionary<Guid, string>> RolesProperty = RegisterProperty<Dictionary<Guid, string>>(c => c.Roles);
        public Dictionary<Guid, string> Roles
        {
            get { return GetProperty(RolesProperty); }
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
        }

        #endregion

        #region [ Factory Methods ]

        public static SecurityIdentity GetIdentity(string sid)
        {
            return DataPortal.Fetch<SecurityIdentity>(sid);
        }

        #endregion

        #region [ Data Access ]

        protected void DataPortal_Fetch(string sid)
        {
            using (Data.SecurePasswordEntities entities = new Data.SecurePasswordEntities())
            {
                var user =
                    (from u in entities.Users
                     where u.SID == sid
                     select u).FirstOrDefault();

                if (user != null)
                {
                    LoadProperty(IDProperty, user.ID);
                    LoadProperty(SIDProperty, user.SID);
                    LoadProperty(NameProperty, new System.Security.Principal.SecurityIdentifier(user.SID).Translate(typeof(System.Security.Principal.NTAccount)).ToString());
                    LoadProperty(IsAdminProperty,
                        (from ur in entities.UserRoles
                         join r in entities.Roles
                         on ur.ID equals r.ID
                         where ur.PID == user.ID &&
                               r.Name == "administrators"
                         select ur).Count() > 0);

                    IQueryable<Data.Role> roles;

                    if (!ReadProperty(IsAdminProperty))
                        roles =
                            (from ur in entities.UserRoles
                             join r in entities.Roles
                             on ur.ID equals r.ID
                             where ur.PID == user.ID
                             select r);
                    else
                        roles =
                             (from r in entities.Roles
                              select r);

                    LoadProperty(RolesProperty, new Dictionary<Guid, string>());

                    foreach (Data.Role role in roles)
                        ReadProperty(RolesProperty).Add(role.ID, role.Name);

                    LoadProperty(IsAuthenticatedProperty, user.Active && roles.Count() > 0);
                }
                else
                {
                    // User could not be found.
                    LoadProperty(IsAuthenticatedProperty, false);
                }
            }
        }

        #endregion
    }
}
