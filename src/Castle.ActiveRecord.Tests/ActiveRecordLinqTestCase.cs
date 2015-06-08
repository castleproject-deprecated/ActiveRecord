﻿// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
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

using Castle.ActiveRecord.Tests.Model.Linq;

namespace Castle.ActiveRecord.Tests
{
	using System.Linq;

	using Castle.ActiveRecord.Framework;

	using NUnit.Framework;

	[TestFixture]
	public class ActiveRecordLinqTestCase : AbstractActiveRecordTest
	{
		[Test,
		 ExpectedException(typeof(ActiveRecordInitializationException),
			ExpectedMessage = "You can't invoke ActiveRecordStarter.Initialize more than once")]
		public void InitializeCantBeInvokedMoreThanOnce()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(), typeof(Post));
			ActiveRecordStarter.Initialize(GetConfigSource(), typeof(Blog));
		}

		[Test]
		public void SimpleOperations()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(), typeof(Post), typeof(Blog));

			using (new SessionScope())
			{
				Recreate();

				Post.DeleteAll();
				Blog.DeleteAll();

				var blogs = from b in Blog.Queryable select b;

				Assert.IsNotNull(blogs);
				Assert.AreEqual(0, blogs.Count());

				var blog = new Blog
							{
								Name = "hammett's blog",
								Author = "hamilton verissimo"
							};
				blog.Save();

				blogs = from b in Blog.Queryable select b;
				Assert.IsNotNull(blogs);
				Assert.AreEqual(1, blogs.Count());

				var retrieved = Blog.Queryable.First();
				Assert.IsNotNull(retrieved);

				Assert.AreEqual(blog.Name, retrieved.Name);
				Assert.AreEqual(blog.Author, retrieved.Author);
			}
		}

		[Test]
		public void SimpleOperationsShowingBug()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(), typeof(Post), typeof(Blog));

			using (new SessionScope())
			{
				Recreate();

				Post.DeleteAll();
				Blog.DeleteAll();

				var blogs = from b in Blog.Queryable select b;

				Assert.IsNotNull(blogs);
				Assert.AreEqual(0, blogs.Count());

				var blog = new Blog
							{
								Name = "hammett's blog",
								Author = "hamilton verissimo"
							};
				blog.Save();

				blogs = from b in Blog.Queryable select b;
				Assert.IsNotNull(blogs);
				Assert.AreEqual(1, blogs.Count());

				// this line will fail because of blogs.Count above
				var retrieved = blogs.First();
				Assert.IsNotNull(retrieved);

				Assert.AreEqual(blog.Name, retrieved.Name);
				Assert.AreEqual(blog.Author, retrieved.Author);
			}
		}

		[Test]
		public void SimpleOperations2()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(), typeof(Post), typeof(Blog));

			using (new SessionScope())
			{
				Recreate();

				Post.DeleteAll();
				Blog.DeleteAll();

				var blogs = Blog.Queryable;
				Assert.IsNotNull(blogs);
				Assert.AreEqual(0, blogs.Count());

				Blog blog = new Blog();
				blog.Name = "hammett's blog";
				blog.Author = "hamilton verissimo";
				blog.Create();

				Assert.AreEqual(1, (from b in Blog.Queryable select b).Count());

				blogs = Blog.Queryable;
				Assert.AreEqual(blog.Name, blogs.First().Name);
				Assert.AreEqual(blog.Author, blogs.First().Author);

				blog.Name = "something else1";
				blog.Author = "something else2";
				blog.Update();

				blogs = Blog.Queryable;
				Assert.IsNotNull(blogs);
				Assert.AreEqual(1, Blog.Queryable.Count());
				Assert.AreEqual(blog.Name, blogs.First().Name);
				Assert.AreEqual(blog.Author, blogs.First().Author);
			}
		}

		[Test]
		public void RelationsOneToMany()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(), typeof(Post), typeof(Blog));

			int blogId;

			Recreate();

			Post.DeleteAll();
			Blog.DeleteAll();

			Blog blog0 = new Blog();
			blog0.Name = "hammett's blog";
			blog0.Author = "hamilton verissimo";
			blog0.Save();

			Post post1 = new Post(blog0, "title1", "contents", "category1");
			Post post2 = new Post(blog0, "title2", "contents", "category2");

			post1.Save();
			post2.Save();

			blogId = blog0.Id;


			using (new SessionScope())
			{
				Blog blog = (from b in Blog.Queryable where b.Id == blogId select b).First();

				Blog blog2 = Blog.Queryable.First(b => b.Id == blogId);
				Assert.AreEqual(blog, blog2);

				Blog blog3 = Blog.Find(blogId);
				Assert.AreEqual(blog, blog3);

				Assert.IsNotNull(blog);
				Assert.IsNotNull(blog.Posts, "posts collection is null");
				Assert.AreEqual(2, blog.Posts.Count);

				foreach (Post post in blog.Posts)
				{
					Assert.AreEqual(blog.Id, post.Blog.Id);
				}
			}
		}

		[Test]
		public void UsingLinqFromNonLinqBaseClass()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(), typeof(Widget));

			Recreate();
			Widget.DeleteAll();

			Widget widget0 = new Widget { Name = "Hello world" };
			widget0.Save();

			using (new SessionScope())
			{
				var widgets = from w in ActiveRecordLinq.AsQueryable<Widget>() select w;
				Assert.IsNotNull(widgets);
				Assert.AreEqual(1, widgets.Count());

				var widget = (from w in ActiveRecordLinq.AsQueryable<Widget>() select w).First();
				Assert.IsNotNull(widget);
				Assert.AreEqual("Hello world", widget.Name);

				var widget2 = ActiveRecordLinq.AsQueryable<Widget>().First(w => w.Name == "Hello world");
				Assert.IsNotNull(widget2);
				Assert.AreEqual("Hello world", widget2.Name);

				Assert.AreEqual(widget2, widget);
			}
		}


		[Test]
		public void UsingLinqViaSessionScopeVariable()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(), typeof(Widget));

			using (ISessionScope scope = new SessionScope())
			{
				Recreate();
				Widget.DeleteAll();

				var widgets = from w in scope.AsQueryable<Widget>() select w;
				Assert.IsNotNull(widgets);
				Assert.AreEqual(0, widgets.Count());

				Widget widget = new Widget { Name = "Hello world" };
				widget.Save();

				widgets = from w in scope.AsQueryable<Widget>() where w.Name == "Hello world" select w;
				Assert.IsNotNull(widgets);
				Assert.AreEqual(1, widgets.Count());
			}
		}



		[Test, ExpectedException(typeof(ActiveRecordException))]
		public void Linq_without_session_scope_should_fail()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(), typeof(Widget));
			Recreate();
			Widget.DeleteAll();

			ActiveRecordLinq.AsQueryable<Widget>();
		}

		[Test]
		public void Projecting()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(), typeof(Widget));

			using (new SessionScope())
			{
				Recreate();
				Widget.DeleteAll();

				var widget = new Widget { Name = "foo" };
				widget.Save();

				var orderedQueryable = ActiveRecordLinqBase<Widget>.Queryable;
				var widgets = (from w in orderedQueryable
							   where w.Name.StartsWith("f")
							   select w.Name).ToList();

				Assert.IsNotNull(widgets);
				Assert.AreEqual("foo", widgets.Single());
			}
		}
		[Test]
		public void Projecting2()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(), typeof(Widget));

			using (new SessionScope())
			{
				Recreate();
				Widget.DeleteAll();

				var widget = new Widget { Name = "foo" };
				widget.Save();

				var orderedQueryable = ActiveRecordLinqBase<Widget>.Queryable;
				var name = (from w in orderedQueryable
							where w.Name.StartsWith("f")
							select w.Name).First();

				Assert.IsNotNull(name);
				Assert.AreEqual("foo", name);
			}
		}
	}
}
