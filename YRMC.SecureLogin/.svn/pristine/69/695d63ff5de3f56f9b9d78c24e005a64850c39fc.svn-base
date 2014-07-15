using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Code.Utilities.Extensions;

namespace YRMC.SecureLogin.Business.Searches
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
            Object[] results = null;

            using (Data.SecurePasswordEntities entities = new Data.SecurePasswordEntities())
            {
                results =
                    (from l in entities.Logs
                     join u in entities.Users
                     on l.UserID equals u.ID
                     join p in entities.Logins
                     on new { l.LoginEntryID, l.LoginID } equals new { LoginEntryID = p.EntryID, LoginID = p.ID }
                     join c in entities.Categories
                     on p.CategoryID equals c.ID
                     join r in entities.Roles
                     on p.RoleID equals r.ID
                     where l.LoginEntryID == PasswordEntryID
                     orderby l.ModifiedDate descending
                     select new
                     {
                         LoginID = l.LoginEntryID,
                         ModifiedDate = l.ModifiedDate,
                         ActionDone = l.Action,
                         LoginName = p.Username,
                         Password = p.Password,
                         Description = p.Description,
                         Category = c.Name,
                         Role = r.Name,
                         MadeBy = u.SID

                     }).ToArray();             
            }

            RaiseListChangedEvents = false;

            foreach (var entity in results)
                Add(new Log(entity));

            RaiseListChangedEvents = true;
        }
        #endregion
    }
}
