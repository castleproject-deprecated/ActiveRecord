# Using Version and Timestamp

NHibernate's Version and Timestamp feature can be used on Castle ActiveRecord as well. Those are used to detect changes from other processes/requests, avoiding a last-win situation.

When saving, NHibernate will make sure that the update will only succeed if the version/timestamp in the database match the version/timestamp on the object. If they are different, the changes will not be saved, and a `StaleObjectException` will be thrown.

## Using Version

Version can be of the following types: `Int64`, `Int32`, `Int16`, `Ticks`, `Timestamp`, or `TimeSpan`.

```csharp
[Version("customer_version")]
public Int32 Version
{
    get { return version; }
    set { version = value; }
}
```

## Using Timestamp

Note that using the `Timestamp` attribute is equal to using the `Version` attribute with `Type="timestamp"`. Because of timestamp precision issues, `Version` is consider safer to use than `Timestamp`

```csharp
[Timestamp("customer_timestamp")]
public Int32 Timestamp
{
    get { return ts; }
    set { ts = value; }
}
```