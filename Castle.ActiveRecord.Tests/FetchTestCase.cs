namespace Castle.ActiveRecord.Tests
{
	using NUnit.Framework;

	using Castle.ActiveRecord.Tests.Model.GenericModel;

	[TestFixture]
	public class FetchTestCase : AbstractActiveRecordTest
	{
		public override void Init()
		{
			base.Init();

			ActiveRecordStarter.Initialize(GetConfigSource(),
				typeof(Blog),
				typeof(Post),
				typeof(Company),
				typeof(Award),
				typeof(Employee),
				typeof(Person));

			Recreate();

			Post.DeleteAll();
			Blog.DeleteAll();
			Company.DeleteAll();
			Award.DeleteAll();
			Employee.DeleteAll();
		}


		[Test]
		public void FetchEnum_Join_on_a_HasMany_property_should_not_return_duplicate_records()
		{
			Blog[] blogs = Blog.FindAll();

			Assert.IsNotNull(blogs);
			Assert.AreEqual(0, blogs.Length);

			var blog = new Blog { Name = "Test blog", Author = "Eric Bowen" };

			blog.Save();

			var post = new Post(blog, "Post1", "Content1", "Category1");
			post.Save();

			blog.Posts.Add(post);

			var post2 = new Post(blog, "Post2", "Content2", "Category2");
			post2.Save();

			blog.Posts.Add(post2);

			blog.Save();

			blogs = Blog.FindAll();

			Assert.IsNotNull(blogs);
			Assert.AreEqual(1, blogs.Length);

		}
	}
}
