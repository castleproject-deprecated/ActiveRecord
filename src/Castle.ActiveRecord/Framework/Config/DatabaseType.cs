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

namespace Castle.ActiveRecord.Framework.Config
{
	/// <summary>
	/// Enum for database types support for configuration construction. 
	/// Not to be confused by databases supported by ActiveRecord
	/// </summary>
	public enum DatabaseType
	{
		/// <summary>
		/// Microsoft SQL Server 2000
		/// </summary>
		MsSqlServer2000,
		/// <summary>
		/// Microsoft SQL Server 2005
		/// </summary>
		MsSqlServer2005,
		/// <summary>
		/// Microsoft SQL Server 2008
		/// </summary>
		MsSqlServer2008,
		/// <summary>
		/// SQLite
		/// </summary>
		SQLite,
		/// <summary>
		/// MySQL 3 or 4
		/// </summary>
		MySql,
		/// <summary>
		/// MySQL 5
		/// </summary>
		MySql5,
		/// <summary>
		/// Firebird
		/// </summary>
		Firebird,
		/// <summary>
		/// PostgreSQL
		/// </summary>
		PostgreSQL,
		/// <summary>
		/// PostgreSQL 8.1
		/// </summary>
		PostgreSQL81,
		/// <summary>
		/// PostgreSQL 8.2
		/// </summary>
		PostgreSQL82,
		/// <summary>
		/// Microsoft SQL Server 2005 Compact Edition
		/// </summary>
		MsSqlCe,
		/// <summary>
		/// Oracle 
		/// </summary>
		Oracle8i,
		/// <summary>
		/// Oracle 9
		/// </summary>
		Oracle9i,
		/// <summary>
		/// Oracle 10g
		/// </summary>
		Oracle10g
	}
}
