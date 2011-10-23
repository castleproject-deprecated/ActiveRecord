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

namespace Castle.ActiveRecord
{
	using System;
	using Framework.Scopes;
	using NHibernate;

	/// <summary>
	/// Scope-implementation making use of a <see cref="ScopedConversation"/>.
	/// </summary>
	public class ConversationalScope : AbstractScope
	{
		/// <summary>
		/// The conversation to use.
		/// </summary>
		protected IScopeConversation conversation;

		/// <summary>
		/// Minimum constructor which sets at least the conversation to
		/// use.
		/// </summary>
		/// <param name="conversation"></param>
		public ConversationalScope(IScopeConversation conversation)
			: base(FlushAction.Config, SessionScopeType.Transactional)
		{
			this.conversation = conversation;
		}

		/// <summary>
		/// Notifies the <see cref="conversation"/> that the session has
		/// failed.
		/// </summary>
		/// <param name="session">The failed sessions.</param>
		public override void FailSession(ISession session)
		{
			throw new NotImplementedException();
		}
		
		/// <summary>
		/// Always <c>true</c>, we take sessions from the <see cref="IConversation"/>.
		/// </summary>
		public override bool WantsToCreateTheSession { get { return true; } }


		/// <summary>
		/// Delegate opening session to <see cref="IConversation"/>
		/// </summary>
		/// <param name="sessionFactory">The factory to use for this type</param>
		/// <param name="interceptor">An interceptor to include</param>
		/// <returns>A valid session from the <see cref="ISessionFactory"/>.</returns>
		public override ISession OpenSession(ISessionFactory sessionFactory, IInterceptor interceptor)
		{
			return conversation.GetSession(sessionFactory, interceptor);
		}
	}
}