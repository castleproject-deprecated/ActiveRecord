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

namespace Castle.ActiveRecord
{
	using System;

	/// <summary>
	/// Define the possible strategies to set the Primary Key values
	/// </summary>
	[Serializable]
	public enum PrimaryKeyType
	{
		/// <summary>
		/// Use Identity column (auto number)
		/// Note: This force an immediate call to the DB when Create() is called
		/// </summary>
		Identity,
		/// <summary>
		/// Use a sequence
		/// </summary>
		Sequence,
		/// <summary>
		/// Use the HiLo algorithm to get the next value
		/// </summary>
		HiLo,
		/// <summary>
		/// Use a sequence and a HiLo algorithm - better performance on Oracle
		/// </summary>
		SeqHiLo,
		/// <summary>
		/// Use the hex representation of a unique identifier
		/// </summary>
		UuidHex,
		/// <summary>
		/// Use the string representation of a unique identifier
		/// </summary>
		UuidString,
		/// <summary>
		/// Generate a Guid for the primary key
		/// Note: You should prefer using GuidComb over this value.
		/// </summary>
		Guid,
		/// <summary>
		/// Generate a Guid in sequence, so it will have better insert performance in the DB.
		/// </summary>
		GuidComb,
		/// <summary>
		/// Use an identity or sequence if supported by the database, otherwise, use the HiLo algorithm
		/// </summary>
		Native,
		/// <summary>
		/// The primary key value is always assigned.
		/// Note: using this you will lose the ability to call Save(), and will need to call Create() or Update()
		/// explicitly.
		/// </summary>
		Assigned,
		/// <summary>
		/// This is a foreign key to another table
		/// </summary>
		Foreign,
		/// <summary>
		/// Returns a <c>Int64</c> constructed from the system
		/// time and a counter value.
		/// </summary>
		/// <remarks>
		/// Not safe for use in a clustser
		/// </remarks>
		Counter,
		/// <summary>
		/// Returns a <c>Int64</c>, constructed by counting from 
		/// the maximum primary key value at startup. 
		/// </summary>
		/// <remarks>
		/// Not safe for use in a cluster
		/// </remarks>
		Increment,
		/// <summary>
		/// A custom generator will be provided. See <see cref="PrimaryKeyAttribute.CustomGenerator"/>
		/// </summary>
		Custom
	}

	/// <summary>
	/// Indicates the property which is the primary key.
	/// </summary>
	/// <example><code>
	/// public class Blog : ActiveRecordBase
	/// {
	///		...
	///		
	///		[PrimaryKey(PrimaryKeyType.Native)]
	///		public int Id
	///		{
	///			get { return _id; }
	///			set { _id = value; }
	///		}
	/// </code></example>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple=false), Serializable]
	public class PrimaryKeyAttribute : WithAccessAttribute
	{
		private static readonly PrimaryKeyType defaultPrimaryKeyType = PrimaryKeyType.Native;
		private PrimaryKeyType? generator;

		/// <summary>
		/// Initializes a new instance of the <see cref="PrimaryKeyAttribute"/> class.
		/// </summary>
		public PrimaryKeyAttribute()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PrimaryKeyAttribute"/> class.
		/// </summary>
		/// <param name="customGenerator">A custom identifier 
		/// generator (that implements <see cref="NHibernate.Id.IIdentifierGenerator"/>).</param>
		public PrimaryKeyAttribute(Type customGenerator) : this(PrimaryKeyType.Custom)
		{
			CustomGenerator = customGenerator;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PrimaryKeyAttribute"/> class.
		/// </summary>
		/// <param name="generator">The generator.</param>
		public PrimaryKeyAttribute(PrimaryKeyType generator)
		{
			this.generator = generator;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PrimaryKeyAttribute"/> class.
		/// </summary>
		/// <param name="generator">The generator.</param>
		/// <param name="column">The PK column.</param>
		public PrimaryKeyAttribute(PrimaryKeyType generator, String column) : this(generator)
		{
			Column = column;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PrimaryKeyAttribute"/> class.
		/// </summary>
		/// <param name="column">The PK column.</param>
		public PrimaryKeyAttribute(string column)
		{
			Column = column;
		}

		/// <summary>
		/// Gets or sets the generator.
		/// </summary>
		/// <value>The generator.</value>
		public PrimaryKeyType Generator
		{
			get { return generator ?? defaultPrimaryKeyType; }
			set { generator = value; }
		}

		/// <summary>
		/// Gets or sets the column name
		/// </summary>
		/// <value>The column.</value>
		public string Column { get; set; }

		/// <summary>
		/// Gets or sets the unsaved value.
		/// </summary>
		/// <value>The unsaved value.</value>
		public string UnsavedValue { get; set; }

		/// <summary>
		/// Gets or sets the name of the sequence.
		/// </summary>
		/// <value>The name of the sequence.</value>
		public string SequenceName { get; set; }

		/// <summary>
		/// Gets or sets the type of the column.
		/// </summary>
		/// <value>The type of the column.</value>
		public string ColumnType { get; set; }

		/// <summary>
		/// Gets or sets the length of values in the column
		/// </summary>
		/// <value>The length.</value>
		public int Length { get; set; }

		/// <summary>
		/// Gets or sets the custom generator. 
		/// The generator must implement <see cref="NHibernate.Id.IIdentifierGenerator"/>
		/// </summary>
		/// <value>The custom generator type.</value>
		public Type CustomGenerator { get; set; }

		/// <summary>
		/// Comma separated value of parameters to the generator
		/// </summary>
		public string Params { get; set; }

		/// <summary>
		/// Set to <c>true</c> if this primary key overrides a primary key in a base class
		/// </summary>
		public bool IsOverride { get; set; }

		internal bool TypeSpecified
		{
			get { return generator.HasValue; }
		}

	}
}
