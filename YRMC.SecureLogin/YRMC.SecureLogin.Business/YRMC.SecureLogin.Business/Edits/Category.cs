﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Code.Utilities.Extensions;

namespace YRMC.SecureLogin.Business.Edits
{
    [Serializable]
    public class Category : Common.BusinessBase<Category, Data.SecurePasswordEntities, Data.Category>
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
                var target = (Category)context.Target;

                if (DataPortal.Execute<ExistsByNameCommand>(new ExistsByNameCommand(target.Name)).Exists)
                    context.AddErrorResult("The Category Name specified already exists.");
            }));
        }

        #endregion

        #region [ Events ]

        protected override void OnSaveProperties(Data.Category entity)
        {
            entity.Name = ReadProperty(NameProperty);
        }

        protected override void OnLoadProperties(Data.Category entity)
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
                        (from item in entities.Categories
                         where item.Name == Name
                         select item).Count() > 0;
                }
            }

            #endregion
        }

        #endregion
    }
}