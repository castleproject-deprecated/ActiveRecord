# Simple Column Mapping

You can map ordinary columns to properties or to fields directly.

## Mapping columns to properties

The simplest form to map a property declared on your class to a column in the database table is to use `PropertyAttribute` without parameters. ActiveRecord will **assume** that the column name is the same as the property name. For example, given the following table script:

```sql
CREATE TABLE Entity (
    [id] [int] IDENTITY (1, 1) NOT NULL,
    [name] [varchar] (20) NULL
) ON [PRIMARY]
```

The mapping for this class would be (including the [Primary Key Mapping|primary key]):

```csharp
using Castle.ActiveRecord;

[ActiveRecord]
public class Entity : ActiveRecordBase<Entity>
{
    private int id;
    private string name;

    [PrimaryKey(PrimaryKeyType.Native)]
    private int Id
    {
        get { return id; }
        set { id = value; }
    }

    [Property]
    public String Name
    {
        get { return name; }
        set { name = value; }
    }
}
```

If the column had a different name, for example `EntityName`, you could use:

```csharp
private string name;

[Property("name")]
public String Name
{
    get { return name; }
    set { name = value; }
}
```

You do not need a setter for the primary key, but NHibernate needs to set the value somehow. You can then specify the `Access` for it, for example:

```csharp
private string name;

[Property(Access=PropertyAccess.FieldCamelcase)]
public String Name
{
    get { return name; }
}
```

If the property should be readonly, it is also possible to use a private setter. It is then not necessary to specify the `PropertyAccess`

```csharp
private string name;

[Property(Access=PropertyAccess.FieldCamelcase)]
public String Name
{
    get { return name; }
    private set { name = value; }
}
```

Please refer to the Reference Manual's Attributes article for further information.

## Mapping columns to fields

Although property binding seems more natural, fields can also be mapped using the `FieldAttribute`. The field can be of any visibility.

The usage is the same as `PropertyAttribute`.

```csharp
[Field]
private string name;
```

Please refer to the Reference Manual's Attributes article for further information.

## Large Objects

If your table have columns for large content (text or binary), you must specify the column type using the property `ColumnType`, which is present on both `PropertyAttribute` and `FieldAttribute`

Value to use | .NET type | Database type
------------ | --------- | -------------
StringClob | string | Text
BinaryBlob | byte[] | Binary
Serializable | any object that implements serializable | Binary

## Nullable Types

As you know, value types in .net cannot assume a null value. Often however, columns in the database like ints or `DateTime` are nullable.

Beginning with .Net Framework 2.0, support for nullable value types is available. ActiveRecord supports the nullable value types natively:

```csharp
using Castle.ActiveRecord;

[ActiveRecord]
public class Entity : ActiveRecordBase<Entity>
{
    private int id;
    private Int32? age;
    private DateTime? created;
    private Boolean? accepted;

    // omitted primary key

    [Property]
    public Int32? Age
    {
        get { return age; }
        set { age = value; }
    }

    [Property]
    public DateTime? CreatedAt
    {
        get { return created; }
        set { created = value; }
    }

    [Property]
    public Boolean? Accepted
    {
        get { return accepted; }
        set { accepted = value; }
    }
}
```