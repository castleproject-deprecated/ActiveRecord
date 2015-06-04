# Enabling Lazy Load

Lazy load is a well known pattern where data is only obtained when it is really needed. If you do not have lazy load enabled and you have a large and complex object graph, when you load one object all dependencies will be loaded in a chain, issuing several SQL statements.

Lazy load can be enabled on a type or in a relation.

## Enabling lazy on an ActiveRecord type

If can enable lazy load for a type by using the `Lazy=true` on the `ActiveRecordAttribute`. For example:

```csharp
using Castle.ActiveRecord;

[ActiveRecord(Lazy=true)]
public class Customer : ActiveRecordBase
{
    // omitted for now
}
```

However, when you do this, NHibernate generates a dynamic proxy for you class. So whenever you load a `Customer` class you will get a `CustomerProxy` (the name is more complex than that).

The proxy is necessary so NHibernate can intercept how you are using the instance. Once you invoke a method, NHibernate can identify it and load the data for you. If you never use the object, it will not waste time loading a data you will never use.

You must be aware that all methods and properties on your class must now be declared as virtual. This is required as the proxy needs to inherit from your class and override the methods in order to make interception work.

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
```

:warning: **Warning:** NHibernate will throw an exception if it identifies an instance method not defined as virtual.

Lazy will only work if the session that loaded the proxy is kept alive. This means that you **must** enclose the code that use types with lazy enabled in a `SessionScope`. Otherwise you will get an lazy initialization failure exception.

```csharp
using(new SessionScope())
{
    Customer customer = Customer.Find(1);

    Console.WriteLine(customer.Name); // loads data
}
```

## Enabling lazy on a relation

All relation attributes (except `BelongsTo`) have a `Lazy` property that is `false` by default. You just need to enable Lazy along the same lines as described above.

You do not need to mark anything as virtual when enabling lazy for relations, though. However the same rules applies regarding `SessionScope`.

When lazy is enabled for a collection, NHibernate returns a lazy enabled collection. Once it is accessed, the items are loaded.