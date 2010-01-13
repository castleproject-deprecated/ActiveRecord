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
		/// <summary>
		/// Creates a conversation with <see cref="ConversationFlushMode.Automatic"/>.
		/// </summary>
		public ScopedConversation() : this(ConversationFlushMode.Automatic)
		{
		}

		/// <summary>
		/// Creates a conversation with the chosen flush mode.
		/// </summary>
		/// <param name="mode">The flush mode to use</param>
		public ScopedConversation(ConversationFlushMode mode)
		{
			flushMode = mode;
		}

		private ConversationFlushMode flushMode;

		private bool _isCanceled = false;
		private Dictionary<ISessionFactory, ISession> openedSessions = new Dictionary<ISessionFactory,ISession>();

		/// <inheritDoc />
		public void Dispose()
		{
			ClearSessions();
		}

		private void ClearSessions()
		{
			foreach (var session in openedSessions.Values)
			{
				if (_isCanceled)
					session.Transaction.Rollback();
				else
				{
					if (flushMode != ConversationFlushMode.Explicit)
						session.Flush();
					session.Transaction.Commit();
				}

				session.Dispose();
			}
			openedSessions.Clear();
		}

		/// <inheritDoc />
		public void Cancel()
		{
			DoCancel(true, null);
		}

		private void DoCancel(bool userCanceled, Exception exceptionCaught)
		{
			_isCanceled = true;
			var args = new ConversationCanceledEventArgs(userCanceled, exceptionCaught);
			TriggerCanceled(args);
		}

		/// <inheritDoc />
		public void Flush()
		{
			AssertNotCanceled();
			foreach (var session in openedSessions.Values)
			{
				session.Flush();
			}
		}

		/// <inheritDoc />
		public void Restart()
		{
			ClearSessions();
			_isCanceled = false;
		}

		/// <inheritDoc />
		public ConversationFlushMode FlushMode
		{
			get { return flushMode; }
			set
			{
				if (openedSessions.Count > 0)
					throw new ActiveRecordException(
						"The FlushMode cannot be set after the "+
						"conversation was used for the first time. "+
						"A session was already opened, setting the "+
						"FlushMode must be done before any sessions "+
						"are opened.");
				flushMode = value;
			}
		}

		/// <inheritDoc />
		public bool IsCanceled
		{
			get { return _isCanceled; }
		}

		/// <summary>
		/// Executes the action.
		/// </summary>
		/// <param name="action">The action</param>
		/// <param name="silently">Whether to throw on exception</param>
		public void Execute(Action action, bool silently)
		{
			try
			{
				using (new ConversationalScope(this))
				{
					action();
				}
			}
			catch (Exception ex)
			{
				DoCancel(false, ex);
				if (!silently) 
					throw;
			}
		}

		/// <inheritDoc />
		public ISession GetSession(ISessionFactory factory, IInterceptor interceptor)
		{
			AssertNotCanceled();
			if (!openedSessions.ContainsKey(factory))
				CreateSession(factory, interceptor);
			return openedSessions[factory];
		}

		private void AssertNotCanceled()
		{
			if (_isCanceled)
			{
				throw new ActiveRecordException(
					"A session was requested from a conversation that has "+
					"been already _isCanceled. Please check that after the "+
					"cancellation of a conversation no more "+
					"ConversationalScopes are opened using it.");
			}
		}

		private void CreateSession(ISessionFactory factory, IInterceptor interceptor)
		{
			var session = factory.OpenSession(interceptor);
			session.BeginTransaction();
			session.FlushMode =
				flushMode == ConversationFlushMode.Automatic ? NHibernate.FlushMode.Auto :
				flushMode == ConversationFlushMode.OnClose ? NHibernate.FlushMode.Commit :
				NHibernate.FlushMode.Never;
			openedSessions.Add(factory,session);
		}

		/// <inheritDoc />
		public void Execute(Action action)
		{
			Execute(action,false);
		}

		/// <inheritDoc />
		public void ExecuteSilently(Action action)
		{
			Execute(action, true);
		}

		/// <inheritDoc />
		public event EventHandler<ConversationCanceledEventArgs> Canceled;

		private void TriggerCanceled(ConversationCanceledEventArgs args)
		{
			if (Canceled != null)
				Canceled(this, args);
		}
	}
}