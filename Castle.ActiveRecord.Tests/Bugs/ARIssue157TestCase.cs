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

namespace Castle.ActiveRecord.Tests.Bugs
{
	using System;

	using Castle.ActiveRecord.Framework;

	using NUnit.Framework;

	[TestFixture]
	public class ARIssue157TestCase : AbstractActiveRecordTest
	{
		public override void Init()
		{
			ActiveRecordStarter.ResetInitializationFlag();


		}

		[Test]
		public void Imports_on_joined_subclass_not_allowed()
		{
			const string message = "Type Castle.ActiveRecord.Tests.Bugs.OneKindOfDocument declares Imports " +
			                       "but it has a joined base class. All imports must be declared on a base class.";
			TestDelegate code = () =>

			ActiveRecordStarter.Initialize(GetConfigSource(),
											typeof(Document),
											typeof(OneKindOfDocument));

			Exception exception = 

			Assert.Throws(typeof(ActiveRecordException), code);
			Assert.AreEqual(message, exception.Message);
		}
	}

	[ActiveRecord, JoinedBase]
	public abstract class Document
	{
		[PrimaryKey]
		protected int Id { get; set; }
	}

	[ActiveRecord]
	[Import(typeof(DocumentSearchResult), "dsr")]
	public class OneKindOfDocument : Document
	{
		[JoinedKey("comp_id")]
		public int CompId { get; set; }
	}

	public class DocumentSearchResult
	{
	}
}