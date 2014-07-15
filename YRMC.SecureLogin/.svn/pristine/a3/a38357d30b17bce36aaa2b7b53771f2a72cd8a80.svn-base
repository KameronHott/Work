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
    public class BusinessLinkBase<T, D, E> : Common.BusinessBase<T, D, E>
        where T : BusinessLinkBase<T, D, E>
        where D : ObjectContext
        where E : EntityObject
    {
        #region [ Properties ]

        public static Csla.PropertyInfo<Guid> PIDProperty = RegisterProperty<Guid>(c => c.PID);
        public Guid PID
        {
            get { return GetProperty(PIDProperty); }
            set { SetProperty(PIDProperty, value); }
        }

        #endregion

        #region [ Methods ]

        protected override void SaveEntityKey(D entities, E entity)
        {
            entity.EntityKey = new System.Data.EntityKey(
                entities.DefaultContainerName + "." + GetEntitySetName(entities, entity),
                new [] { 
                    new KeyValuePair<string, object>("ID", ReadProperty(IDProperty)),
                    new KeyValuePair<string, object>("PID", ReadProperty(PIDProperty))
                });
        }

        protected override void SaveKeyProperties(E entity)
        {
            base.SaveKeyProperties(entity);

            MethodCaller.CallPropertySetter(entity, "PID", ReadProperty(PIDProperty));
        }

        #endregion

        #region [ Events ]

        protected override void OnLoadProperties(E entity)
        {
            base.OnLoadProperties(entity);

            LoadProperty(PIDProperty, MethodCaller.GetPropertyDescriptor(typeof(E), "PID").GetValue(entity));
        }

        #endregion
    }
}
