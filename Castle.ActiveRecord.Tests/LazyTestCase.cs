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

namespace Castle.ActiveRecord.Tests
{
	using NUnit.Framework;

	using Castle.ActiveRecord.Tests.Model;

	[TestFixture]
	public class LazyTestCase : AbstractActiveRecordTest
	{
		[Test]
		public void CanSaveAndLoadLazyEntityOutsideOfScope()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(),
				   typeof(BlogLazy), typeof(PostLazy));
			Recreate();
			BlogLazy blog = new BlogLazy();
			blog.Save();
			PostLazy post = new PostLazy(blog, "a", "b", "c");
			post.Save();

			PostLazy postFromDb = PostLazy.Find(post.Id);
			Assert.AreEqual("a", postFromDb.Title);
		}

		[Test]
		public void CanSaveAndLoadLazy()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(),typeof(VeryLazyObject));
			Recreate();
			VeryLazyObject lazy = new VeryLazyObject();
			lazy.Title = "test";

			ActiveRecordMediator.Save(lazy);

			VeryLazyObject lazyFromDb = (VeryLazyObject)ActiveRecordMediator.FindByPrimaryKey(typeof(VeryLazyObject),lazy.Id);
			Assert.AreEqual("test", lazyFromDb.Title);

			lazyFromDb.Title = "test for update";
			ActiveRecordMediator.Update(lazyFromDb);

			lazyFromDb = (VeryLazyObject)ActiveRecordMediator.FindByPrimaryKey(typeof(VeryLazyObject), lazy.Id);
			Assert.AreEqual("test for update", lazyFromDb.Title);
		}

		[Test]
		public void CanLoadLazyProperty()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(), typeof(LazyObjectWithLazyBlobProperty));
			Recreate();

			string teststring = @"data:image/png;base64, iVBORw0KGgoAAAANSUhEUgAAAAoAAAAKCAYAAACNMs+9AAAABGdBTUEAALGPC/xhBQAAAAlwSFlzAAALEwAACxMBAJqcGAAAAAd0SU1FB9YGARc5KB0XV+IAAAAddEVYdENvbW1lbnQAQ3JlYXRlZCB3aXRoIFRoZSBHSU1Q72QlbgAAAF1JREFUGNO9zL0NglAAxPEfdLTs4BZM4DIO4C7OwQg2JoQ9LE1exdlYvBBeZ7jqch9//q1uH4TLzw4d6+ErXMMcXuHWxId3KOETnnXXV6MJpcq2MLaI97CER3N0vr4MkhoXe0rZigAAAABJRU5ErkJggg==";
			int id;

			using (new SessionScope()) {
				LazyObjectWithLazyBlobProperty lazy = new LazyObjectWithLazyBlobProperty();

				lazy.BlobData = System.Text.Encoding.UTF8.GetBytes(teststring);
				ActiveRecordMediator.Save(lazy);
				id = lazy.Id;
			}

			using (new SessionScope()) {
				LazyObjectWithLazyBlobProperty lazyFromDb = (LazyObjectWithLazyBlobProperty) ActiveRecordMediator.FindByPrimaryKey(typeof (LazyObjectWithLazyBlobProperty), id);
				Assert.True(!NHibernate.NHibernateUtil.IsPropertyInitialized(lazyFromDb, "BlobData"));

				byte[] fromDb = lazyFromDb.BlobData;
				Assert.True(NHibernate.NHibernateUtil.IsPropertyInitialized(lazyFromDb, "BlobData"));

				Assert.AreEqual(teststring, System.Text.Encoding.UTF8.GetString(fromDb));
			}
		}

        [Test]
        public void CanLoadLazyBelongsToOutsideOfScope() {
            ActiveRecordStarter.Initialize(GetConfigSource(),
                   typeof(ObjectWithLazyAssociation), typeof(VeryLazyObject));
            Recreate();

            var lazy = new VeryLazyObject();
            lazy.Title = "test";
            ActiveRecordMediator.Save(lazy);

            var obj = new ObjectWithLazyAssociation();
            obj.LazyObj = lazy;
            ActiveRecordMediator.Save(obj);

            var objFromDb = (ObjectWithLazyAssociation)ActiveRecordMediator.FindByPrimaryKey(typeof(ObjectWithLazyAssociation), obj.Id);
            Assert.False(NHibernate.NHibernateUtil.IsInitialized(objFromDb.LazyObj));
            Assert.AreEqual("test", objFromDb.LazyObj.Title);
            Assert.True(NHibernate.NHibernateUtil.IsInitialized(objFromDb.LazyObj));
        }
	}
}
