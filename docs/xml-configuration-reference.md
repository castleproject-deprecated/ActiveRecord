# XML Configuration Reference

The following is the definition of the expected xml schema. Differences regarding the how it must appear in a standalone xml file or in a configuration associated with an AppDomain (web.config for instance) are also explained below.

:information_source: If you are using Castle ActiveRecord in a web application, **you must add** the attribute `isWeb="true"`. This forces ActiveRecord to use a different strategy to hold instances. This is necessary as different threads may serve the same request events in a web application.

```xml
<configuration>

    <activerecord
        isWeb="true|false"
        isDebug="true|false"
        pluralizeTableNames="true|false"
        threadinfotype="custom thread info implementation"
        sessionfactoryholdertype="custom session holder implementation"
        namingstrategytype="custom namingstrategy implementation">

        <config
            database="MsSqlServer2000|MsSqlServer2005|MsSqlServer2008|SQLite|MySql|MySql5|Firebird|PostgreSQL|PostgreSQL81|PostgreSQL82|MsSqlCe|Oracle8i|Oracle9i|Oracle10g"
            connectionStringName="name of connection string in config">
            <!-- Any legal NHibernate parameter you want to specify or override its default value -->
        </config>

        <config type="Full Type name to Abstract Class that defines boundaries for different database">
            <add key="connection.driver_class"           value="NHibernate Driver" />
            <add key="dialect"                           value="NHibernate Dialect" />
            <add key="connection.provider"               value="NHibernate Connection Provider" />
            <add key="proxyfactory.factory_class"        value="NHibernate.ByteCode.Castle.ProxyFactoryFactory,NHibernate.ByteCode.Castle" />
            <!-- Use only one of the two attributes below -->
            <add key="connection.connection_string"      value="connection string" />
            <add key="connection.connection_string_name" value="name of connection string in config" />
        </config>

    </activerecord>

</configuration>
```

The following table explains the attributes.

Attribute | Required | Description
--------- | -------- | -----------
isWeb | No | If ActiveRecord is running in a ASP.NET application, you must add this attribute with the value true
isDebug | No | If true forces ActiveRecord to write the mappings to files. Useful for debugging and to show NHibernate team when asking for help on their forum. The files are written to the bin folder.
pluralizeTableNames | No | If true, ActiveRecord will infer the table names as plural of the entity name. Defaults to false.
threadinfotype | No | Full qualified type name to a custom implementation of IThreadScopeInfo
sessionfactoryholdertype | No | Full qualified type name to a custom implementation of ISessionFactoryHolder
namingstrategytype | No | Full qualified type name to a custom implementation of INamingStrategy. This is a NHibernate interface
type (on config node) | No | Only required if more than one config node is present. Should be a fully qualified type name to an abstract class that extends ActiveRecordBase and defines the boundaries to a different database
database (on config node) | No* | Optional shortcut attribute that populates configuration with default values for chosed database. The defaults can be overriden. When used, 'connectionStringName' must also be specified. Alternatively a shorter version of this attribute - 'db' can be used.
connectionStringName (on config node) | No* | Name of connection string in config to use. When used, 'database' must also be specified. Alternatively a shorter version of this attribute - 'csn' can be used.

The NHibernate config elements are rather static and can be used from the examples below. The only element to pay attention to is the connection information. There are two possibilities two specify how to connect to the database:

* Adding a connection string using the key connection.connection_string
* Using a predefined named connection string by specifying connection.connection_string_name

Using the latter allows to use a connection string that is set in the connectionStrings-section of app.config or web.config. Those connection strings can be set by administrators using a GUI without knowing about ActiveRecord or NHibernate configuration. Example:

```xml
<configuration>
  <configSections>
      <section name="activerecord"
               type="Castle.ActiveRecord.Framework.Config.ActiveRecordSectionHandler, Castle.ActiveRecord" />
  </configSections>

  <connectionStrings>
      <add name="main" connectionString="Data Source=.;Initial Catalog=test;Integrated Security=SSPI"/>
  </connectionStrings>
  <activerecord>
      <config>
        <add key="connection.driver_class" value="NHibernate.Driver.SqlClientDriver" />
        <add key="dialect" value="NHibernate.Dialect.MsSql2005Dialect" />
        <add key="connection.provider" value="NHibernate.Connection.DriverConnectionProvider" />
        <add key="connection.connection_string_name" value="main" />
      </config>
  </activerecord>
</configuration>
```

## Configuration on standalone xml file

All that is required in a standalone configuration file is that the activerecord node be the root element node. For example:

```xml
<activerecord>

    <config>
      <add
        key="connection.driver_class"
        value="NHibernate.Driver.SqlClientDriver" />
      <add
        key="dialect"
        value="NHibernate.Dialect.MsSql2000Dialect" />
      <add
        key="connection.provider"
        value="NHibernate.Connection.DriverConnectionProvider" />
      <add
        key="connection.connection_string"
        value="Data Source=.;Initial Catalog=test;Integrated Security=SSPI" />
    </config>

</activerecord>
```

## AppDomain configuration

Every AppDomain has a configuration file associated with it. For web application the file will be web.config. For .net executables it will be the name of the executable file and the sufix .config, for example myapp.exe.config.

You can use these files to add a configuration to Castle ActiveRecord. Just make sure you declare a section for it under the sections node.

```xml
<configuration>

    <configSections>
        <section name="activerecord"
                 type="Castle.ActiveRecord.Framework.Config.ActiveRecordSectionHandler, Castle.ActiveRecord" />
    </configSections>

    <activerecord>

      <config>
        <add
            key="connection.driver_class"
            value="NHibernate.Driver.SqlClientDriver" />
        <add
            key="dialect"
            value="NHibernate.Dialect.MsSql2000Dialect" />
        <add
            key="connection.provider"
            value="NHibernate.Connection.DriverConnectionProvider" />
        <add
            key="connection.connection_string"
            value="Data Source=.;Initial Catalog=test;Integrated Security=SSPI" />
      </config>

    </activerecord>

</configuration>
```

## Examples per Database

The following sections illustrates some usage of the Xml configuration.

### Microsoft SQL Server 2000

```xml
<activerecord>

    <config>
      <add
        key="connection.driver_class"
        value="NHibernate.Driver.SqlClientDriver" />
      <add
        key="dialect"
        value="NHibernate.Dialect.MsSql2000Dialect" />
      <add
        key="connection.provider"
        value="NHibernate.Connection.DriverConnectionProvider" />
      <add
        key="connection.connection_string"
        value="Data Source=.;Initial Catalog=test;Integrated Security=SSPI" />
    </config>

</activerecord>
```

### Microsoft SQL Server 2005

```xml
<activerecord>

    <config>
      <add
        key="connection.driver_class"
        value="NHibernate.Driver.SqlClientDriver" />
      <add
        key="dialect"
        value="NHibernate.Dialect.MsSql2005Dialect" />
      <add
        key="connection.provider"
        value="NHibernate.Connection.DriverConnectionProvider" />
      <add
        key="connection.connection_string"
        value="Data Source=.;Initial Catalog=test;Integrated Security=SSPI" />
    </config>

</activerecord>
```

### Oracle

```xml
<activerecord>

    <config>
      <add
        key="connection.driver_class"
        value="NHibernate.Driver.OracleClientDriver" />
      <add
        key="dialect"
        value="NHibernate.Dialect.OracleDialect" />
      <add
        key="connection.provider"
        value="NHibernate.Connection.DriverConnectionProvider" />
      <add
        key="connection.connection_string"
        value="Data Source=dm;User ID=dm;Password=dm;" />
    </config>

</activerecord>
```

### MySQL

```xml
<activerecord>

    <config>
      <add
        key="connection.driver_class"
        value="NHibernate.Driver.MySqlDataDriver" />
      <add
        key="dialect"
        value="NHibernate.Dialect.MySQLDialect" />
      <add
        key="connection.provider"
        value="NHibernate.Connection.DriverConnectionProvider" />
      <add
        key="connection.connection_string"
        value="Database=test;Data Source=someip;User Id=blah;Password=blah" />
    </config>

</activerecord>
```

### Firebird

```xml
<activerecord>

    <config>
      <add
        key="connection.driver_class"
        value="NHibernate.Driver.FirebirdDriver" />
      <add
        key="dialect"
        value="NHibernate.Dialect.FirebirdDialect" />
      <add
        key="connection.provider"
        value="NHibernate.Connection.DriverConnectionProvider" />
      <add
        key="connection.connection_string"
        value="Server=localhost;Database=d:\db.fdb;User=name;password=masterkey;ServerType=1;Pooling=false" />
      <add
        key="query.substitutions"
        value="true 1, false 0" />
    </config>

</activerecord>
```

### PostgreSQL

```xml
<activerecord>

    <config>
      <add
        key="connection.driver_class"
        value="NHibernate.Driver.NpgsqlDriver" />
      <add
        key="dialect"
        value="NHibernate.Dialect.PostgreSQLDialect" />
      <add
        key="connection.provider"
        value="NHibernate.Connection.DriverConnectionProvider" />
      <add
        key="connection.connection_string"
        value="Server=localhost;initial catalog=nhibernate;User ID=nhibernate;Password=nhibernate;" />
    </config>

</activerecord>
```