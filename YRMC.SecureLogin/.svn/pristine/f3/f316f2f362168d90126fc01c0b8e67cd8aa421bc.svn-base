using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.DirectoryServices;
using System.Text.RegularExpressions;
using System.Security.Principal;

namespace YRMC.SecureLogin.Business.Security
{
    [Serializable]
    public class SecurityPrincipal : Csla.Security.CslaPrincipal
    {
        #region [ Constructors ]

        private SecurityPrincipal(IIdentity identity)
            : base(identity)
        {
        }

        #endregion

        #region [ Factory Methods ]

        public static void Login(string sid)
        {
            SecurityIdentity si = SecurityIdentity.GetIdentity(sid);

            if (si.IsAuthenticated)
                Csla.ApplicationContext.User = new SecurityPrincipal(si);
            else
                Csla.ApplicationContext.User = new Csla.Security.UnauthenticatedPrincipal();
        }

        #endregion

        #region [ Methods ]

        public bool IsInRole(Guid id)
        {
            return ((SecurityIdentity)Identity).Roles.ContainsKey(id);
        }

        public override bool IsInRole(string name)
        {
            return ((SecurityIdentity)Identity).Roles.ContainsValue(name);
        }

        #endregion
    }
}
