// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Castle.ActiveRecord
{
	using NHibernate;

	/// <summary>
	/// Interface for specific conversations using the ActiveRecord
	/// scope machinery. 
	/// </summary>
	public interface IScopeConversation : IConversation
	{
		/// <summary>
		/// Looks up or creates an <see cref="ISession"/> using the
		/// specified <see cref="ISessionFactory"/>.
		/// </summary>
		/// <param name="factory">The factory to use.</param>
		/// <param name="interceptor">An interceptor to include.</param>
		/// <returns>An open session.</returns>
		ISession GetSession(ISessionFactory factory, IInterceptor interceptor);
	}
}