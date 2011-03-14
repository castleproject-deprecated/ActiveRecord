// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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

using NHibernate.Bytecode;
using NHibernate.Proxy;

namespace Castle.ActiveRecord.ByteCode 
{
    /// <summary>
    /// The factory infrastructure used to build AR proxy objects.
    /// Use this one if you want automatic session management durring proxy hydration.
    /// </summary>
    public class ProxyFactoryFactory : IProxyFactoryFactory 
    {

        public IProxyFactory BuildProxyFactory() 
        {
            return new ProxyFactory();
        }

        public IProxyValidator ProxyValidator 
        {
            get { return new DynProxyTypeValidator(); }
        }

        public bool IsInstrumented(System.Type entityClass) 
        {
            return true;
        }
    }
}
