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
    using System.Linq;
    using NHibernate;
    using NUnit.Framework;

    [TestFixture]
    public class ConversationScenarioTest : NUnitInMemoryTest
    {
        public override Type[] GetTypes()
        {
            return new[] {typeof (BlogLazy), typeof (PostLazy)};
        }

        [Test]
        public void BasicScenario()
        {
            // Arrange
            ArrangeRecords();

        	// Act
            IScopeConversation conversation = new ScopedConversation();
            BlogLazy queriedBlog;
            using (new ConversationalScope(conversation))
            {
                queriedBlog = BlogLazy.Find(1);
            }

            // No scope here
            var fetchedPosts = queriedBlog.PublishedPosts;
            var firstPost = fetchedPosts[0] as PostLazy;
            firstPost.Published = false;

            using (new ConversationalScope(conversation))
            {
                firstPost.SaveAndFlush();
                queriedBlog.Refresh();
            }

            // Assert

            // Again, we're querying lazy properties out of scope
            Assert.That(queriedBlog.PublishedPosts, Is.Empty);
            Assert.That(queriedBlog.Posts, Is.Not.Empty);

            conversation.Dispose();
        }

    	[Test]
		public void CanCancelConversations()
		{
			ArrangeRecords();
    		using (var conversation = new ScopedConversation())
    		{
    			BlogLazy blog = null;
    			using (new ConversationalScope(conversation))
    			{
    				blog = BlogLazy.FindAll().First();
    			}

    			blog.Author = "Somebody else";

    			using (new ConversationalScope(conversation))
    			{
    				blog.SaveAndFlush();
    			}

    			conversation.Cancel();
    		}
			Assert.That(BlogLazy.FindAll().First().Author, Is.EqualTo("Markus"));
		}

		[Test]
		public void CanSetFlushModeToNever()
		{
			ArrangeRecords();

			using (var conversation = new ScopedConversation(ConversationFlushMode.Explicit))
			{
				BlogLazy blog;
				using (new ConversationalScope(conversation))
				{
					blog = BlogLazy.FindAll().First();
					blog.Author = "Anonymous";
					blog.Save();
					BlogLazy.FindAll(); // Triggers flushing if allowed
				}

				Assert.That(blog.Author, Is.EqualTo("Anonymous"));

				// Outside any ConversationalScope session-per-request is used
				Assert.That(BlogLazy.FindAll().First().Author, Is.EqualTo("Markus"));

				conversation.Flush();
			}

			Assert.That(BlogLazy.FindAll().First().Author, Is.EqualTo("Anonymous"));
		}

		[Test]
		public void CanSetFlushModeToOnClose()
		{
			ArrangeRecords();

			using (var conversation = new ScopedConversation(ConversationFlushMode.OnClose))
			{
				BlogLazy blog;
				using (new ConversationalScope(conversation))
				{
					blog = BlogLazy.FindAll().First();
					blog.Author = "Anonymous";
					blog.Save();
					BlogLazy.FindAll(); // Triggers flushing if allowed
				}

				Assert.That(blog.Author, Is.EqualTo("Anonymous"));

				// Outside any ConversationalScope session-per-request is used
				Assert.That(BlogLazy.FindAll().First().Author, Is.EqualTo("Markus"));

				// conversation.Flush(); // Only needed when set to explicit
			}

			Assert.That(BlogLazy.FindAll().First().Author, Is.EqualTo("Anonymous"));
		}

    	[Test]
    	public void CanRestartAConversationWithFreshSessions()
    	{
    		ISession s1, s2;
    		using (var c = new ScopedConversation())
    		{
    			using (new ConversationalScope(c))
    			{
    				BlogLazy.FindAll();
    				s1 = BlogLazy.Holder.CreateSession(typeof (BlogLazy));
    			}
				
				c.Restart();

				using (new ConversationalScope(c))
				{
					BlogLazy.FindAll();
					s2 = BlogLazy.Holder.CreateSession(typeof(BlogLazy));
				}    			

				Assert.That(s1, Is.Not.SameAs(s2));
    			Assert.That(s1.IsOpen, Is.False);
    			Assert.That(s2.IsOpen, Is.True);
    		}
    	}

    	[Test]
    	public void CanUseIConversationDirectly()
    	{
    		ArrangeRecords();

    		using (IConversation conversation = new ScopedConversation())
    		{
    			BlogLazy blog = null;
    			conversation.Execute(() => { blog = BlogLazy.FindAll().First(); });

				Assert.That(blog, Is.Not.Null);
				Assume.That(blog.Author, Is.EqualTo("Markus"));

				// Lazy access
				Assert.That(blog.PublishedPosts.Count, Is.EqualTo(1));

    			blog.Author = "Anonymous";

    			conversation.Execute(() => blog.Save());
				
    		}

			Assert.That(BlogLazy.FindAll().First().Author, Is.EqualTo("Anonymous"));
    	}



    	private void ArrangeRecords()
    	{
    		BlogLazy blog = new BlogLazy()
    		                	{
    		                		Author = "Markus",
    		                		Name = "Conversations"
    		                	};
    		PostLazy post = new PostLazy()
    		                	{
    		                		Blog = blog,
    		                		Category = "Scenario",
    		                		Title = "The Convesration is here",
    		                		Contents = "A new way for AR in fat clients",
    		                		Created = new DateTime(2010, 1, 1),
    		                		Published = true
    		                	};
    		blog.Save();
    		post.Save();
    	}
    }
}
