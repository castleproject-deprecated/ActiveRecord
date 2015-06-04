# Tuning (Performance Improvements)

This article explains some steps towards a better performance.

## Too many selects (select n+1)

:information_source: This is the most common reason for performance issues with OR/M based applications. Be sure to read this section and the relevant documentation in NHibernate's site thoroughfully. In nearly all cases, some thought and careful appliance of eager fetching can solve the issue.

Using an O/RM can greatly simplify your life, but is has its on set of Gotcha that you need to be aware of. One of the more serious ones is the Select N + 1 issue. Let us start from the simple case, of the usual Blog -->> Posts model. Assuming that you have not enabled lazy loading, what would be the result of this line of code?

```csharp
Blog[] blogs = Blog.FindAll();
```

Well, we have a single select to grab all the blogs from the database. And then, for each blog a select to grab all its posts (and then a select per post to load its comments, etc). That is a problem usually refered as the Select N + 1 problem. Fortantely, we can easily solve this by marking the assoication from blog to posts and from post to comments as lazy. See the lazy load section and [Enabling lazy load|Enabling lazy load] for the details.

Now, let us assume that all your collections are lazy loaded and that you have the following piece of code:

```csharp
foreach (Post post in blog.Posts)
{
    foreach (Comment comment in post.Comments)
    {
        //print comment...
    }
}
```

What is going on in here? We execute a SELECT to grab all the Posts in the blogs. Then, for each of the posts, we need to execute another select. We just got that issue again! But here we have another tool at our disposal, eager loading.

## Lazy load

One problem that you may encounter is that loading a single entity causes its entire object graph to be loaded with it. (You load a post, and it loads its blog, which loads all its posts, which load all of their comments, etc...). This issue is solved by using Lazy Loading, which means that NHibernate will only load the data that you are interested at.

Enable lazy load for relations as described in the [Enabling lazy load|Enabling lazy load]. Lazy loading can be defined at the class level as well, using `ActiveRecord(Lazy=true)`.

Marking a class as lazy means that you all its persistent properties must be virtual (so NHibernate is able to intercept and load the property when it is required) and all accesses (even inside the class) to properties must be done using the properties (and not the fields).

:information_source: Lazy loading is only possible when you are inside a scope!

Lazy loading is one of the more important tools in increasing the performance of an application, and like most performance related issues, it require testing in the context of the application to know what is the best solution for each scenario.

## Overused Relations and eager loadings

If your code does intensive work on relations, consider replacing the operation by an [HQL query|HQL query]. Databases are very optimized to handle process on a huge amount of data.

Using HQL, you can specify the data that you want to load, saving database roundtrips. For instnace, here is how we load a post with all its comments:

```csharp
from Post p join fetch p.Comments where p.Id = 1
```

There are a couple of things that you need to know about eager loading (check the "Too many selects" section to see more details about using it effectively):

* Eager loading is only avialable using HQL queries in Active Record
* You can eagerly load only a single collection (HasMany, HasManyAndBelongsToMany) assoication per query, but any number of entity (BelongsTo) assoications.

Eager loading is used by using the fetch modifier to a join. In the above query, we saw that we can use it to load a collection eagerly. Now let us see how we can load a collection (HasMany) and an assoication (BelongsTo) eagerly:

```csharp
from Post p
		join fetch p.Comments
		join fetch p.Blog
	where p.Id = 1
```

Remember that NHibernate is issuing a join to gather all this data, so eager loading many assoications is not recommended. Issue several seperted queries and let NHibernate sort the results into a coherent object graph.

The query above will completely solve the Select N+1 issue for the code shown in the introduction. This efficently grab all the data from the database in a single query.

In almost all cases, it is better to ask the database for the data as expclictly as possible, rather than loading the data on demand. The HQL langauge is can bring a lot of power into your hands. Consult the [NHibernate documentation on HQL](http://docs.jboss.org/hibernate/stable/core/reference/en/html/queryhql.html) for all the detials.

## Profiling Active Record

The easiest way to figure out what is going on with Active Record is to watch how it talks to the database. In a console application, all we need to do is to tell NHibernate that we would like it to show us the queries, by adding this to Active Record config section:

```xml
<add key="show_sql" value="true" >
```

However, in a web (or WinForms for that matter) scenario, that is not very helpful. For that, we need to redirect NHibernate's logging to a useful place. Let's start by simply logging all the queries to the trace (which we can show as part of the page). Add the following to your web.config file:

```xml
<configSections>
    <section name="log4net"  type="log4net.Config.Log4NetConfigurationSectionHandler, log4net" />
</configSections>

<log4net>
    <appender name="trace" type="log4net.Appender.TraceAppender, log4net">
        <layout type="log4net.Layout.PatternLayout,log4net">
            <param name="ConversionPattern" value="%d [%t] %-5p %c [%x] &lt;%P{user}> - %m%n" />
        </layout>
    </appender>

    <appender name="NHibernate.SQL">
        <appender-ref ref="trace" />
    </appender>
</log4net>
```

Another very useful tool in this regard is [SQL Server Profiler](http://msdn.microsoft.com/en-us/library/ms173799.aspx), which can show you the queries executed on the server in real time. If you don't your SQL Server, you might consider using the commercial tool [NHibernate Profiler](http://nhprof.com/). If you want to profile the application itself, you can use a commercial profiling software like [dotTrace profiler](http://www.jetbrains.com/profiler/).