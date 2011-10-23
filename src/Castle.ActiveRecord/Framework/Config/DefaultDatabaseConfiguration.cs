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
	using System;
	using System.Collections.Generic;
	using System.Data;

    using Castle.ActiveRecord.ByteCode;

	using NHibernate.Connection;
	using NHibernate.Dialect;
	using NHibernate.Driver;

	/// <summary>
	/// Exposes default configuration properties for common databases defined in <see cref="DatabaseType"/> enum.
	/// </summary>
	public class DefaultDatabaseConfiguration
	{
		private const string cache_use_second_level_cache = "cache.use_second_level_cache";
		private const string connection_connection_release_mode = "connection.release_mode";
		private const string connection_driver_class = "connection.driver_class";
		private const string connection_isolation = "connection.isolation";
		private const string connection_provider = "connection.provider";
		private const string dialect = "dialect";
		private const string proxyfactory_factory_class = "proxyfactory.factory_class";
		private const string query_substitutions = "query.substitutions";

		private IDictionary<string, string> Configure<TDriver, TDialect>()
			where TDriver : IDriver
			where TDialect : Dialect
		{
			return Configure<TDriver, TDialect>(new Dictionary<string, string>());
		}

		/// <summary>
		/// Returns dictionary of common properties pre populated with default values for given <paramref name="databaseType"/>.
		/// </summary>
		/// <param name="databaseType">Database type for which we want default properties.</param>
		/// <returns></returns>
		public IDictionary<string, string> For(DatabaseType databaseType)
		{
			switch (databaseType)
			{
				case DatabaseType.MsSqlServer2000:
					return Configure<SqlClientDriver, MsSql2000Dialect>();
				case DatabaseType.MsSqlServer2005:
					return Configure<SqlClientDriver, MsSql2005Dialect>();
				case DatabaseType.MsSqlServer2008:
					return Configure<SqlClientDriver, MsSql2008Dialect>();
				case DatabaseType.SQLite:
					return Configure<SQLite20Driver, SQLiteDialect>(SQLite());
				case DatabaseType.MySql:
					return Configure<MySqlDataDriver, MySQLDialect>();
				case DatabaseType.MySql5:
					return Configure<MySqlDataDriver, MySQL5Dialect>();
				case DatabaseType.Firebird:
					return Configure<FirebirdDriver, FirebirdDialect>(Firebird());
				case DatabaseType.PostgreSQL:
					return Configure<NpgsqlDriver, PostgreSQLDialect>();
				case DatabaseType.PostgreSQL81:
					return Configure<NpgsqlDriver, PostgreSQL81Dialect>();
				case DatabaseType.PostgreSQL82:
					return Configure<NpgsqlDriver, PostgreSQL82Dialect>();
				case DatabaseType.MsSqlCe:
					return Configure<SqlServerCeDriver, MsSqlCeDialect>(MsSqlCe());
				// using oracle's own data driver since Microsoft
				// discontinued theirs, and that's what everyone
				// seems to be using anyway.
				case DatabaseType.Oracle8i:
					return Configure<OracleDataClientDriver, Oracle8iDialect>();
				case DatabaseType.Oracle9i:
					return Configure<OracleDataClientDriver, Oracle9iDialect>();
				case DatabaseType.Oracle10g:
					return Configure<OracleDataClientDriver, Oracle10gDialect>();
			}

			throw new ArgumentOutOfRangeException("databaseType", databaseType, "Unsupported database type");
		}

		private IDictionary<string, string> Configure<TDriver, TDialect>(Dictionary<string, string> configuration)
			where TDriver : IDriver
			where TDialect : Dialect
		{
			configuration[connection_provider] = LongName<DriverConnectionProvider>();
			configuration[cache_use_second_level_cache] = false.ToString();
			configuration[proxyfactory_factory_class] = LongName<ProxyFactoryFactory>();
			configuration[dialect] = LongName<TDialect>();
			configuration[connection_driver_class] = LongName<TDriver>();
			return configuration;
		}

		private string LongName<TType>()
		{
			return typeof(TType).AssemblyQualifiedName;
		}

		private Dictionary<string, string> SQLite()
		{
			// based on https://www.hibernate.org/361.html#A9
			return new Dictionary<string, string>
			{
				{ query_substitutions, "true=1;false=0" }
			};
		}

		private Dictionary<string, string> MsSqlCe()
		{
			// to workaround exception being thrown with default setting
			// when an implicit transaction is used with identity id
			// see: AR-ISSUE-273 for details
			return new Dictionary<string, string>
			{
				{ connection_connection_release_mode, "on_close" }
			};
		}

		private Dictionary<string, string> Firebird()
		{
			// based on https://www.hibernate.org/361.html#A5
			return new Dictionary<string, string>
			{
				{ query_substitutions, "true 1, false 0, yes 1, no 0" },
				{ connection_isolation, IsolationLevel.ReadCommitted.ToString() },
				{ "command_timeout", 444.ToString() },
				{ "use_outer_join", true.ToString() },
			};
		}
	}
}
