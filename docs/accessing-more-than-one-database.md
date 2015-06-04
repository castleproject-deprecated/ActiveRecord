# Accessing more than one database

You can use more than one database with ActiveRecord. In order to do so you must create base classes that define, based on the hierarchy, which database is being used. Those are called *Root types*. If you use just one database, the *root type* is `ActiveRecordBase`.

## Adding a different database

Let's analyze the steps involved in getting ActiveRecord to work with more than one database.

### First: Create your root type

You must create an abstract class. It is recommended that this class extends ActiveRecordBase, because all *ActiveRecord types* bound to the second database must use it as the base class. This class can be empty.

However, it is not necessary to extend ActiveRecordBase. When ActiveRecord is used without base types (using ActiveRecordMediator), there is no gain from extending ActiveRecordBase, and inheriting from it can be safely omitted.

```csharp
using Castle.ActiveRecord

public abstract class LogisticDatabase : ActiveRecordBase
{
}
```

The class must not create a table by its own. It cannot be used as direct base class for single table or class table inheritance as described under [Implementing Type hierarchies](type-hierarchy.md).

### Second: configure the second database

On the existing configuration, you must use add another config set bound to the abstract class you have just created. For more information on it, see [XML Configuration Reference](xml-configuration-reference.md).