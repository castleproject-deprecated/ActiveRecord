# Unit Testing

Testing plays the most important role on software development. With ActiveRecord we usually keep a separated database for unit testing, so all data can be deleted mercilessly.

We suggest that you use a base class for testing your object model. This greatly simplifies the task. However you have a few decisions to make:

* Do you want to manage the schema yourself?
* Do you want to let ActiveRecord/NHibernate create the schema for you on the test database?

If you want to manage the schema yourself, you must delete all data before executing each test, something like the following will be enough:

```csharp
protected virtual void PrepareSchema()
{
    // Remember to do it in a descendent dependency order

    Office.DeleteAll();
    User.DeleteAll();
}
```

Note that you have to implement the `DeleteAll` method on your classes. Something like:

```csharp
using Castle.ActiveRecord;

[ActiveRecord]
public class Office : ActiveRecordBase
{
    // definition omitted

    public static void DeleteAll()
    {
        ActiveRecordBase.DeleteAll(typeof(Office));
    }
}
```

On the other hand, if you want to let ActiveRecord create the schema, use the following:

```csharp
protected virtual void PrepareSchema()
{
    ActiveRecordStarter.CreateSchema();
}

protected virtual void DropSchema()
{
    ActiveRecordStarter.DropSchema();
}
```

The code below is a suggestion of an abstract class to simplify unit testing:

:warning: **Warning:** Don't forget to call the base method when using additional SetUp attributes in your test classes.

```csharp
using NUnit.Framework;

using Castle.ActiveRecord;
using Castle.ActiveRecord.Framework;
using Castle.ActiveRecord.Framework.Config;

public abstract class AbstractModelTestCase
{
    protected SessionScope scope;

    [TestFixtureSetUp]
    public virtual void FixtureInit()
    {
        InitFramework();
    }

    [SetUp]
    public virtual void Init()
    {
        PrepareSchema();

        CreateScope();
    }

    [TearDown]
    public virtual void Terminate()
    {
        DisposeScope();

        DropSchema();
    }

    [TestFixtureTearDown]
    public virtual void TerminateAll()
    {
    }

    protected void FlushAndRecreateScope()
    {
        DisposeScope();
        CreateScope();
    }

    protected void CreateScope()
    {
        scope = new SessionScope();
    }

    protected void DisposeScope()
    {
        scope.Dispose();
    }

    protected virtual void PrepareSchema()
    {
        // If you want to delete everything from the model.
        // Remember to do it in a descendent dependency order

        // Office.DeleteAll();
        // User.DeleteAll();

        // Another approach is to always recreate the schema
        // (please use a separate test database if you want to do that)

        ActiveRecordStarter.CreateSchema();
    }

    protected virtual void DropSchema()
    {
        ActiveRecordStarter.DropSchema();
    }

    protected virtual void InitFramework()
    {
        IConfigurationSource source = ActiveRecordSectionHandler.Instance;

        ActiveRecordStarter.Initialize(source);

        // Remember to add the types, for example
        // ActiveRecordStarter.Initialize( source, typeof(Blog), typeof(Post) );
    }
}
```

A test case using this base class would be similar to the following:

```csharp
using NUnit.Framework;

[TestFixture]
public class OfficeTestCase : AbstractModelTestCase
{
    [Test]
    public void Creation()
    {
        Office myoffice = new Office("The Office");
        myoffice.Create();

        FlushAndRecreateScope(); // Persist the changes as we're using scopes

        Office[] offices = Office.FindAll();
        Assert.AreEqual(1, offices.Length);
        Assert.AreEqual("The Office", offices[0].Name);
    }
}
```