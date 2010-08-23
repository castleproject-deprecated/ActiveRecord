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


namespace Castle.ActiveRecord.Linq
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq.Expressions;

	using NHibernate;
	using NHibernate.Linq;

	/// <summary>
	/// Linq Active Record Query
	/// </summary>
	public class LinqQuery<T>:IActiveRecordQuery
	{
		private readonly QueryOptions options;
		private readonly Expression expression;
		private readonly Type rootType;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="options"></param>
		/// <param name="expression"></param>
		/// <param name="rootType"></param>
		public LinqQuery(QueryOptions options, Expression expression,Type rootType)
		{
			this.options = options;
			this.expression = expression;
			this.rootType = rootType;
		}

		/// <inheritDoc/>
		public Type RootType
		{
			get { return rootType; }
		}

		/// <inheritDoc/>
		public List<T> Result { get; private set; }

		/// <inheritDoc />
		public object Execute(ISession session)
		{
			var result = new NHibernateQueryProvider(session, options).Execute(expression);
			if (result is IEnumerable<T>)
				Result = new List<T>(result as IEnumerable<T>);
			return result;
		}

		/// <inheritDoc />
		public IEnumerable Enumerate(ISession session)
		{
			return (IEnumerable)Execute(session);
		}
	}
}
