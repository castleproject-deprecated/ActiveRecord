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

namespace Castle.ActiveRecord
{
	using System;

	/// <summary>
	/// Maps a standard column of the table.
	/// </summary>
	/// <example>
	/// In the following example, the column is also
	/// called 'name', so you don't have to specify.
	/// <code>
	/// public class Blog : ActiveRecordBase
	/// {
	///		...
	///		
	///		[Property]
	///		public int Name
	///		{
	///			get { return _name; }
	///			set { _name = value; }
	///		}
	///	</code>
	/// To map a column name, use 
	/// <code>
	///		[Property("blog_name")]
	///		public int Name
	///		{
	///			get { return _name; }
	///			set { _name = value; }
	///		}
	///	</code>
	/// </example>
	[AttributeUsage(AttributeTargets.Property), Serializable]
	public class PropertyAttribute : WithAccessOptionalTableAttribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyAttribute"/> class.
		/// </summary>
		public PropertyAttribute()
		{
			Insert = true;
			Update = true;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyAttribute"/> class.
		/// </summary>
		/// <param name="column">The column.</param>
		public PropertyAttribute(String column) : this()
		{
			Column = column;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyAttribute"/> class.
		/// </summary>
		/// <param name="column">The column.</param>
		/// <param name="type">The type.</param>
		public PropertyAttribute(String column, String type) : this(column)
		{
			ColumnType = type;
		}

		/// <summary>
		/// Gets or sets a value indicating whether this property allow null.
		/// </summary>
		/// <value><c>true</c> if the value is not nullable; otherwise, <c>false</c>.</value>
		public bool NotNull { get; set; }

		/// <summary>
		/// Gets or sets the length of the property (for strings - nvarchar(50) )
		/// </summary>
		/// <value>The length.</value>
		public int Length { get; set; }

		/// <summary>
		/// Gets or sets the column name
		/// </summary>
		/// <value>The column.</value>
		public string Column { get; set; }

		/// <summary>
		/// Set to <c>false</c> to ignore this property when updating entities of this ActiveRecord class.
		/// </summary>
		public bool Update { get; set; }

		/// <summary>
		/// Set to <c>false</c> to ignore this property when inserting entities of this ActiveRecord class.
		/// </summary>
		public bool Insert { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="PropertyAttribute"/> is unique.
		/// </summary>
		/// <value><c>true</c> if unique; otherwise, <c>false</c>.</value>
		public bool Unique { get; set; }

		/// <summary>
		/// Set to <c>true</c> to load this property lazily.
		/// </summary>
		public bool Lazy { get; set; }

		/// <summary>
		/// Gets or sets the formula used to calculate this property
		/// </summary>
		/// <value>The formula.</value>
		public string Formula { get; set; }

		/// <summary>
		/// Gets or sets the type of the column.
		/// </summary>
		/// <value>The type of the column.</value>
		public string ColumnType { get; set; }

		/// <summary>
		/// From NHibernate documentation:
		/// A unique-key attribute can be used to group columns 
		/// in a single unit key constraint. 
		/// </summary>
		/// <value>unique key name</value>
		/// <remarks>
		/// Currently, the 
		/// specified value of the unique-key attribute is not 
		/// used to name the constraint, only to group the columns 
		/// in the mapping file.
		/// </remarks>
		public string UniqueKey { get; set; }

		/// <summary>
		/// From NHibernate documentation:
		/// specifies the name of a (multi-column) index
		/// </summary>
		/// <value>index name</value>
		public string Index { get; set; }

		/// <summary>
		/// From NHibernate documentation:
		/// overrides the default column type
		/// </summary>
		/// <value>column_type</value>
		public string SqlType { get; set; }

		/// <summary>
		/// From NHibernate documentation:
		/// create an SQL check constraint on either column or table
		/// </summary>
		/// <value>Sql Expression</value>
		public string Check { get; set; }

		/// <summary>
		/// Gets or sets the default value for a column (used by schema generation). 
		/// Please note that you should be careful to set Insert=false or set the value to the same 
		/// as the default on the database. 
		/// </summary>
		/// <value>The default value for the column.</value>
		public string Default { get; set; }

		/// <summary>
		/// Set to <c>true</c> if this property overrides a property in a base class
		/// </summary>
		public bool IsOverride { get; set; }
	}
}
