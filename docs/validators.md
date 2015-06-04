# Validators

A validator performs a rule check and is used in combinator with the ActiveRecordValidationBase.

There are two classes involved to make the validation work. The first one is an attribute, that allow the programmer to declaratively decorate a property with the validation (or validations) it wants to perform on its value. The second one is the implementation of a validator. The attribute is responsible for instantiating the validator implementation.

When associating a validator with a property, you may optionally override the error message in the case the validation fails. For example:

Note: validation relies on `Castle.Components.Validator`.

```csharp
[ValidateEmail]
public String Email
{
    get { return email; }
    set { email = value; }
}
```

If the above validation fails the error message will be "Field Email doesn't seem like a valid e-mail". To customize the message, use:

```csharp
[ValidateEmail("Not a valid email")]
public String Email
{
    get { return email; }
    set { email = value; }
}
```

## The Available Validators

The following sections describe the existing validators.

### ValidateIsUnique

Checks that the property value does not exist in the database table.

### ValidateRegExp

Checks the value of the property against a regular expression that must be matched.

### ValidateEmail

Checks if the property value looks like a valid email. This is done using a regular expression.

### ValidateNonEmpty

Checks if the value is not null and not empty.

### ValidateConfirmation

Checks the value of the property has the same value of a different property. Useful for Password and Confirm password properties.

## Creating a custom validator

To create your own validator you must start with an attribute that extends from `AbstractValidationAttribute`. It must pass on an instance that implements `IValidator`.

By implementing the `IValidator` interface you will get access to the `PropertyInfo` the attribute is associated with. You must perform the check on the `Perform` method.