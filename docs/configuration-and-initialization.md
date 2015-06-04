# Configuration and Initialization

Before using an ActiveRecord class in runtime you must properly initialize the framework. This must happen only once for the entire application lifetime.

In order to initialize the framework, you must supply some obligatory information.

* What kind of database you are using
* How to connect to it

Optionally you can turn debug on, turn on caching and make fine tune adjustments. The sections below explain how to start the framework and give it all information it needs to initialize itself properly.

## Initializing

Before using ActiveRecord your application must invoke the method `Initialize` on `ActiveRecordStarter`. This method has a few overloads. Ultimately it needs an implementation of `IConfigurationSource` and a set of ActiveRecord types to inspect. It basically configures NHibernate, inspects the types for semantic errors and constructs the mapping for each of them.

In this section we will postpone the construction or obtention of an `IConfigurationSource` and focus the attention on the overloads.

### Initialize(IConfigurationSource source, params Type[] types)

This overload allow you to specify an IConfigurationSource and an array of ActiveRecord types. For example:

```csharp
using Castle.ActiveRecord;

IConfigurationSource config = ... ;

ActiveRecordStarter.Initialize(config, typeof(Blog), typeof(Post));
```

:information_source: When you use this overload you must remember that once you add one or more ActiveRecord types you must include the types on the call, otherwise the new types will not work.

### Initialize(Assembly assembly, IConfigurationSource source)

This overload allow you to specify an `IConfigurationSource` and an `Assembly` instance. ActiveRecord then iterates over all types on the Assembly and initializes the types it identifies as ActiveRecord types. The implementation checks whether the type uses the `ActiveRecordAttribute`.

Using this overload saves you from updating the call everytime a new type is included to your domain model.

Example:

```csharp
using Castle.ActiveRecord;

IConfigurationSource config = ... ;

Assembly asm = Assembly.Load("Company.Project.Models")

ActiveRecordStarter.Initialize(asm, config);
```

:warning: **Warning:** Try to create an `Assembly` exclusively for ActiveRecord types if you can. This overload will inspect all public types. If there are thousands of types, this can take a considerable amount of time.

### Initialize(Assembly[] assemblies, IConfigurationSource source)

Just like the overload above, this overload allow you to specify an `IConfigurationSource` and an array of `Assembly` instances. ActiveRecord then iterates over all types on the `Assembly` array and initializes the types it identifies as ActiveRecord types. The implementation checks whether the type uses the ActiveRecordAttribute.

This overload is useful when you have reusable ActiveRecord classes and a project uses a combination of reusable classes and their own types.

Example:

```csharp
using Castle.ActiveRecord;

IConfigurationSource config = ... ;

Assembly asm1 = Assembly.Load("Company.Common.Models")
Assembly asm2 = Assembly.Load("Company.Project.Models")

ActiveRecordStarter.Initialize(new Assembly[] { asm1, asm2 }, config);
```

### Initialize()

This overload uses an assumption to initialize the framework. It assumes that:

* The configuration can be obtained from the configuration associated with the AppDomain (more on that on the next section).
* All ActiveRecord types can be found on the executing assembly.

As much as this is the simpler overload, it is also the one which imposes more restrictions. As a general rule, do not use it.

```csharp
using Castle.ActiveRecord;

ActiveRecordStarter.Initialize();
```

## Configuring

Configuring Castle ActiveRecord is necessary as it will never be able to guess the database you are using or the connection string. When you configure it, you end up configuring the NHibernate instance ActiveRecord uses, and some other configuration is specific to ActiveRecord.

Basically you need to tell:

* The driver
* The dialect
* The connection provider
* The connection string

These entries are not optional and must be informed. Optionally you can configure caching, schema name, query substitutions. You can find more information about those on NHibernate documentation.

Castle ActiveRecord abstracts the configuration from its source using the interface IConfigurationSource. You can construct an implementation yourself if you want to, or you can use one of the three implementations provided, which are listed below.

### InPlaceConfigurationSource

The `InPlaceConfigurationSource` allows you to hardcode the information (or at least part of it) required to configure.

```csharp
using System.Collections;
using Castle.ActiveRecord.Framework.Config;

IDictionary<string, string> properties = new Dictionary<string, string>();

properties.Add("connection.driver_class", "NHibernate.Driver.SqlClientDriver");
properties.Add("dialect", "NHibernate.Dialect.MsSql2000Dialect");
properties.Add("connection.provider", "NHibernate.Connection.DriverConnectionProvider");
properties.Add("connection.connection_string", "Data Source=.;Initial Catalog=test;Integrated Security=SSPI");

InPlaceConfigurationSource source = new InPlaceConfigurationSource();

source.Add(typeof(ActiveRecordBase), properties);

ActiveRecordStarter.Initialize(source, typeof(Blog), typeof(Post));
```

`InPlaceConfigurationSource` is only recommended in two situations. You are getting acquanted with ActiveRecord or your system has a custom configuration support and you want to integrate wit it.

### XmlConfigurationSource

You can also read the configuration from a Xml file. The schema is documented in [XML Configuration Reference](xml-configuration-reference.md).

```csharp
using Castle.ActiveRecord.Framework.Config;

XmlConfigurationSource config = new XmlConfigurationSource("ARConfig.xml");

ActiveRecordStarter.Initialize(config, typeof(Blog), typeof(Post));
```

:warning: **Warning:** If a non-absolute filename is use like in the example above, the file will be searched based on the working directory. Usually the working directory is the bin folder.

The `XmlConfigurationSource` is the better approach if you want to externalize the configuration in a file with an exclusive purpose of holding the ActiveRecord configuration.

### ActiveRecordSectionHandler

With the `ActiveRecordSectionHandler` you can use the configuration file associated with the `AppDomain` (for example the web.config). The schema is documented in [XML Configuration Reference](xml-configuration-reference.md).

```csharp
using Castle.ActiveRecord.Framework.Config;

IConfiguration config = ActiveRecordSectionHandler.Instance;

ActiveRecordStarter.Initialize(config, typeof(Blog), typeof(Post));
```

:warning: **Warning:** If a section is not found an exception will be thrown.