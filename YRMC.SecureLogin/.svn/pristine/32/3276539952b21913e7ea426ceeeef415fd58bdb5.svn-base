using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Text;
using Csla.Reflection;
using Code.Utilities.Extensions;

namespace YRMC.SecureLogin.Business.Common
{
    [Serializable]
    public class BusinessBase<T, D, E> : Csla.BusinessBase<T>
        where T : BusinessBase<T, D, E>
        where D : ObjectContext
        where E : EntityObject
    {
        #region [ Properties ]

        public static Csla.PropertyInfo<Guid> IDProperty = RegisterProperty<Guid>(c => c.ID);
        public Guid ID
        {
            get { return GetProperty(IDProperty); }
            set { SetProperty(IDProperty, value); }
        }

        #endregion

        #region [ Business Rules ]

        protected override void AddBusinessRules()
        {
            base.AddBusinessRules();

            //BusinessRules.AddRule(new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.ReadProperty, IDProperty, typeof(T).Name + ".ID.Read"));
            //BusinessRules.AddRule(new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.WriteProperty, IDProperty, typeof(T).Name + ".ID.Write"));

            BusinessRules.AddRule(new Csla.Rules.CommonRules.Lambda(IDProperty, (context) =>
            {
                var target = (T)context.Target;
                if (target.ID == Guid.Empty)
                    context.AddErrorResult("ID cannot be empty");
            }));
        }

        #endregion

        #region [ Authorization Rules ]

        private static void AddObjectAuthorizationRules()
        {
            //Csla.Rules.BusinessRules.AddRule(typeof(T), new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.CreateObject, typeof(T).Name + ".Create()"));
            //Csla.Rules.BusinessRules.AddRule(typeof(T), new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.GetObject, typeof(T).Name + ".Get()"));
            //Csla.Rules.BusinessRules.AddRule(typeof(T), new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.EditObject, typeof(T).Name + ".Edit()"));
            //Csla.Rules.BusinessRules.AddRule(typeof(T), new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.DeleteObject, typeof(T).Name + ".Delete()"));
        }

        #endregion

        #region [ Factory Methods ]

        public static T Create()
        {
            return Csla.DataPortal.Create<T>();
        }

        #endregion

        #region [ Virtual Methods ]

        public override T Save()
        {
            BusinessRules.CheckRules();

            return base.Save();
        }

        #endregion

        #region [ Methods ]

        protected void LoadProperties(E entity)
        {
            using (BypassPropertyChecks)
            {
                OnLoadProperties(entity);
            }
        }

        protected string GetEntitySetName(D entities, E entity)
        {
            return entities
                .MetadataWorkspace
                .GetEntityContainer(entities.DefaultContainerName, System.Data.Metadata.Edm.DataSpace.CSpace)
                .BaseEntitySets
                    .Where(o => o.ElementType.Name == typeof(E).Name)
                    .Select(o => o.Name).First();
        }

        protected virtual void SaveEntityKey(D entities, E entity)
        {
            entity.EntityKey = new System.Data.EntityKey(entities.DefaultContainerName + "." + GetEntitySetName(entities, entity), "ID", ReadProperty(IDProperty));
        }

        protected virtual void SaveKeyProperties(E entity)
        {
            MethodCaller.CallPropertySetter(entity, "ID", ReadProperty(IDProperty));
        }

        #endregion

        #region [ Events ]

        protected virtual void OnCreate()
        {
            LoadProperty(IDProperty, new Guid().ToComb());
        }

        protected virtual void OnLoadProperties(E entity)
        {
            LoadProperty(IDProperty, MethodCaller.GetPropertyDescriptor(typeof(E), "ID").GetValue(entity));
        }

        protected virtual void OnSaveProperties(E entity)
        {
        }

        #endregion

        #region [ Data Access ]

        [Csla.RunLocal()]
        protected override void DataPortal_Create()
        {
            using (BypassPropertyChecks)
            {
                OnCreate();
            }

            MarkNew();
        }

        protected override void DataPortal_Insert()
        {
            if (IsNew)
            {
                using (D entities = (D)MethodCaller.CreateInstance(typeof(D)))
                {
                    E entity = (E)MethodCaller.CreateInstance(typeof(E));

                    SaveKeyProperties(entity);
                    OnSaveProperties(entity);

                    entities.AddObject(GetEntitySetName(entities, entity), entity);

                    entities.SaveChanges();
                }
            }
        }

        protected override void DataPortal_Update()
        {
            if (IsDirty)
            {
                using (D entities = (D)MethodCaller.CreateInstance(typeof(D)))
                {
                    E entity = (E)MethodCaller.CreateInstance(typeof(E));

                    SaveEntityKey(entities, entity);
                    SaveKeyProperties(entity);

                    entities.Attach(entity);

                    OnSaveProperties(entity);

                    entities.SaveChanges();
                }
            }
        }

        protected override void DataPortal_DeleteSelf()
        {
            if (!IsNew)
            {
                using (D entities = (D)MethodCaller.CreateInstance(typeof(D)))
                {
                    E entity = (E)MethodCaller.CreateInstance(typeof(E));

                    SaveEntityKey(entities, entity);
                    SaveKeyProperties(entity);

                    entities.Attach(entity);

                    entities.DeleteObject(entity);

                    entities.SaveChanges();
                }
            }
        }

        #endregion
    }
}
