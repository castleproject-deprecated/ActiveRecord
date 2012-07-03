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
	using DynamicProxy;
	using NHibernate;
	using NHibernate.ByteCode.Castle;
	using NHibernate.Engine;
	using NHibernate.Proxy;

    class ProxyFactory : AbstractProxyFactory 
    {
		protected static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(ProxyFactory));
        private static readonly ProxyGenerator ProxyGenerator = new ProxyGenerator();

        protected static ProxyGenerator DefaultProxyGenerator 
        {
            get { return ProxyGenerator; }
        }

        /// <summary>
		/// Build a proxy using the Castle.DynamicProxy library, that overrides the default <see cref="NHibernate.ByteCode.Castle.LazyInitializer"/>
        /// </summary>
        /// <param name="id">The value for the Id.</param>
        /// <param name="session">The Session the proxy is in.</param>
        /// <returns>A fully built <c>INHibernateProxy</c>.</returns>
        public override INHibernateProxy GetProxy(object id, ISessionImplementor session) 
        {
            try
            {
            	var initializer = new LazyInitializer(EntityName, PersistentClass, id, GetIdentifierMethod,
            	                                      SetIdentifierMethod, ComponentIdType, session);

                object generatedProxy = IsClassProxy
                                            ? ProxyGenerator.CreateClassProxy(PersistentClass, Interfaces, initializer)
                                            : ProxyGenerator.CreateInterfaceProxyWithoutTarget(Interfaces[0], Interfaces,
                                                                                                initializer);

                initializer._constructed = true;
                return (INHibernateProxy)generatedProxy;
            } 
            catch (Exception e) 
            {
                log.Error("Creating a proxy instance failed", e);
                throw new HibernateException("Creating a proxy instance failed", e);
            }
        }

		/// <summary>
		/// Returns a proxy capable of field interception.
		/// </summary>
		/// <returns></returns>
        public override object GetFieldInterceptionProxy(object instanceToWrap) 
        {
            var proxyGenerationOptions = new ProxyGenerationOptions();
            var interceptor = new LazyFieldInterceptor();
            proxyGenerationOptions.AddMixinInstance(interceptor);
            return ProxyGenerator.CreateClassProxy(PersistentClass, proxyGenerationOptions, interceptor);
        }
    }
}
