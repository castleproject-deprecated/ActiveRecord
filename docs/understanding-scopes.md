# Understanding Scopes

Any database related operation on ActiveRecord ultimately delegates the task to NHibernate. Basically, all NHibernate operations demands an ISession instance.

When ActiveRecord is about to delegate something to NHibernate it checks if there is a ISession instance available and if not, one is created and disposed as soon as the operation is completed.

A SessionScope is a way to encapsulate and extend the life of a NHibernate's ISession instance. Once one is active, when ActiveRecord checks for an ISession instance, the scope is found and from that point on it is in charge of providing the session.

It is absolutely necessary that you know the basics of what is the purpose of the NHibernate ISession, the Flush operation and supported Flush behaviors. Please take a moment to read the NHibernate documentation.

If you're familiar with the Unit of Work pattern, then consider a ISession a unit of work interaction with the database. NHibernate, however, is smart enough to flush changes whenever it decides it needs to. For instance, before sending a query to the database.

That might lead to unexpected results. That's why it's important to understand how things work and then design your solution appropriately.

## Understanding the internals

The `ISessionFactoryHolder` interface implementation holds and manages one or more session factories (one per database configured). Any ActiveRecord operation invokes `CreateSession` and `ReleaseSession`, for example:

```csharp
protected internal static Array FindAll(Type targetType, Order[] orders, params ICriterion[] criterias)
{
    ISession session = holder.CreateSession(targetType);

    try
    {
        // implementation omitted for clarity
    }
    catch(ValidationException)
    {
        throw;
    }
    catch(Exception ex)
    {
        throw new ActiveRecordException("Could not perform FindAll for " + targetType.Name, ex);
    }
    finally
    {
        holder.ReleaseSession(session);
    }
}
```

The holder implementation always checks for a registered scope:

```csharp
[MethodImpl(MethodImplOptions.Synchronized)]
public ISession CreateSession(Type type)
{
    if (threadScopeInfo.HasInitializedScope)
    {
        return CreateScopeSession(type);
    }

    ISessionFactory sessionFactory = GetSessionFactory(type);

    ISession session = OpenSession(sessionFactory);

    System.Diagnostics.Debug.Assert( session != null );

    return session;
}
```

## ISessionScope

The `ISessionScope` interface defines the contract for possible scope implementations. All scopes must have a fixed type that defines its semantic.

We have three built-in supported scopes:

* `SessionScope`: Keeps an `ISession` instance alive, thus maximizing the performance by providing a first level cache, batching operations and allowing lazy load to work
* `TransactionMode`: Defines the scope of a transaction. Can be nested within a `SessionScope` and even between multiple transactions.
* `DifferentDatabaseScope`: replaces the connection used by NHibernate with one provided by your code, thus forcing the underlying operations to happen in a different database

## Understanding SessionScope

The `SessionScope` should always be used on real apps built using ActiveRecord. It allows lazy load relations to work, it batches operations and provides a first level cache.

### First level cache

Consider the following code:

```
TODO
```

### Batching changes

Consider the following code:

```
TODO
```

### Possible unexpected behavior

Consider the following code:

```
TODO
```

## Read only Scopes

A NHibernate `ISession` manages the changes made to entities and `Flush` them in some situations. If your process performs lots of reads, and is not supposed to write, or you want to decide when a write should be performed, then you should create a `SessionScope` that does not perform `Flush`.

```csharp
using(new SessionScope(FlushAction.Never))
{
    // lots of operations db related here
}
```

For these cases, performance will be greatly improved. However it is up to you to manage the `Flush`:

```csharp
using(new SessionScope(FlushAction.Never))
{
    // lots of operations

    // Everything is OK, flush the changes
    SessionScope.Current.Flush();
}
```

At the same time, NHibernate is keeping tracks of changes to objects within the scope. If there are too many objects and too many changes to keep track, then performance will slowly downgrade. So a flushing now and then will be required.
