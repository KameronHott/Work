using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Code.Utilities.Extensions;

namespace YRMC.SecureLogin.Business.Edits
{
    [Serializable]
    public class Role : Common.BusinessBase<Role, Data.SecurePasswordEntities, Data.Role>
    {
        #region [ Properties ]

        public static PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);
        public string Name
        {
            get { return GetProperty(NameProperty); }
            set { SetProperty(NameProperty, value); }
        }

        #endregion

        #region [ Business Rules ]

        protected override void AddBusinessRules()
        {
            base.AddBusinessRules();

            BusinessRules.AddRule(new Csla.Rules.CommonRules.Required(NameProperty));

            BusinessRules.AddRule(new Csla.Rules.CommonRules.Lambda(NameProperty, (context) =>
            {
                var target = (Role)context.Target;

                if (DataPortal.Execute<ExistsByNameCommand>(new ExistsByNameCommand(target.Name)).Exists)
                    context.AddErrorResult("The Role Name specified already exists.");
            }));
        }

        #endregion

        #region [ Authorization Rules ]

        private static void AddObjectAuthorizationRules()
        {
            string[] roles = new string[] { "Administrators" };

            Csla.Rules.BusinessRules.AddRule(typeof(Role), new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.GetObject, roles));
        }

        #endregion

        #region [ Events ]

        protected override void OnSaveProperties(Data.Role entity)
        {
            entity.Name = ReadProperty(NameProperty);
        }

        protected override void OnLoadProperties(Data.Role entity)
        {
            base.OnLoadProperties(entity);

            LoadProperty(NameProperty, entity.Name);
        }

        #endregion

        #region [ Data Access ]

        [Serializable]
        private class ExistsByNameCommand : Csla.CommandBase<ExistsByNameCommand>
        {
            #region [ Constructors ]

            public ExistsByNameCommand(string name)
            {
                Name = name;
            }

            #endregion

            #region [ Properties ]

            public string Name { get; set; }
            public bool Exists { get; set; }

            #endregion

            #region [ Data Access ]

            protected override void DataPortal_Execute()
            {
                using (Data.SecurePasswordEntities entities = new Data.SecurePasswordEntities())
                {
                    Exists =
                        (from item in entities.Roles
                         where item.Name == Name
                         select item).Count() > 0;
                }
            }

            #endregion
        }

        #endregion
    }
}
