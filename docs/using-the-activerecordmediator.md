# Using the ActiveRecordMediator (avoiding a base class)

Castle ActiveRecord offers a lot of functionality in persisting objects: It allows to use NHibernate without requiring to maintain XML mapping files, it manages sessions and its integration into MonoRail simplifies using web applications.

One constraint that was found to hinder using ActiveRecord in practice was the requirement to inherit from a certain base class. Given the fact that multiple inheritance is not possible in .Net, the need for a base class is very problematic.

And while the active record pattern is especially useful for creating data-driven applications, there are use cases where it is not adequate. Especially when adopting Domain Driven Design, there remains considerable debate whether ActiveRecord should be banned from the domain layer altogether. Although only removing the base class from ActiveRecord types does not make them persistance ignorant or deals with other problematic factors like the dominance of properties, it avoids attaching persistance behaviour to the classes.

Stripped of the persistance related methods, ActiveRecord types can be used as domain classes. This is still questionable, but a practical way to accomplish transition from a data-driven to a domain-driven approach.

Without persistance behaviour, ActiveRecord types can also be used as a DTOs (Data Transfer Objects) across the application. Methods like `Create()` or `Delete()` should not be present in DTOs since they distract the client code developer from using the applications services. A plain serializable object on the other hand can be filled with data and passed to the application layer which will use it to execute the required domain logic in the domain layer.

## Basic Operations

The basic operations are present in the `ActiveRecordMediator` class. This is a generic class that contains static methods providing the same functionality as `ActiveRecordBase` concerning basic operations discussed in lifecycle article.

The following example shows the usage of ActiveRecordMediator:

```csharp
// Create
Customer c = new Customer();
c.Name = "Mr. Johnson";
c.Address = String.Empty;
ActiveRecordMediator<Customer>.Save(c);

// Read/Update
Customer loaded = ActiveRecordMediator<Customer>.FindOne(Expression.Eq("Name", "Mr. Johnson"));
loaded.Address = "Middle of the Road";
ActiveRecordMediator<Customer>.Save(loaded);

// Delete
ActiveRecordMediator<Customer>.Delete(c);
```

All information about the lifecycle, sessions and transactions are still valid, only the calls to perform ActiveRecord operations has changed.

## Validation Support

Another consequence of not using a base class is missing validation support. The `ActiveRecordValidationBase` offers an `IsValid()` and ActiveRecord denies flushing changes if an entity inheriting from this base class is not valid.

Without using the base class, this functionality is void. Only database constraints are still effective. These constraints are defined within the Property or Field attribute and allow only basic validations.

However, Castle's Validation support is not gone, but it must be invoked explicitly instead.

The following example shows how this is done:

```csharp
// Entity
[ActiveRecord]
public class Customer
{
    private Guid id;

    [PrimaryKey(PrimaryKeyType.GuidComb)]
    public Guid Id
    {
        get { return id; }
        set { id = value; }
    }
    private string name;

    [Property(Unique=true,NotNull=true)]
    [ValidateNonEmpty]
    [ValidateIsUnique]
    public string Name
    {
        get { return name; }
        set { name = value; }
    }
    private string address;

    [Property(NotNull=true)]
    [ValidateNonEmpty]
    public string Address
    {
        get { return address; }
        set { address = value; }
    }
}

// using code
Customer c = new Customer();
c.Name = "Mr. Johnson";
c.Address = String.Empty;

IValidatorRunner runner = new ValidatorRunner(new CachedValidationRegistry());
if (runner.IsValid(c))
{
    ActiveRecordMediator<Customer>.Save(c);
}
else
{
    foreach (string msg in runner.GetErrorSummary(c).ErrorMessages)
    {
        Console.WriteLine(msg);
    }
}
```

## Providing Callbacks

The class `ActiveRecordHooksBase` which is base class of `ActiveRecordBase` provides a couple of methods to hook into ActiveRecord's lifecycle management. Classes that do not inherit from `ActiveRecordBase` do not offer these hooks.

In order to hook into the lifecycle, it is possible to use the strategies that NHibernate offers. Since NHibernate works only on plain classes, these methods are also suitable for ActiveRecord-types without a base class.

The two possibilities are:

* **Interceptors**: Interceptors implement the interface `IInterface`, which defines roughly the same hooks as `ActiveRecordHooksBase`. Interceptors must be registered with the session used.
* **Events**: The event system was introduced in NHibernate 2.0. It replaces interceptors and offers a more modular approach to hooking into the lifecycle. For all hooks events are fired from within NHibernate. Any event listener registered will be notified of the event and can act accordingly.

:information_source: Events have the benefit that the listeners can be registered in the configuration. This allows to add them transparently to the application. **ActiveRecord does not yet support the registration of event listeners in the configuration.**

The use of lifecycle callbacks, interceptors and events is discussed in a the article [Hooks and lifecycle](hooks-and-lifecycle.md).

## The ActiveRecordMediator as a Repository

In addition to the persistence methods available many classic ActiveRecord entity types contain own persistence logic, for example static `Find`-methods that return entity collections based on the types special characteristics.

When the original persistance logic ActiveRecord offers, is removed from the entity, there is no good reason to put additional persistance logic in it, even if domain-driven development is not an issue.

Such methods belong together in a single class. This type of class is often called a **Repository**. If a specialized repository implementation such as `IRepository` from **Rhino Commons** is not used, `ActiveRecordMediator` can be used to build custom repository types:

```csharp
public class CustomerRepo : ActiveRecordMediator<Customer>
{
    public static Customer[] FindByName(string name)
    {
        return FindAll(new ICriterion[]{Expression.Like("Name", name,MatchMode.Anywhere)});
    }
}
```

The repository can then be used exactly like the `ActiveRecordMediator` itself:

```csharp
// Create
Customer c = new Customer();
c.Name = "Mr. Johnson";
c.Address = String.Empty;
CustomerRepo.Save(c);

// Read/Update
Customer loaded = CustomerRepo.FindByName("Johnson")[0];
loaded.Address = "Middle of the Road";
CustomerRepo.Save(loaded);

// Delete
CustomerRepo.Delete(loaded);
```