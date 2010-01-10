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

namespace Castle.ActiveRecord.Framework.Config
{
	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.IO;
	using System.Text;
	using System.Xml;

	/// <summary>
	/// Source of configuration based on Xml 
	/// source like files, streams or readers.
	/// </summary>
	public class XmlConfigurationSource : InPlaceConfigurationSource
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="XmlConfigurationSource"/> class.
		/// </summary>
		protected XmlConfigurationSource()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="XmlConfigurationSource"/> class.
		/// </summary>
		/// <param name="xmlFileName">Name of the XML file.</param>
		public XmlConfigurationSource(String xmlFileName)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(xmlFileName);
			PopulateSource(doc.DocumentElement);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="XmlConfigurationSource"/> class.
		/// </summary>
		/// <param name="stream">The stream.</param>
		public XmlConfigurationSource(Stream stream)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(stream);
			PopulateSource(doc.DocumentElement);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="XmlConfigurationSource"/> class.
		/// </summary>
		/// <param name="reader">The reader.</param>
		public XmlConfigurationSource(TextReader reader)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(reader);
			PopulateSource(doc.DocumentElement);
		}

		/// <summary>
		/// Populate this instance with values from the given XML node
		/// </summary>
		protected void PopulateSource(XmlNode section)
		{
			XmlAttribute isWebAtt = section.Attributes["isWeb"];
			XmlAttribute threadInfoAtt = section.Attributes["threadinfotype"];
			XmlAttribute isDebug = section.Attributes["isDebug"];
			XmlAttribute lazyByDefault = section.Attributes["default-lazy"];
			XmlAttribute pluralize = section.Attributes["pluralizeTableNames"];
			XmlAttribute verifyModelsAgainstDBSchemaAtt = section.Attributes["verifyModelsAgainstDBSchema"];
			XmlAttribute defaultFlushType = section.Attributes["flush"];
			XmlAttribute searchable = section.Attributes["searchable"];

			SetUpThreadInfoType(isWebAtt != null && "true" == isWebAtt.Value,
			                    threadInfoAtt != null ? threadInfoAtt.Value : String.Empty);

			XmlAttribute sessionfactoryholdertypeAtt =
				section.Attributes["sessionfactoryholdertype"];

			SetUpSessionFactoryHolderType(sessionfactoryholdertypeAtt != null
			                              	?
			                              sessionfactoryholdertypeAtt.Value
			                              	: String.Empty);

			XmlAttribute namingStrategyTypeAtt = section.Attributes["namingstrategytype"];

			SetUpNamingStrategyType(namingStrategyTypeAtt != null ? namingStrategyTypeAtt.Value : String.Empty);

			SetDebugFlag(ConvertBool(isDebug));

			SetIsLazyByDefault(ConvertBool(lazyByDefault));

			SetPluralizeTableNames(ConvertBool(pluralize));

			SetVerifyModelsAgainstDBSchema(verifyModelsAgainstDBSchemaAtt != null && verifyModelsAgainstDBSchemaAtt.Value == "true");

			if (defaultFlushType == null)
			{
				SetDefaultFlushType(DefaultFlushType.Classic);
			}
			else
			{
				SetDefaultFlushType(defaultFlushType.Value);
			}

			Searchable = ConvertBool(searchable);

			PopulateConfigNodes(section);
		}

		private void PopulateConfigNodes(XmlNode section)
		{
			const string Config_Node_Name = "config";

			foreach(XmlNode node in section.ChildNodes)
			{
				if (node.NodeType != XmlNodeType.Element) continue;

				if (!Config_Node_Name.Equals(node.Name))
				{
					String message = String.Format("Unexpected node. Expect '{0}' found '{1}'",
					                               Config_Node_Name, node.Name);

					throw new ConfigurationErrorsException(message);
				}

				Type targetType = typeof(ActiveRecordBase);

				IDictionary<string, string> defaults = null;
				if (node.Attributes.Count != 0)
				{
					XmlAttribute typeNameAtt = node.Attributes["type"];

					if (typeNameAtt != null)
					{
						String typeName = typeNameAtt.Value;

						targetType = Type.GetType(typeName, false, false);

						if (targetType == null)
						{
							String message = String.Format("Could not obtain type from name '{0}'", typeName);

							throw new ConfigurationErrorsException(message);
						}
					}

					var databaseName = node.Attributes["database"] ?? node.Attributes["db"];
					var connectionStringName = node.Attributes["connectionStringName"] ?? node.Attributes["csn"];
					if (databaseName != null && connectionStringName != null)
					{
						defaults = SetDefaults(databaseName.Value, connectionStringName.Value);
					}
					else if (databaseName != null || connectionStringName != null)
					{
						var message =
							String.Format(
								"Using short form of configuration requires both 'database' and 'connectionStringName' attributes to be specified.");
						throw new ConfigurationErrorsException(message);
					}
				}

				Add(targetType, BuildProperties(node, defaults));
			}
		}

		/// <summary>
		/// Sets the default configuration for database specifiend by <paramref name="name"/>.
		/// </summary>
		/// <param name="name">Name of the database type.</param>
		/// <param name="connectionStringName">name of the connection string specified in connectionStrings configuration section</param>
		/// <returns></returns>
		protected IDictionary<string, string> SetDefaults(string name, string connectionStringName)
		{
			var names = Enum.GetNames(typeof(DatabaseType));
			if (!Array.Exists(names, n => n.Equals(name, StringComparison.OrdinalIgnoreCase)))
			{
				var builder = new StringBuilder();
				builder.AppendFormat("Specified value ({0}) is not valid for 'database' attribute. Valid values are:", name);
				foreach (var value in Enum.GetValues(typeof(DatabaseType)))
				{
					builder.AppendFormat(" '{0}'", value.ToString());
				}

				builder.Append(".");
				throw new ConfigurationErrorsException(builder.ToString());
			}

			var type = (DatabaseType)Enum.Parse(typeof(DatabaseType), name, true);
			var defaults = new DefaultDatabaseConfiguration().For(type);
			defaults["connection.connection_string_name"] = connectionStringName;
			return defaults;
		}

		/// <summary>
		/// Builds the configuration properties.
		/// </summary>
		/// <param name="node">The node.</param>
		/// <param name="defaults"></param>
		/// <returns></returns>
		protected IDictionary<string, string> BuildProperties(XmlNode node, IDictionary<string, string> defaults)
		{
			var dict = defaults ?? new Dictionary<string, string>();

			foreach(XmlNode addNode in node.SelectNodes("add"))
			{
				XmlAttribute keyAtt = addNode.Attributes["key"];
				XmlAttribute valueAtt = addNode.Attributes["value"];

				if (keyAtt == null || valueAtt == null)
				{
					String message = String.Format("For each 'add' element you must specify 'key' and 'value' attributes");

					throw new ConfigurationErrorsException(message);
				}
				string value = valueAtt.Value;

				dict[keyAtt.Value] = value;
			}

			return dict;
		}

		private static bool ConvertBool(XmlNode boolAttrib)
		{
			return boolAttrib != null && "true".Equals(boolAttrib.Value, StringComparison.OrdinalIgnoreCase);
		}
	}
}
