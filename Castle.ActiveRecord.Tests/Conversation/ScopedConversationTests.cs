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

namespace Castle.ActiveRecord.Tests.Conversation
{
	using System;
	using Framework;
	using NHibernate;
	using NUnit.Framework;

	[TestFixture]
	public class ScopedConversationTests : NUnitInMemoryTest
	{
		public override Type[] GetTypes()
		{
			return new[] { typeof(BlogLazy), typeof(PostLazy) };
		}
		
		[Test]
		public void SessionsAreKeptThroughoutTheConversation()
		{
			IScopeConversation conversation = new ScopedConversation();
			ISession session = null;

			using (new ConversationalScope(conversation))
			{
				BlogLazy.FindAll();
				session = BlogLazy.Holder.CreateSession(typeof (BlogLazy));
			}

			Assert.That(session.IsOpen);

			using (new ConversationalScope(conversation))
			{
				BlogLazy.FindAll();
				Assert.That(BlogLazy.Holder.CreateSession(typeof(BlogLazy)), Is.SameAs(session));
			}

			conversation.Dispose();
			Assert.That(session.IsOpen, Is.False);
		}
	
		[Test]
		public void ThrowsReasonableErrorMessageWhenUsedAfterCancel()
		{
			using (var conversation = new ScopedConversation())
			{
				conversation.Cancel();

				var ex = Assert.Throws<ActiveRecordException>(() =>
				                                              	{
				                                              		using (new ConversationalScope(conversation))
				                                              			BlogLazy.FindAll();
				                                              	}
					);

				Assert.That(ex.Message.Contains("cancel"));
				Assert.That(ex.Message.Contains("ConversationalScope"));
				Assert.That(ex.Message.Contains("session"));
				Assert.That(ex.Message.Contains("request"));
			}
		}

		[Test]
		public void CannotFlushAfterCancel()
		{
			using (var conv = new ScopedConversation(ConversationFlushMode.Explicit))
			{
				conv.Cancel();
				Assert.Throws<ActiveRecordException>(() => { conv.Flush(); });
			}
		}

		[Test]
		public void CanSetFlushModeAfterCreation()
		{
			using (var conv=new ScopedConversation())
			{
				conv.FlushMode = ConversationFlushMode.Explicit;
				Assert.That(conv.FlushMode, Is.EqualTo(ConversationFlushMode.Explicit));
			}
		}

		[Test]
		public void CannotSetFlushModeAfterUsingTheConversation()
		{
			using (var c = new ScopedConversation())
			{
				using (new ConversationalScope(c))
				{
					BlogLazy.FindAll();
				}

				var ex = Assert.Throws<ActiveRecordException>(
					()=>c.FlushMode=ConversationFlushMode.Explicit);

				Assert.That(ex.Message, Is.Not.Null
					.And.Contains("FlushMode")
					.And.Contains("set")
					.And.Contains("after")
					.And.Contains("session"));
			}
		}
		[Test]
		public void CanCheckWhetherAConversationWasCanceled()
		{
			using (var c = new ScopedConversation())
			{
				Assert.That(c.IsCanceled, Is.False);
				c.Cancel();
				Assert.That(c.IsCanceled);
			}
		}

		[Test]
		public void CanRestartAConversation()
		{
			using (var c = new ScopedConversation())
			{
				Assert.That(c.IsCanceled, Is.False);
				c.Cancel();
				Assert.That(c.IsCanceled);
				c.Restart();
				Assert.That(c.IsCanceled, Is.False);
			}
		}

		[Test]
		public void ConversationIsCanceledAfterErrorInExecute()
		{
			using (var c = new ScopedConversation())
			{
				Assert.Throws<ApplicationException>(() => c.Execute(() => new PostLazy().SaveWithException()));
				Assert.That(c.IsCanceled);
			}
		}

		[Test]
		public void ConversationIsCanceledAfterSilentError()
		{
			using (var c = new ScopedConversation())
			{
				c.ExecuteSilently(() => new PostLazy().SaveWithException());
				Assert.That(c.IsCanceled);
			}
		}

		[Test]
		public void TriggersEventWhenCanceling()
		{

			bool triggered = false;
			using (var c = new ScopedConversation())
			{
				c.Canceled += (conv,args) => triggered = true;
				c.Cancel();
			}

			Assert.That(triggered);
		}

		[Test]
		public void TriggersWithRightArgsWhenCanceling()
		{

			bool triggered = false;
			bool userCancel = false;
			using (var c = new ScopedConversation())
			{
				c.Canceled += (conv, args) => { triggered = true;
				                              	userCancel = args.CanceledByUser; };
				c.Cancel();
			}

			Assert.That(triggered);
			Assert.That(userCancel);
		}

		[Test]
		public void TriggersWithRightArgsWhenCancelingAfterException()
		{

			bool triggered = false;
			Exception ex = null;
			using (var c = new ScopedConversation())
			{
				c.Canceled += (conv, args) =>
				{
					triggered = true;
					ex = args.Exception;
				};
				c.ExecuteSilently(()=>new PostLazy().SaveWithException());
			}

			Assert.That(triggered);
			Assert.That(ex, Is.Not.Null
				.And.InstanceOf<ApplicationException>());
		}

	}
}