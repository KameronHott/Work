using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Code.Utilities.Extensions;

namespace YRMC.SecureLogin.Business.Lists
{
    [Serializable]
    public class Logins : Csla.ReadOnlyListBase<Logins, Login>
    {
    }
}
