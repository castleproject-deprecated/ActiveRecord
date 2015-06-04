# Validation Support

You can have validatable properties on your ActiveRecord classes by using `ActiveRecordValidationBase` instead of `ActiveRecordBase`

By doing so, you can use the following methods:

* `IsValid`: will return true only if all validation test passes
* `ValidationErrorMessages`: returns a string array of descriptive error messages

## Example

The following class uses validations on two properties. If you attempt to save an invalid instance, ActiveRecord will throw an exception, so calling `IsValid` before saving it is a good idea.

```csharp
using Castle.Components.Validator;

[ActiveRecord]
public class Customer : ActiveRecordValidationBase
{
    private String contactName;
    private String phone;

    public Customer()
    {
    }

    [Property, ValidateNonEmpty]
    public string ContactName
    {
        get { return contactName; }
        set { contactName = value; }
    }

    [Property, ValidateNonEmpty]
    public string Phone
    {
        get { return phone; }
        set { phone = value; }
    }
}
```

For a list of implemented validators and how to implement your own see [Validators](validators.md).

Some validation logic is not suitable for declarative (attribute based) validators. If that is your case, you can override the method `IsValid`. Just make sure, if your validation test passes, you invoke the base implementation.

By default, a `ValidationException` exception will be thrown if the validation fails. If you want to change this behavior, override the method `OnNotValid`.

## ActiveRecordValidationBase<T>

You can also use ActiveRecordValidationBase so you can combine the niceness of declarative validations with the power of the generic implementation of ActiveRecordBase