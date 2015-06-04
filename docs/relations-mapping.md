# Relations Mapping

Relations is what we call the association among types or with themselves.

Many-to-one associations are described using `BelongsToAttribute`. One-to-many uses `HasManyAttribute`. Many-to-many uses `HasAndBelongsToManyAttribute`.

## BelongsTo

A many-to-one relation can be mapped using the `BelongsToAttribute`.

Consider the following table script:

```sql
CREATE TABLE Blogs
(
    [id] [int] IDENTITY (1, 1) NOT NULL,
    [name] [varchar] (50) NULL
) ON [PRIMARY]

CREATE TABLE Posts
(
    [id] [int] IDENTITY (1, 1) NOT NULL,
    [title] [varchar] (50) NULL,
    [contents] [text] NULL,
    [blogid] [int] NULL
) ON [PRIMARY]
```

The blogid on `Posts` is clear a foreign key to the `Blogs` table. You can map the reference on the `Post` to a `Blog`, and to do it you can use `BelongsToAttribute`:

```csharp
using Castle.ActiveRecord;

[ActiveRecord("posts")]
public class Post : ActiveRecordBase
{
    private int id;
    private string title;
    private string contents;
    private Blog blog;

    [PrimaryKey]
    public int Id
    {
        get { return id; }
        set { id = value; }
    }

    [Property]
    public string Title
    {
        get { return title; }
        set { title = value; }
    }

    [Property(ColumnType="StringClob")]
    public string Contents
    {
        get { return contents; }
        set { contents = value; }
    }

    [BelongsTo("blogid")]
    public Blog OwnerBlog
    {
        get { return blog; }
        set { blog = value; }
    }
}
```

Assigning a blog instance to this property - and obviously saving the post instance - will create the association.

More information on the attribute can be found at Attributes article.

## HasMany

An one-to-many relation can be mapped using the `HasManyAttribute`

Consider the following table script:

```sql
CREATE TABLE Blogs
(
    [id] [int] IDENTITY (1, 1) NOT NULL,
    [name] [varchar] (50) NULL
) ON [PRIMARY]

CREATE TABLE Posts
(
    [id] [int] IDENTITY (1, 1) NOT NULL,
    [title] [varchar] (50) NULL,
    [contents] [text] NULL,
    [blogid] [int] NULL
) ON [PRIMARY]
```

The blogid on `Posts` is clearly a foreign key to the `Blogs` table. You can make the `Blog` class have a set of `Posts` using the `HasManyAttribute`:

```csharp
using Castle.ActiveRecord;

[ActiveRecord("blogs")]
public class Blog : ActiveRecordBase
{
    private int id;
    private string name;
    private IList posts = new ArrayList();

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

    [HasMany(typeof(Post), Table="Posts", ColumnKey="blogid")]
    public IList Posts
    {
        get { return posts; }
        set { posts = value; }
    }
}
```

:information_source: If the other side of the relation (the `Post` class) had a `BelongsTo` relation to the `Blog` class, the you could omit the `Table` and `ColumnKey` properties.

Now we can use the newly added relation:

```csharp
Blog blog = new Blog();
blog.Name = "hammett's blog";
blog.Create();

Post post = new Post();
post.Title = "First post";
post.Contents = "Hello world";
post.Create();

blog.Posts.Add(post);
blog.Update();
```

By default this kind of relation is writable. You can control the behavior using the `Cascade` property on `HasManyAttribute`. You can also turn off the writable behavior by saying that the relation is only controlled by the other side. You can do this using the `Inverse` property. For example:

```csharp
[HasMany(typeof(Post), Table="Posts", ColumnKey="blogid", Inverse=true)]
public IList Posts
{
    get { return posts; }
    set { posts = value; }
}
```

More information on the attribute can be found at Attributes article. For an explanation of the `Inverse concept`, please refer to this [article at nhprof.com](http://nhprof.com/Learn/Alert?name=SuperfluousManyToOneUpdate).

## HasAndBelongsToMany

A many-to-many relation can be mapped using the `HasAndBelongsToMany`. As usual it requires an association table.

Consider the following table script:

```sql
CREATE TABLE Posts
(
    [id] [int] IDENTITY (1, 1) NOT NULL,
    [title] [varchar] (50) NULL,
    [contents] [text] NULL
) ON [PRIMARY]

CREATE TABLE Categories
(
    [id] [int] IDENTITY (1, 1) NOT NULL,
    [title] [varchar] (50) NULL,
) ON [PRIMARY]

CREATE TABLE PostCategory
(
    [postid] [int] NOT NULL,
    [categoryid] [int] NOT NULL
) ON [PRIMARY]
```

The relation is that a post can have many categories and thus a category can be related to many posts.

```csharp
using Castle.ActiveRecord;

[ActiveRecord("posts")]
public class Post : ActiveRecordBase
{
    private int id;
    private string title;
    private string contents;
    private Blog blog;
    private IList categories = new ArrayList();

    [PrimaryKey]
    private int Id
    {
        get { return id; }
        set { id = value; }
    }

    [Property]
    public string Title
    {
        get { return title; }
        set { title = value; }
    }

    [Property(ColumnType="StringClob")]
    public string Contents
    {
        get { return contents; }
        set { contents = value; }
    }

    [BelongsTo("blogid")]
    public Blog OwnerBlog
    {
        get { return blog; }
        set { blog = value; }
    }

    [HasAndBelongsToMany(typeof(Category),
        Table="PostCategory", ColumnKey="postid", ColumnRef="categoryid")]
    public IList Categories
    {
        get { return categories; }
        set { categories = value; }
    }
}
```

The other side of the relation can be mapped identically. The only change is the inversion of `ColumnKey` and `ColumnRef`. It is wise to choose one side of the relation as the owner. The other side, the non-writable, need to use `Inverse=true`.

In the example above it would be semantically correct to have the `Post` class controlling the relation. The other side, `Category`, can optionally have a list of `Posts`, and will use `Inverse=true`.

```csharp
using Castle.ActiveRecord;

[ActiveRecord("categories")]
public class Category : ActiveRecordBase
{
    private int id;
    private string title;
    private IList posts = new ArrayList();

    [PrimaryKey]
    private int Id
    {
        get { return id; }
        set { id = value; }
    }

    [Property]
    public string Title
    {
        get { return title; }
        set { title = value; }
    }

    [HasAndBelongsToMany(typeof(Post),
        Table="PostCategory", ColumnKey="categoryid", ColumnRef="postid", Inverse=true)]
    public IList Posts
    {
        get { return posts; }
        set { posts = value; }
    }
}
```

:information_source: We cannot stress enough how important it is to define a proper Cascade behavior for your relations in a real world application.

More information on the attribute can be found at Attributes article.

## Attributes on the association table

More than often the association table in a many-to-many relation is used to hold association attributes. In the example used so far, the `PostCategory` could have some columns to hold some arbitrary data.

It is desirable then to map this relation correctly to a class that represents the association table. So create a `PostCategory` ActiveRecord type. Now comes the trick part: ActiveRecord classes must have primary keys. So you have two options. Either you add a surrogate key to your association table or you use composite key.

The current support for composite keys does not support relations as the keys, although this is supported by NHibernate. Nevertheless this is on the Roapmap and should be implemented by the next version.

### Association table with surrogate key

On this approach we introduce a primary key in a table where, semantically, the key could be the the two foreign keys.

```sql
CREATE TABLE PostCategory
(
    [id] [int] IDENTITY (1, 1) NOT NULL,
    [postid] [int] NOT NULL,
    [categoryid] [int] NOT NULL,
    [arbitraryvalue] [int] NULL
) ON [PRIMARY]
```

Now it is just a matter of implementing the class as you normally would.

```csharp
using Castle.ActiveRecord;
using NHibernate.Expression;

[ActiveRecord]
public class PostCategory : ActiveRecordBase
{
    private int id;
    private Post post;
    private Category category;
    private int arbitraryvalue;

    [PrimaryKey]
    private int Id
    {
        get { return id; }
        set { id = value; }
    }

    [BelongsTo("postid")]
    public Post Post
    {
        get { return post; }
        set { post = value; }
    }

    [BelongsTo("categoryid")]
    public Category Category
    {
        get { return category; }
        set { category = value; }
    }

    [Property]
    public int ArbitraryValue
    {
        get { return arbitraryvalue; }
        set { arbitraryvalue = value; }
    }

    public static PostCategory[] FindByPost(Post post)
    {
        return FindAll(typeof(PostCategory), Expression.Eq("Post", post));
    }

    public static PostCategory[] FindByCategory(Category category)
    {
        return FindAll(typeof(PostCategory), Expression.Eq("Category", category));
    }
}
```

As you see we introduced find methods to allow the retrival of instances based on an specific post or category.

### Using a composite key

The composite key approach does not require the introduction of a surrogate key, but requires more work and can be used with relations (yet). As you can see the post and category are represent by their ids instead of `Post` and `Category` instance. This is highly undesirable for object oriented domain models.

```csharp
using Castle.ActiveRecord;
using NHibernate.Expression;

[Serializable]
public class PostCategoryKey
{
    private int postid;
    private int categoryid;

    [KeyProperty]
    public int PostId
    {
        get { return postid; }
        set { postid = value; }
    }

    [KeyProperty]
    public int CategoryId
    {
        get { return categoryid; }
        set { categoryid = value; }
    }

    public override int GetHashCode()
    {
        return postid ^ categoryid;
    }

    public override bool Equals(object obj)
    {
        if (this == obj)
        {
            return true;
        }
        PostCategoryKey key = obj as PostCategoryKey;
        if (key == null)
        {
            return false;
        }
        if (postid != key.postid || categoryid != key.categoryid)
        {
            return false;
        }
        return true;
    }
}

[ActiveRecord]
public class PostCategory : ActiveRecordBase
{
    private PostCategoryKey id;
    private int arbitraryvalue;

    [CompositeKey]
    public PostCategoryKey Id
    {
        get { return id; }
        set { id = value; }
    }

    [Property]
    public int ArbitraryValue
    {
        get { return arbitraryvalue; }
        set { arbitraryvalue = value; }
    }

    public static PostCategory[] FindByPost(Post post)
    {
        return FindAll(typeof(PostCategory),
            Expression.Eq("PostCategory_postid", post.Id));
    }

    public static PostCategory[] FindByCategory(Category category)
    {
        return FindAll(typeof(PostCategory),
            Expression.Eq("PostCategory_categoryid", category.Id));
    }
}
```

## OneToOne

The one-to-one implemented by NHibernate is often misunderstood. It is used for classes that share primary keys and useful when you have some kind of Class Table Inheritance.

The primary key of the table which isn't autogenerated must be declared using PrimaryKeyType.Foreign:

```csharp
using Castle.ActiveRecord;

[ActiveRecord("Customer")]
public class Customer : ActiveRecordBase
{
    private int custID;
    private string name;
    private CustomerAddress addr;

    [PrimaryKey]
    public int CustomerID
    {
        get { return custID; }
        set { custID = value; }
    }

    [OneToOne]
    public CustomerAddress CustomerAddress
    {
        get { return addr; }
        set { addr = value; }
    }

    [Property]
    public string Name
    {
        get { return name; }
        set { name = value; }
    }
}

[ActiveRecord("CustomerAddress")]
public class CustomerAddress : ActiveRecordBase
{
    private int custID;
    private string address;
    private Customer cust;

    [PrimaryKey(PrimaryKeyType.Foreign)]
    public int CustomerID
    {
        get { return custID; }
        set { custID = value; }
    }

    [OneToOne]
    public Customer Customer
    {
        get { return cust; }
        set { cust = value; }
    }

    [Property]
    public string Address
    {
        get { return addr; }
        set { addr = value; }
    }
}
```

You should read more about it on NHibernate documentation.

More information on the attribute can be found at Attributes article.

## Any and HasManyToAny

There are certain cases when you need to make an association from an entity to a range of possible objects that doesn't necessarily share a common base class.

:warning: **Warning:** This is a fairly advanced scenario. Try to find a simpler solution if you can.

### Using Any

A simple example may be a payment method in an `Order` class, where the choices are either a bank account or a credit card, like this:

```csharp
using Castle.ActiveRecord;

[ActiveRecord("CreditCards")]
public class CreditCard : ActiveRecordBase, IPaymentMethod
{ ... }

[ActiveRecord("BankAccounts")]
public class BankAccount : ActiveRecordBase, IPaymentMethod
{ ... }
```

A possible `Order` class does not know in advance the payment map, or how to map them. They are not part of any hierarchy (either in the object model or an ActiveRecord one). The solution is to map them to this schema:

```sql
CREATE TABLE Orders
(
  [Id] [int] not null identity(1,1),
  ...
  [Billing_Details_Id] [int] not null,
  [Billing_Details_Type] [nvarchar] not null
)
```

Together `BillingDetailsId` and `BillingDetailsType` points to the correct account or credit card that should pay for the order. Here is the attributes declarations. Note that unlike most other attributes, here you need to specify a few properties. They cannot be infered.

```csharp
[Any(typeof(int), MetaType=typeof(string),
    TypeColumn="Billing_Details_Type",
    IdColumn="Billing_Details_Id",
    Cascade=CascadeEnum.SaveUpdate)]
[Any.MetaValue("CREDIT_CARD", typeof(CreditCard))]
[Any.MetaValue("BANK_ACCOUNT", typeof(BankAccount))]
public IPaymentMethod PaymentMethod
{
    get { ... }
    set { ... }
}
```

The first parameter is the type of the Id column (in this case `BillingDetailsId`), the second is the `MetaType` definition, which in this case mean the type of the the field that defines the type of the id.

Next we have the `TypeColumn` and `IdColumn`, which match `Billing_Details_Type` and `Billing_Details_Id`.

The interesting part is the `Any.MetaValue` attribute. Here, we define that when the value in the `Billing_Details_Type` column is "`CREDIT_CARD`", the value in the `Billing_Details_Id` column is the primary key of a `CreditCard`, and when the `Billing_Details_Type` is "`BANK_ACCOUNT`", then the value in `Billing_Details_Id` should be interpreted as the primary key of a `BankAccount` class.
Quick Note

The type of the property should be of a common type or interface that all the possible objects share (worst case scenario: make it of type `System.Object`).

### Using HasManyToAny

A natural extention of `Any`, the `HasManyToAnyAttribute` provides the same functionality for collections. Here is an example of a class that needs a set of payment methods:

```csharp
[HasManyToAny(typeof(IPayment), "pay_id", "payments_table", typeof(int),
    "Billing_Details_Type", "Billing_Details_Id", MetaType=typeof(string))]
[Any.MetaValue("CREDIT_CARD", typeof(CreditCard))]
[Any.MetaValue("BANK_ACCOUNT", typeof(BankAccount))]
public ISet PaymentMethod
{
    get { ... }
    set { ... }
}
```

The parameters for `HasManyToAny` are (in order of apperances in the constructor):

* `typeof(IPayment)`: the type of the objects in this collection
* `pay_id`: the key column that maps the values in this collection to this object
* `payment_table`: the table for this collection
* `typeof(int)`: the type of the id column - identical to the first parameter of `Any`
* `Billing_Details_Type`: identical in function to the `Billing_Details_Type` mentioned above
* `Billing_Details_Id`: identical to the `{Billing_Details_Id` mentioned above
* `MetaType=typeof(string)`: the type of the type column / identical to the one described above

More information on the attribute can be found at Attributes article.