# Framework Events

ActiveRecord exposes some events that you might use to integrate it with a top level framework.

## ActiveRecordStarter.SessionFactoryHolderCreated

You can subscribe to this event to get notification about the creation of an `ISessionFactoryHolder` instance implementation.

## ISessionFactoryHolder.OnRootTypeRegistered

You can subscribe to this event to get notification when a *root type* is registered. A *root type* defines the database being used. If you are using only one database the *root type* will be `ActiveRecordBase`.