# Nested Data (NHibernate Components)

You can use different classes to map specific chunks of data. For example, a Customer table might have address related column. Instead of having the address on the Customer ActiveRecord class, you could map them to an Address class.

## Using a separated class

For the example stated above, we could have `Customer` ActiveRecord class declared as the example below:

```csharp
using Castle.ActiveRecord;

[ActiveRecord]
public class Customer : ActiveRecordBase
{
    private int id;

    private string street;
    private string city;
    private string state;
    private string zipcode;

    [PrimaryKey]
    private int Id
    {
        get { return id; }
        set { id = value; }
    }

    [Property]
    public string Street
    {
        get { return street; }
        set { street = value; }
    }

    [Property]
    public string City
    {
        get { return city; }
        set { city = value; }
    }

    [Property]
    public string State
    {
        get { return state; }
        set { state = value; }
    }

    [Property]
    public string ZipCode
    {
        get { return zipcode; }
        set { zipcode = value; }
    }
}
```

We can then extract the address related mapping to an `Address` class:

```csharp
using Castle.ActiveRecord;

public class Address
{
    private string street;
    private string city;
    private string state;
    private string zipcode;

    [Property]
    public string Street
    {
        get { return street; }
        set { street = value; }
    }

    [Property]
    public string City
    {
        get { return city; }
        set { city = value; }
    }

    [Property]
    public string State
    {
        get { return state; }
        set { state = value; }
    }

    [Property]
    public string ZipCode
    {
        get { return zipcode; }
        set { zipcode = value; }
    }
}
```

Now we can simplify the `Customer` class code:

```csharp
[ActiveRecord]
public class Customer : ActiveRecordBase
{
    private int id;
    private Address address;

    [PrimaryKey]
    private int Id
    {
        get { return id; }
        set { id = value; }
    }

    [Nested]
    public Address Address
    {
        get { return street; }
        set { street = value; }
    }

}
```

You can optionally specify a column prefix using the `ColumnPrefix` attribute.

Please refer to the Reference Manual's Attributes article for further information.