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

namespace Castle.ActiveRecord.Tests.PropertyAccessTests
{
	using System;
	using NUnit.Framework;

	[TestFixture]
	class TestCases : NUnitInMemoryTest
	{
		public override Type[] GetTypes()
		{
			return new [] {typeof(Project), typeof(Task)};
		}

		[Test]
		public void TasksCollectionIsCreated()
		{
			createSampleProject();

			var project = Project.FindFirst();
			Assert.That(project.Tasks, Is.Not.Null);
			Assert.That(project.Tasks, Is.Not.Empty);
		}

		[Test]
		public void IsScheduledIsSetInDatabase()
		{
			createSampleProject();
			var tasks = Task.FindAllByProperty("IsScheduled", true);

			Assert.That(tasks, Is.Not.Null);
			Assert.That(tasks.Length, Is.EqualTo(1));
		}

		private void createSampleProject()
		{
			var project = new Project() {Name="Release AR"};
			project.Save();

			var task1 = new Task() {Name = "Create Tag", Project = project};
			var task2 = new Task() { Name = "Upload Zip", Project = project, ScheduledDate = DateTime.Today.AddDays(7)};
 
			task1.Save();
			task2.Save();
		}
	}
}
