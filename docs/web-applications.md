# Web Applications

For a web application you should initialize ActiveRecord on the `Application_Start` call. It is also advisable that you keep a session scope per request.

## Initializing

As `Application_Start` will be executed only once per application instance, it is the best place to initialize the framework. The initialization is covered on [Configuration and Initialization](configuration-and-initialization.md) document. Here we are just going to show you how to get this working in a web application.

First, if your web application does not have a `global.asax file`, create one like the one below. Fix the namespace to match your project namespace.

```
<%@ Application Inherits="My.Web.App.MyHttpApplication" %>
```

Then create a class that extends `HttpApplication`.

```csharp
namespace My.Web.App
{
    using System;
    using System.Web;

    using Castle.ActiveRecord;
    using Castle.ActiveRecord.Framework;
    using Castle.ActiveRecord.Framework.Config;

    using My.Web.App.Models;

    public class MyHttpApplication : HttpApplication
    {
        protected void Application_Start(Object sender, EventArgs e)
        {
            // Replace the code below as you want to match your
            // preference about ActiveRecord initialization

            IConfigurationSource source = ActiveRecordSectionHandler.Instance;

            ActiveRecordStarter.Initialize( source,
                typeof(Account),
                typeof(AccountPermission),
                typeof(ProductLicense),
                typeof(SimplePerson),
                typeof(Category),
                typeof(PersonBase),
                typeof(PersonUser));
        }
    }
}
```

## Enabling a SessionScope per request

You might use the `HttpApplication` to subscribe to the `BeginRequest` and `EndRequest` events. Those are the best place to start a session scope and to dispose it.

```csharp
namespace My.Web.App
{
    using System;
    using System.Web;

    using Castle.ActiveRecord;
    using Castle.ActiveRecord.Framework;
    using Castle.ActiveRecord.Framework.Config;

    using My.Web.App.Models;

    public class MyHttpApplication : HttpApplication
    {
        public MyHttpApplication()
        {
            // Subscribe to the events

            BeginRequest += new EventHandler(OnBeginRequest);
            EndRequest += new EventHandler(OnEndRequest);
        }

        protected void Application_Start(Object sender, EventArgs e)
        {
            // Initialization code omitted
        }

        public void OnBeginRequest(object sender, EventArgs e)
        {
            // You might want to configure a different kind of session scope here, ie a readonly one
            HttpContext.Current.Items.Add("ar.sessionscope", new SessionScope());
        }

        public void OnEndRequest(object sender, EventArgs e)
        {
            try
            {
                SessionScope scope = HttpContext.Current.Items["ar.sessionscope"] as SessionScope;

                if (scope != null)
                {
                    scope.Dispose();
                }
            }
            catch(Exception ex)
            {
                HttpContext.Current.Trace.Warn("Error", "EndRequest: " + ex.Message, ex);
            }
        }
    }
}
```

This approach gives you control over the `SessionScope` initialization. But if you do not want anything different than the standard functionality, you may replace the code above for the addition of a module.

## Using the SessionScopeWebModule

The `SessionScopeWebModule` module does exactly what the code above does -- subscribes to the events and creates and disposes of a `SessionScope`.

To use it, just add the following entry to your `web.config`:

```xml
<system.web>
    <httpModules>
        <add
            name="ar.sessionscope"
            type="Castle.ActiveRecord.Framework.SessionScopeWebModule, Castle.ActiveRecord" />
    </httpModules>
</system.web>
```