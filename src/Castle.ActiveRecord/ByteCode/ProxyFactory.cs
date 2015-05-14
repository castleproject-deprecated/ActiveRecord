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
	using NHibernate;
	
	using NHibernate.Engine;
	using NHibernate.Intercept;
	using NHibernate.Proxy;

    internal class ProxyFactory : AbstractProxyFactory 
    {
        private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(ProxyFactory));
        private readonly NHibernate.Proxy.DynamicProxy.ProxyFactory factory = new NHibernate.Proxy.DynamicProxy.ProxyFactory();

        /// <summary>
		/// Build a proxy using the Castle.DynamicProxy library, that overrides the default <see cref="LazyInitializer"/>
        /// </summary>
        /// <param name="id">The value for the Id.</param>
        /// <param name="session">The Session the proxy is in.</param>
        /// <returns>A fully built <c>INHibernateProxy</c>.</returns>
        public override INHibernateProxy GetProxy(object id, ISessionImplementor session) 
        {
            try
            {
                var defaultLazyInitializer = new LazyInitializer(this.EntityName, this.PersistentClass, id, this.GetIdentifierMethod, this.SetIdentifierMethod, this.ComponentIdType, session);
                return this.IsClassProxy ? (INHibernateProxy)this.factory.CreateProxy(this.PersistentClass, defaultLazyInitializer, this.Interfaces) : (INHibernateProxy)this.factory.CreateProxy(this.Interfaces[0], defaultLazyInitializer, this.Interfaces);
            }
            catch (Exception ex)
            {
                log.Error((object)"Creating a proxy instance failed", ex);
                throw new HibernateException("Creating a proxy instance failed", ex);
            }
        }
    }
}
