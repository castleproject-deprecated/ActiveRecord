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

namespace Castle.ActiveRecord.Tests.Model.GenericModel
{
	using System.Collections;
	using System.Collections.Generic;

	using NHibernate;

	using Castle.ActiveRecord;
	using Castle.ActiveRecord.Framework;


	[ActiveRecord("BlogTable")]
	public class Blog : ActiveRecordBase<Blog>
	{
		public Blog()
		{
			Posts = new List<Post>();
		}

		public Blog(int i):this()
		{
			Id = i;
		}

		[PrimaryKey]
		public int Id { get; set; }

		[Property]
		public string Name { get; set; }

		[Property]
		public string Author { get; set; }

		[HasMany(Table = "Posts", Fetch = FetchEnum.Join, ColumnKey = "blogid")]
		public IList<Post> Posts { get; set; }

		[HasMany(typeof(Post), Table = "Posts", ColumnKey = "blogid", Where = "published = 1")]
		public IList PublishedPosts { get; set; }

		[HasMany(typeof(Post), Table = "Posts", ColumnKey = "blogid", Where = "published = 0")]
		public IList UnPublishedPosts { get; set; }

		[HasMany(typeof(Post), Table = "Posts", ColumnKey = "blogid", OrderBy = "created desc")]
		public IList RecentPosts { get; set; }

		public void CustomAction()
		{
			ActiveRecordMediator<Blog>.Execute(new NHibernateDelegate(MyCustomMethod), this);
		}

		private object MyCustomMethod(ISession session, object blogInstance)
		{
			session.Delete(blogInstance);
			session.Flush();

			return null;
		}

		internal static ISessionFactoryHolder Holder
		{
			get { return ActiveRecordMediator<Blog>.GetSessionFactoryHolder(); }
		}
	}
}
