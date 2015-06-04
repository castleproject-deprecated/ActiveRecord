# Type Hierarchy

In an object oriented world classes can be extended and as you descend down the hierarchy they become more specialized. For example, a Person has attributes shared by persons. A Customer class extends Person with specialized attributes that only customers have.

It is also common to share behavior by using a common base class. For example, articles and blog posts can be distinct entities in a blog application. However, both receive comments and trackbacks. It would be a good idea if they extend from a base class that offers the "reviewable" behavior.

In this section we present you with approaches to implement type hierarchies using Castle ActiveRecord.

## Type hierarchy mapping patterns

There are three common patterns for mapping type hierarchies to database tables. Martin Fowler uses the following names for these patterns:

### Single Table Inheritance

Single Table Inheritance from the database perspective uses a single table with a discriminator column to determine which type each row contains. The table must contain columns for all the attributes required by any subclasses.

### Class Table Inheritance

Class Table Inheritance involves using different tables for each class where the "base" table defines the primary key, and the others "inherit" it.

### Concrete Table Inheritance

Concrete Table Inheritance is a third way to map a class hierarchy, each concrete class has it's own database table.

## Single Table Inheritance - Using a discriminator

First of all consider the following table script:

```sql
CREATE TABLE [companies] (
    [id] [int] IDENTITY (1, 1) NOT NULL ,
    [client_of] [int] NULL ,
    [name] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
    [type] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
) ON [PRIMARY]

CREATE TABLE [people] (
    [id] [int] IDENTITY (1, 1) NOT NULL ,
    [name] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
) ON [PRIMARY]

CREATE TABLE [people_companies] (
    [person_id] [int] NOT NULL ,
    [company_id] [int] NOT NULL
) ON [PRIMARY]
```

We want to represent three entities with the same Companies table (`Company`, `Firm`, `Client`). In order to achieve this, we use a discriminator column - in this case the column type.

```csharp
using Castle.ActiveRecord;

[ActiveRecord("companies",
  DiscriminatorColumn="type",
  DiscriminatorType="String",
  DiscriminatorValue="company")]
public class Company : ActiveRecordBase
{
    private int id;
    private String name;
    private IList _people;

    public Company()
    {
    }

    public Company(string name)
    {
        this.name = name;
    }

    [PrimaryKey]
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

[ActiveRecord(DiscriminatorValue="firm")]
public class Firm : Company
{
    private IList clients;

    public Firm()
    {
    }

    public Firm(string name) : base(name)
    {
    }

    [HasMany(typeof(Client), ColumnKey="client_of")]
    public IList Clients
    {
        get { return clients; }
        set { clients = value; }
    }
}

[ActiveRecord(DiscriminatorValue="client")]
public class Client : Company
{
    private Firm firm;

    public Client()
    {
    }

    public Client(string name, Firm firm) : base(name)
    {
        this.firm = firm;
    }

    [BelongsTo("client_of")]
    public Firm Firm
    {
        get { return firm; }
        set { firm = value; }
    }
}
```

Each class can have its own `FindAll`, `DeleteAll`, only affecting its subset of records on the same table.

:information_source: If you want to expose the discriminator column in your ActiveRecord type then you must turn off inserts and updates for that column. In other words use `Property(Insert=false, Update=false)`. This is required as otherwise NHibernate will control it, not your code.

## Class Table Inheritance - Using joined subclasses

With this approach we would have a specialized table for each subclass. Consider the following table script:

```sql
CREATE TABLE [Entity]
(
    [id] [int] IDENTITY (1, 1) NOT NULL ,
    [name] [varchar] (50) NULL ,
    [type] [varchar] (10) NULL
) ON [PRIMARY]

CREATE TABLE [EntityCompany]
(
    [comp_id] [int] NOT NULL ,
    [company_type] [tinyint] NOT NULL
) ON [PRIMARY]

CREATE TABLE [EntityPerson]
(
    [person_id] [int] NOT NULL ,
    [email] [varchar] (50) NULL ,
    [phone] [varchar] (12) NULL
) ON [PRIMARY]
```

The `Entity` table holds the primary key that is auto generated. Both `EntityCompany` and `EntityPerson` have primary keys as well. But they are foreign keys to the primary key on the Entity table.

The base class is no different from ordinary ActiveRecord types. The only difference is the introduction of the `JoinedBaseAttribute` used to mark the base class in a joined base approach.

```csharp
using Castle.ActiveRecord;

[ActiveRecord("entity"), JoinedBase]
public class Entity : ActiveRecordBase
{
    private int id;
    private string name;
    private string type;

    public Entity()
    {
    }

    [PrimaryKey]
    private int Id
    {
        get { return id; }
        set { id = value; }
    }

    [Property]
    public string Name
    {
        get { return name; }
        set { name = value; }
    }

    [Property]
    public string Type
    {
        get { return type; }
        set { type = value; }
    }

    public static void DeleteAll()
    {
        DeleteAll(typeof(Entity));
    }

    public static Entity[] FindAll()
    {
        return (Entity[]) FindAll(typeof(Entity));
    }

    public static Entity Find(int id)
    {
        return (Entity) FindByPrimaryKey(typeof(Entity), id);
    }
}
```

The subclasses extend `Entity` and use the attribute `JoinedKeyAttribute` to decorate the shared key. This is required.

```csharp
using Castle.ActiveRecord;

[ActiveRecord("entitycompany")]
public class CompanyEntity : Entity
{
    private byte company_type;
    private int comp_id;

    [JoinedKey("comp_id")]
    public int CompId
    {
        get { return comp_id; }
        set { comp_id = value; }
    }

    [Property("company_type")]
    public byte CompanyType
    {
        get { return company_type; }
        set { company_type = value; }
    }

    public new static void DeleteAll()
    {
        DeleteAll(typeof(CompanyEntity));
    }

    public new static CompanyEntity[] FindAll()
    {
        return (CompanyEntity[]) FindAll(typeof(CompanyEntity));
    }

    public new static CompanyEntity Find(int id)
    {
        return (CompanyEntity) FindByPrimaryKey(typeof(CompanyEntity), id);
    }
}

[ActiveRecord("entityperson")]
public class PersonEntity : Entity
{
    private int person_id;

    [JoinedKey]
    public int Person_Id
    {
        get { return person_id; }
        set { person_id = value; }
    }

    public new static void DeleteAll()
    {
        DeleteAll(typeof(PersonEntity));
    }

    public new static PersonEntity[] FindAll()
    {
        return (PersonEntity[]) FindAll(typeof(PersonEntity));
    }

    public new static PersonEntity Find(int id)
    {
        return (PersonEntity) FindByPrimaryKey(typeof(PersonEntity), id);
    }
}
```

Please refer to the Reference Manual's Attributes article for further information.

## Concrete Table Inheritance - Using

Concrete table inheritance is supported by ActiveRecord without using any special techniques. If you define a type hierarchy you can place a `ActiveRecordAttribute` on each subclass which requires its own table.