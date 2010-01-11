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
	using System;
	using System.Collections.Generic;
	using System.Threading;
	using Framework;
	using Framework.Scopes;
	using NHibernate;

	/// <summary>
	/// An <see cref="IConversation"/> implementation using 
	/// <see cref="AbstractScope"/>.
	/// </summary>
	public class ScopedConversation : IScopeConversation
	{
		private bool canceled = false;

		/// <inheritDoc />
		public void Dispose()
		{
			//todo: Add possibility to rollback on failure
			foreach (var session in openedSessions.Values)
			{
				if (canceled)
					session.Transaction.Rollback();
				else
				{
					session.Transaction.Commit();
					session.Flush();
				}

				session.Dispose();
			}
		}

		/// <inheritDoc />
		public void Cancel()
		{
			canceled = true;
		}

		/// <inheritDoc />
		public ISession GetSession(ISessionFactory factory, IInterceptor interceptor)
		{
			if (canceled)
			{
				throw new ActiveRecordException(
					"A session was requested from a conversation that has "+
                    "been already canceled. Please check that after the "+
					"cancellation of a conversation no more "+
                    "ConversationalScopes are opened using it.");
			}
			if (!openedSessions.ContainsKey(factory))
				CreateSession(factory, interceptor);
			return openedSessions[factory];
		}

		private void CreateSession(ISessionFactory factory, IInterceptor interceptor)
		{
			var session = factory.OpenSession(interceptor);
			session.BeginTransaction();
			// todo: allow to configure this
			session.FlushMode = FlushMode.Auto;
			openedSessions.Add(factory,session);
		}

		private Dictionary<ISessionFactory, ISession> openedSessions = new Dictionary<ISessionFactory,ISession>();
	}
}