namespace Castle.ActiveRecord.Tests.Bugs
{
	using Model;
	using NUnit.Framework;

	[TestFixture]
	public class ARIssue293TestCase : AbstractActiveRecordTest
	{
		[Test]
		public void Should_execute_formula()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(), typeof(Post), typeof(Blog));
			Recreate();

			Post.DeleteAll();
			Blog.DeleteAll();

			Blog blog = new Blog();
			blog.Name = "hammett's blog";
			blog.Author = "hamilton verissimo";
			blog.Save();

			var blogs = Blog.FindAll();

			Assert.AreEqual(2, blogs[0].SomeFormula);
		}
	}
}
