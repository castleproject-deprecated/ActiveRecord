# Troubleshooting

Sometimes things go wrong. This section lists actions you may take to find out why a mapping is not being accepted, or why your query is not working.

## Enabling ActiveRecord debug

When you enable Castle ActiveRecord debug mode, it output the mapping files it produces for each entity. This might be valuable to identify problems, or to report problems to NHibernate team.

For instructions on how to enable it check the [XML Configuration Reference](xml-configuration-reference.md).

## Enabling NHibernate debug

NHibernate uses log4net, so it's just a matter of configuring it. For example, to have a log.txt file with all debug information you can get, use:

```xml
<configuration>

    <configSections>
    <section name="nhibernate" type="System.Configuration.NameValueSectionHandler, System, Version=1.0.5000.0,Culture=neutral, PublicKeyToken=b77a5c561934e089" />
        <section name="log4net"
                 type="log4net.Config.Log4NetConfigurationSectionHandler, log4net" />
    </configSections>

    <log4net>

        <!-- Define some output appenders -->
        <appender name="trace" type="log4net.Appender.TraceAppender, log4net">
            <layout type="log4net.Layout.PatternLayout,log4net">
                <param name="ConversionPattern" value="%d [%t] %-5p %c [%x] &amp;lt;%P{user}&amp;gt; - %m%n" />
            </layout>
        </appender>

        <appender name="console" type="log4net.Appender.ConsoleAppender, log4net">
            <layout type="log4net.Layout.PatternLayout,log4net">
                <param name="ConversionPattern" value="%d [%t] %-5p %c [%x] &amp;lt;%P{user}&amp;gt; - %m%n" />
            </layout>
        </appender>

        <appender name="rollingFile" type="log4net.Appender.RollingFileAppender,log4net" >
            <param name="File" value="log.txt" />
            <param name="AppendToFile" value="true" />
            <param name="RollingStyle" value="Date" />
            <param name="DatePattern" value="yyyy.MM.dd" />
            <param name="StaticLogFileName" value="true" />

            <layout type="log4net.Layout.PatternLayout,log4net">
                <param name="ConversionPattern" value="%d [%t] %-5p %c [%x] &amp;lt;%X{auth}&amp;gt; - %m%n" />
            </layout>

        </appender>

        <root>
            <!-- priority value can be set to ALL|INFO|WARN|ERROR -->
            <priority value="ALL" />
            <appender-ref ref="rollingFile" />
        </root>

    </log4net>

    <nhibernate>
        <!-- with this set to true, SQL statements
         will output to the console window if it's a console app -->
        <add key="hibernate.show_sql" value="true" />
    </nhibernate>

</configuration>
```

After that, make sure you are initializing the log4net config in your code by invoking its initializer.

```csharp
log4net.Config.XmlConfigurator.Configure();
```

Then, run your test cases and check the log file for Exceptions - yes, do a search. There will be plenty of information about what went wrong. Note that log.txt will be in the same directory as your EXE or Web application directory -- not necessarily where your .csproj file is.