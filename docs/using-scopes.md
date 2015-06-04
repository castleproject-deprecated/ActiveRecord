# Using Scopes

Scopes allow you to optimize a sequence of database related operations and to add some semantic to a block of code. A `SessionScope` is required if you want to use lazy initialized types or collections.

A scope is active when constructed, and performs its work upon disposal. Once created - and before being disposed - the scope is active and available to all ActiveRecord operations on the same thread it was created. That means that you do not need to pass scopes along with in your method calls.

## SessionScope

A `SessionScope` is used to batch operations and to keep the NHibernate session. You must dispose a `SessionScope` after using it, so the C# `using` idiom becomes handy to enforce that.

```csharp
using(new SessionScope())
{
    Blog blog = new Blog();
    blog.Author = "hammett";
    blog.Name = "some name";
    blog.Save();

    Post post = new Post(blog, "title", "post contents", "Castle");
    post.Save();
}
```

More on the `SessionScope` can be found at [Understanding Scopes|Understanding Scopes].

## TransactionScope

There is also an scope to enable transactions:

```csharp
TransactionScope transaction = new TransactionScope();

try
{
    Blog blog = new Blog();
    blog.Author = "hammett";
    blog.Name = "some name";
    blog.Save();

    Post post = new Post(blog, "title", "post contents", "Castle");
    post.Save();
}
catch(Exception)
{
    transaction.VoteRollBack();

    throw;
}
finally
{
    transaction.Dispose();
}
```

## Nested transactions

You can also have nested transactions which are particularly useful when you have a layer to mark transaction boundaries:

```csharp
using(TransactionScope root = new TransactionScope())
{
    Blog blog = new Blog();

    using(TransactionScope t1 = new TransactionScope(TransactionMode.Inherits))
    {
        blog.Author = "hammett";
        blog.Name = "some name";
        blog.Save();

        t1.VoteCommit();
    }

    using(TransactionScope t2 = new TransactionScope(TransactionMode.Inherits))
    {
        Post post = new Post(blog, "title", "post contents", "Castle");

        try
        {
            post.SaveWithException();
        }
        catch(Exception)
        {
            t2.VoteRollBack();
        }
    }
}
```

Note the `TransactionMode.Inherits`.