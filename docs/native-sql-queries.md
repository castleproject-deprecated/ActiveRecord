# Native SQL Queries

There are a couple of scenarios in which HQL doesn't provide for all features you need. For example if you want to intersect three or more tables adding them to the "from" clause. Sometimes HQL complies with your requirements (most of them) but you want to get a fine grained SQL statement control.

Teaching Native SQL Queries is out of the scope of this article. You should consult the [NHibernate documentation on Native SQL Queries](http://www.hibernate.org/hib_docs/nhibernate/html/querysql.html).

## CreateSQLQuery

`CreateSQLQuery` is used in cases where the query would be a native SQL query.

Here are an example that involves two models, one to store words and another to store synonyms.

```csharp
using Castle.ActiveRecord.Queries;
using Castle.ActiveRecord;
using Castle.ActiveRecord.Framework;
using System.Collections;
using Iesi.Collections;
using NHibernate.Expression;
using NHibernate;

[ActiveRecord]
public class Word : ActiveRecordBase<Word>
{
    private int _id;
    private string _key;

    public Word() {}

    [PrimaryKey]
    private int Id
    {
        get { return _id; }
        set { _id = value; }
    }

    [Property]
    public string Key
    {
        get { return _key; }
        set { _key = value; }
    }

    public IList<Word> FindSynonyms()
    {
            string query = @"
select synonym.key
from word, synonym
where
    synonym.word = word.id and
    word.key = :key";

            return (IList<Word>) ActiveRecordMediator<Word>.Execute(
                delegate(ISession session, object instance)
                {
                    return session.CreateSQLQuery(query, "synonym", typeof(Word))
                        .SetParameter("key", this.Key)
                        .SetMaxResult(10)
                        .List<Word>();
                }, this);
    }
}
```

Here you have a more complex query. There is a Menu model to store menu items, there is a MenuItemTranslation model to store items translations and the last model is the Language one, to store languages.

```csharp
private const string translationQuery = @"
select menuitemtranslation.translation
from  menu, language, menuitemtranslation
where
    menuitemtranslation.menu = :menuid and
    menuitemtranslation.lang = language.id and
    language.englishname = :lang
";

public string FindTranslation(string lang)
{
    if ((lang == null) || (lang.Length == 0)) return Description;

    IList<string> translations = (IList<string>) ActiveRecordMediator<MenuItemTranslation>.Execute(
        delegate(Isession session, object instance)
        {
            return session.CreateSQLQuery(translationQuery)
                .SetParameter("menuid", this.Id)
                .SetParameter("lang", lang)
                .SetMaxResults(1)
                .IList<string>();
        }, null);
    if ((translations != null) && (translations.Count > 0))
    {
        return translations[0];
    }
    else
    {
        return Description;
    }
}
```