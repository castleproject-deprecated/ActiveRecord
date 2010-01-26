namespace Castle.ActiveRecord.Linq
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq.Expressions;

	using Castle.ActiveRecord.Framework;

	using NHibernate.Linq;

	/// <summary>
	/// Default Active Record implementation of <see cref="QueryProvider"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class QueryProvider<T> : QueryProvider
	{
		private readonly QueryOptions options;

		static QueryProvider()
		{
			Experimental = true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="options"></param>
		public QueryProvider(QueryOptions options)
		{
			this.options = options;
		}

		/// <summary>
		/// when set to true, enables experimental support for LINQ queries without need to use explicit session scope.
		/// </summary>
		public static bool Experimental
		{
			get; set;
		}

		/// <inheritDoc />
		public override object Execute(Expression expression)
		{
			if(Experimental)
			{
				var type = expression.Type;
				if (type.IsGenericType)
				{
					var closingType = GetClosingType(type);
					if(closingType!=null)
					{
						return Activator.CreateInstance(typeof(LinqResultWrapper<>).MakeGenericType(closingType), options, expression,
						                                typeof(T));
					}
				}
			}

			var linqQuery = new LinqQuery<T>(options, expression, typeof(T));
			return ActiveRecordBase.ExecuteQuery(linqQuery);
		}

		private Type GetClosingType(Type type)
		{
			var arguments = type.GetGenericArguments();
			if (arguments.Length == 1)
			{
				return arguments[0];
			}

			foreach (var argument in arguments)
			{
				var closedEnumerable = typeof(IEnumerable<>).MakeGenericType(argument);
				if(closedEnumerable.IsAssignableFrom(type))
				{
					return argument;
				}
			}

			return null;
		}
	}

	/// <inheritDoc/>
	public class LinqResultWrapper<T> : IEnumerable<T>
	{
		private readonly QueryOptions options;
		private readonly Expression expression;
		private readonly Type rootType;
		private IList<T> list;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="options"></param>
		/// <param name="expression"></param>
		/// <param name="rootType"></param>
		public LinqResultWrapper(QueryOptions options, Expression expression, Type rootType)
		{
			this.options = options;
			this.rootType = rootType;
			this.expression = expression;
		}

		/// <inheritDoc/>
		public IEnumerator<T> GetEnumerator()
		{
			Populate();
			return list.GetEnumerator();
		}

		private void Populate()
		{
			var linqQuery = new LinqQuery<T>(options, expression,rootType);
			ActiveRecordBase.ExecuteQuery(linqQuery);
			list = linqQuery.Result;
		}

		/// <inheritDoc/>
		IEnumerator IEnumerable.GetEnumerator()
		{
			Populate();
			return list.GetEnumerator();
		}
	}
}
