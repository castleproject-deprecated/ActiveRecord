// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Itopcase" file="LazyInitializer.cs">
//   Copyright(C)2013
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Castle.ActiveRecord.ByteCode
{
    using System;
    using System.Reflection;

    using NHibernate;
    using NHibernate.Engine;
    using NHibernate.Proxy;
    using NHibernate.Type;

    internal class LazyInitializer : DefaultLazyInitializer
    {
        public LazyInitializer(string entityName, 
                               Type persistentClass, 
                               object id, 
                               MethodInfo getIdentifierMethod, 
                               MethodInfo setIdentifierMethod, 
                               IAbstractComponentType componentIdType, 
                               ISessionImplementor session)
            : base(
                entityName, 
                persistentClass, 
                id, 
                getIdentifierMethod, 
                setIdentifierMethod, 
                componentIdType, 
                session)
        {
        }

        /// <summary>
        /// Perform an ImmediateLoad of the actual object for the Proxy.
        /// </summary>
        public override void Initialize()
        {
            ISession newSession = null;
            try
            {
                //If the session has been disconnected, reconnect before continuing with the initialization.
                if (Session == null || !Session.IsOpen || !Session.IsConnected)
                {
                    newSession =
                        ActiveRecordMediator.GetSessionFactoryHolder()
                                            .CreateSession(PersistentClass);
                    Session = newSession.GetSessionImplementation();
                }

                base.Initialize();
            }
            finally
            {
                if (newSession != null)
                {
                    ActiveRecordMediator.GetSessionFactoryHolder().ReleaseSession(newSession);
                }
            }
        }
    }
}