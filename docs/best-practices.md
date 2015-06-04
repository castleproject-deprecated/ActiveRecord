# Best Practices

After eating our own dog food for a while, we think we might have some valuable tips for newcomers.

## Create test cases for your domain model

Having test cases for your domain model allow you to easily identify what change broke some functionality. This is a general rule for test cases but some people tend to neglect test cases for things that involve databases.

We think that testing each layer individually is important, including the data access layer. Just make sure there is a separated databases to be used exclusively by the test cases.

As the application grows big, you might end up with a complex object model with lots of interconnections. So testing become a complex issue, for example, imagine that you want to test an Order class, but to test it you must create a Product, and a Supplier and an Consumer.

We recommend using [ObjectMother pattern](http://martinfowler.com/bliki/ObjectMother.html) for such cases.

## Test interactions and relations

The interactions among objects are especially important. When they are meaningful, create special methods for them that assert some conditions.

For example, suppose you have a `Pool` and `Vote` classes. The `Pool` might expose a `RegisterVote` method. Have your test asserting the correct behavior for it.

## Use relations wisely

There are a number of things ofter overlooked when relations are defined. The Inverse property for example, defines which side of the relation manages the collection. The types used (`List`, `List`) have an implied semantic which often is not considered.

The best source of information about relations is (and will ever be) the [NHibernate documentation](http://nhforge.org/doc/nh/en/index.html).

## Define a Cascade behavior for your relations

The cascade defines how NHibernate should handle children/parent associations when something is changed or deleted. Castle ActiveRecords always defaults to `None` so it is up to you to specify the behavior you want.

One more time the best source of information about cascades is the [NHibernate documentation](http://nhforge.org/doc/nh/en/index.html).