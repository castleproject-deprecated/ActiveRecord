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

	/// <summary>
	/// Determines when the sessions in a conversation flush.
	/// </summary>
	public enum ConversationFlushMode
	{
		/// <summary>
		/// FlushMode is set to automatic, all sessions flush
		/// whenever NHibernate needs it.
		/// </summary>
		Automatic,

		/// <summary>
		/// All information is flushed when the conversation is
		/// disposed and was not canceled
		/// </summary>
		OnClose,

		/// <summary>
		/// Conversation must be flushed explicitly.
		/// </summary>
		Explicit
	}

	/// <summary>
    /// Conversations allow to define broader units of work
    /// than <see cref="SessionScope"/> allows to.
    /// </summary>
    public interface IConversation : IDisposable
    {
		/// <summary>
		/// Cancels all changes made in this session.
		/// </summary>
		void Cancel();

		/// <summary>
		/// Flushes all sessions in this conversation.
		/// </summary>
		void Flush();

		/// <summary>
		/// Resets the conversation, allowing it to be used again
		/// with new sessions after canceling.
		/// <remarks>
		/// This functionality supports serving instances through
		/// IoC where it is not possible to simple create a new
		/// conversation after an error. Restarting the conversation
		/// offers error recovery in such cases.
		/// </remarks>
		/// </summary>
		void Restart();

		/// <summary>
		/// The FlushMode to use. Setting the Flushmode via
		/// property allows using IoC-Containers for
		/// providing Conversation objects and configuring
		/// them afterwards.
		/// Setting this property is only supported before
		/// the conversation is actually used.
		/// </summary>
		ConversationFlushMode FlushMode { get; set; }

		/// <summary>
		/// Whether the conversation is canceled
		/// </summary>
		bool Canceled { get; }
    }
}