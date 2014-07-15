using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Code.Utilities.Extensions;

namespace YRMC.SecureLogin.Business.Lists
{
    [Serializable]
    public class Logs : Csla.ReadOnlyListBase<Logs, Log>
    {
        #region [ Factory Methods ]

        public static Logs GetByEntryID(Guid PasswordEntryID)
        {
            return DataPortal.Fetch<Logs>(PasswordEntryID);
        }

        #endregion

        #region [ Data Access ]
        protected virtual void DataPortal_Fetch(Guid PasswordEntryID)
        {
            Data.Log[] results = null;

            using (Data.SecurePasswordEntities entities = new Data.SecurePasswordEntities())
            {
                results =
                    (from l in entities.Logs
                     where l.LoginEntryID == PasswordEntryID
                     select l).ToArray();             
            }

            RaiseListChangedEvents = false;

            foreach (var entity in results)
                Add(new Log(entity));

            RaiseListChangedEvents = true;
        }
        #endregion
    }
}
