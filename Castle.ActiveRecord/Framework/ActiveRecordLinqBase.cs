// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
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

namespace Castle.ActiveRecord.Framework
{
	using System;
	using System.Linq;
	using NHibernate;
	using NHibernate.Linq;

	/// <summary>
	/// A variation of the ActiveRecordBase class which provides the
	/// ability to use the record type in a linq expression.
	/// </summary>
	/// <typeparam name="T">The class which defines the active record entity.</typeparam>
	[Serializable]
	public class ActiveRecordLinqBase<T> : ActiveRecordBase<T>
	{
		/// <summary>
		/// The static property Queryable on the active record class is used as a Linq collection
		/// or as the in argument in a Linq expression. 
		/// 
		/// Examples include:
		/// var items = from f in Foo.Queryable select f;
		/// var item = Foo.Queryable.First();
		/// var items = from f in Foo.Queryable where f.Name == theName select f;
		/// var item = Foo.Queryable.First(f => f.Name == theName);
		/// </summary>
		public static IOrderedQueryable<T> Queryable
		{
			get
			{
				ISession session = holder.CreateSession(typeof(T));

				return session.AsQueryable<T>();
			}
		}
	}
}
