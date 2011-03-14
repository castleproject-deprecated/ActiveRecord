// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.ActiveRecord.ByteCode
{
	using System;
	using System.Reflection;
	using NHibernate;
	using NHibernate.Engine;
	using NHibernate.Type;

    class LazyInitializer : NHibernate.ByteCode.Castle.LazyInitializer 
    {
        public LazyInitializer(string entityName, Type persistentClass, object id, 
                               MethodInfo getIdentifierMethod, MethodInfo setIdentifierMethod, 
                               IAbstractComponentType componentIdType, ISessionImplementor session) : 
            base(entityName, persistentClass, id, getIdentifierMethod, setIdentifierMethod, componentIdType, session) { }

        /// <summary>
        /// Perform an ImmediateLoad of the actual object for the Proxy.
        /// </summary>
        public override void Initialize() {
            ISession newSession = null;
            try 
            {
                //If the session has been disconnected, reconnect before continuing with the initialization.
                if (Session == null || !Session.IsOpen || !Session.IsConnected) {
                    newSession = ActiveRecordMediator.GetSessionFactoryHolder().CreateSession(PersistentClass);
                    Session = newSession.GetSessionImplementation();
                }
                base.Initialize();
            }
            finally 
            {
                if (newSession != null) ActiveRecordMediator.GetSessionFactoryHolder().ReleaseSession(newSession);
            }
        }
    }
}
