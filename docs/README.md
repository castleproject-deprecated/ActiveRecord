# Castle ActiveRecord Documentation

<img align="right" src="images/castle-logo.png">

The **Castle ActiveRecord** project is an implementation of the [ActiveRecord pattern](http://en.wikipedia.org/wiki/Active_record) for .NET. The ActiveRecord pattern consists on instance properties representing a record in the database, instance methods acting on that specific record and static methods acting on all records.

**Castle ActiveRecord** is built on top of [NHibernate](http://www.nhibernate.org/), but its attribute-based mapping frees the developer of writing XML for database-to-object mapping, which is needed when using NHibernate directly.

:warning: **Warning:** Although ActiveRecord makes using NHibernate easy it does not hide all details of NHibernate behaviour. You **need** to understand NHibernate flushing behaviour and how to work with text/ntext columns.

## Reference Manual

* [Understanding Scopes](understanding-scopes.md) - Explains the SessionScope and TransactionScope classes
* [Validators](validators.md)
* [XML Configuration Reference](xml-configuration-reference.md)

## User's Guide

* [Getting Started](getting-started.md) - A tutorial for "how", after the introduction explains "what"
* [Configuration and Initialization](configuration-and-initialization.md) - Explains the configuration options and schema, and provides some illustrative examples as well
* [Creating an ActiveRecord class](creating-an-activerecord-class.md)
* [The Persistency Lifecycle](persistency-lifecycle.md)
* [Primary Key Mapping](primary-key-mapping.md)
* [Simple Column Mapping](simple-column-mapping.md)
* [Relations Mapping](relations-mapping.md)
  * [BelongsTo](relations-mapping.md#belongsto)
  * [HasMany](relations-mapping.md#hasmany)
  * [HasAndBelongsToMany](relations-mapping.md#hasandbelongstomany)
  * [OneToOne](relations-mapping.md#onetoone)
  * [Any and HasManyToAny](relations-mapping.md#any-and-hasmanytoany)
* [Schema Generation](schema-generation.md)
* [Unit Testing](unit-testing.md)
* [Type Hierarchy](type-hierarchy.md) - Discuss approaches to achieve inheritance within your object model and map it correctly to the underlying database
* [Nested Data (NHibernate Components)](nested-data.md)
* [Using HQL (Hibernate Query Language)](using-hql.md) - Illustrates the usage of HQL
* [Native SQL Queries](native-sql-queries.md)
* [Using Scopes](using-scopes.md)
* [Enabling Lazy Load](enabling-lazy-load.md)
* [Validation Support](validation-support.md) - Presents the ActiveRecordValidationBase that is able to validate properties with predefined validators
* [Best Practices](best-practices.md) - Some recommendations
* [Web Applications](web-applications.md)
* [Troubleshooting](troubleshooting.md) - Something went wrong? Check what problems we had and how we work around them
* [Frequently Asked Questions](faq.md)
* [External Articles on ActiveRecord](external-articles.md)

## Advanced Usage

* [Using the ActiveRecordMediator (avoiding a base class)](using-the-activerecordmediator.md)
* [Using Version and Timestamp](using-version-and-timestamp.md)
* [Using Imports](using-imports.md)
* [Hooks and Lifecycle](hooks-and-lifecycle.md)
* [Framework Events](framework-events.md)
* [Accessing more than one database](accessing-more-than-one-database.md)
* [Tuning (Performance Improvements)](tuning.md)
* [ByteCode Generator and Scopeless Lazy Loading](bytecode-generator.md)

## Integrations

* [MonoRail ActiveRecord Integration](https://github.com/castleproject/MonoRail/blob/master/MR2/docs/activerecord-integration.md)
* [MonoRail ActiveRecord Scaffolding](https://github.com/castleproject/MonoRail/blob/master/MR2/docs/getting-started-with-activerecord-integration.md)