# Using Imports

If there is a collision of names among your entities, you may use `Import` so you do not need to use their full name on queries.

You can also use imports to query returns to classes.

```csharp
using Castle.Framework;

[ActiveRecord, Import(typeof(OrderSummary), "summary")]
public class Order : ActiveRecordBase
{
    // omitted for clarity
}

public class OrderSummary
{
    private float value;
    private int quantity;

    public float Value
    {
        get { return value; }
        set { this.value = value; }
    }

    public int Quantity
    {
        get { return quantity; }
        set { quantity = value; }
    }
}
```