# ByteCode Generator and Scopeless Lazy Loading

ActiveRecord provides a custom ByteCode generator, based off of the Castle ByteCode packaged with NHibernate. The ActiveRecord ByteCode provides additional session management functionality not found in the standard implementation.

A ByteCode provides the object proxies used when lazy loading entities from the database. Please see [enabling lazy load](enabling-lazy-load.md) for additional information on how to use ActiveRecords lazy functionality.

ActiveRecord can lazy load three types of objects:

* ActiveRecord objects
* Properties
* Relationships

The ActiveRecord ByteCode will provide session management for lazy ActiveRecord objects. As a result, you do not need to provide a session scope when accessing these objects.

:warning: **Warning:** Session management of lazy properties and relationships is not currently supported.  These must still be accessed from within the original SessionScope.

For example, if you have the following classes defined:

```csharp
using Castle.ActiveRecord;

[ActiveRecord(Lazy=true)]
public class Customer : ActiveRecordBase
{
    private int id;
    private string name;

    [PrimaryKey]
    public virtual int Id
    {
        get { return id; }
        set { id = value; }
    }

    [Property]
    public virtual string Name
    {
        get { return name; }
        set { name = value; }
    }
}

[ActiveRecord]
public class Order : ActiveRecordBase
{
    private int id;
    private Customer owner

    [PrimaryKey]
    public virtual int Id
    {
        get { return id; }
        set { id = value; }
    }

    [BelongsTo(Lazy = FetchWhen.OnInvoke)]
    public Customer Owner
    {
        get { return owner; }
        set { owner = value; }
    }
}
```

Using the ActiveRecord ByteCode, you may access a Customer like this:

```csharp
Customer customer = Customer.Find(1);

Console.WriteLine(customer.Name);  // loads data
```

The standard Castle ByteCode would throw a LazyInitializationException when customer.Name was accessed. In other words, you don't need a underlying opened SessionScope when you replace the standard bytecode provider with the AR one.

Lazy objects can also be accessed through a BelongsTo relationship.

```csharp
Order order = Order.Find(1);

Console.WriteLine(order.Owner.Name);
```

The ActiveRecord ByteCode will also allow you to move lazy objects between sessions without error.

```csharp
Order order;
using(new SessionScope())
{
    order = Order.Find(1);
}

//...snip...

using(new SessionScope())
{
    Console.WriteLine(order.Owner.Name);
}
```

## Configuration

To use the ActiveRecord ByteCode you must reference it when configuring ActiveRecord.  Set the proxyfactory.factory_class attribute to "**Castle.ActiveRecord.ByteCode.ProxyFactoryFactory, Castle.ActiveRecord**".

For instance, you may have the following in your XML configuration file:

```xml
<activerecord>
    <config>
        <add key="connection.driver_class" value="NHibernate.Driver.SqlClientDriver" />
        <add key="dialect"                 value="NHibernate.Dialect.MSSql2005Dialect" />
        <add key="connection.provider"     value="NHibernate.Connection.DriverConnectionProvider" />
        <add key="connection.connection_string" value="Server=(local)\sqlexpress;initial catalog=test;Integrated Security=SSPI" />
        <add key="proxyfactory.factory_class" value="Castle.ActiveRecord.ByteCode.ProxyFactoryFactory, Castle.ActiveRecord" />
    </config>
</activerecord>
```