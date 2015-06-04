# Hooks and Lifecycle

The `ActiveRecordBase` class implements NHibernate's `ILifecycle` interface. For more information on the methods involve, consult the [NHibernate documentation](http://nhforge.org/doc/nh/en/index.html).

Additionaly all `ISession` instances are linked to an `IInterceptor` implementation which ultimately invokes your ActiveRecord class instance. This is a fairly advanced usage.