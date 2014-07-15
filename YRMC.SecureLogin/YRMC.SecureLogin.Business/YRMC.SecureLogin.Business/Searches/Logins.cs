using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Code.Utilities.Extensions;

namespace YRMC.SecureLogin.Business.Searches
{
    [Serializable]
    public class Logins : Csla.ReadOnlyListBase<Logins, Login>
    {       
        #region [ Factory Methods ]

        public static Logins GetBySearch(string searchBy, string searchText)
        {
            return DataPortal.Fetch<Logins>(new LoginCriteria
            {
                SearchBy = searchBy,
                SearchText = searchText
            });
        }
        
        #endregion

        #region [ Data Access ]

        [Serializable]
        public class LoginCriteria : CriteriaBase<LoginCriteria>
        {
            public string SearchBy { get; set; }
            public string SearchText { get; set; }
        }

        protected virtual void DataPortal_Fetch(LoginCriteria criteria)
        {
            dynamic results;
            Security.SecurityIdentity user = (Security.SecurityIdentity)Csla.ApplicationContext.User.Identity;
 
            using (Data.SecurePasswordEntities entities = new Data.SecurePasswordEntities())
            {
                var query =
                    (from l in entities.Logins
                     join c in entities.Categories
                     on l.CategoryID equals c.ID
                     join r in entities.Roles
                     on l.RoleID equals r.ID
                     where l.Active == true
                     select new
                     {
                         ID = l.ID,
                         EntryID = l.EntryID,
                         CategoryID = c.ID,
                         CategoryName = c.Name,
                         RoleName = r.Name,
                         Description = l.Description,
                         Username = l.Username,
                         Active = l.Active,
                         ModifiedDate = l.ModifiedDate
                     });

                if (!user.IsAdmin)
                {
                    query = 
                        (from l in entities.Logins
                        join c in entities.Categories
                        on l.CategoryID equals c.ID
                        join r in entities.Roles
                        on l.RoleID equals r.ID
                        join ur in entities.UserRoles
                        on r.ID equals ur.ID
                        where ur.PID == user.ID && l.Active == true
                        select new
                        { 
                            ID = l.ID,
                            EntryID = l.EntryID,
                            CategoryID = c.ID,
                            CategoryName = c.Name,
                            RoleName = r.Name,
                            Description = l.Description,
                            Username = l.Username,
                            Active = l.Active,
                            ModifiedDate = l.ModifiedDate
                        });
                }

                if (!criteria.SearchText.IsNullOrWhiteSpace())
                {
                    if (criteria.SearchBy == "1")
                    {
                        query = query.Where(o =>
                            o.CategoryName.Contains(criteria.SearchText) ||
                            o.RoleName.Contains(criteria.SearchText) ||
                            o.Description.Contains(criteria.SearchText) ||
                            o.Username.Contains(criteria.SearchText));
                    }
                    else if (criteria.SearchBy == "2")
                    {
                        query = query.Where(o => o.CategoryName.Contains(criteria.SearchText));
                    }
                    else if (criteria.SearchBy == "3")
                    {
                        query = query.Where(o => o.RoleName.Contains(criteria.SearchText));
                    }
                    else if (criteria.SearchBy == "4")
                    {
                        query = query.Where(o => o.Description.Contains(criteria.SearchText));
                    }
                    else if (criteria.SearchBy == "5")
                    {
                        query = query.Where(o => o.Username.Contains(criteria.SearchText));
                    }
                }

                results = query.ToArray();
            }

            RaiseListChangedEvents = false;

            foreach (var entity in results)
                Add(new Login(entity));

            RaiseListChangedEvents = true;
        }

        #endregion
    }
}
