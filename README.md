# Meziantou.DataGenerator

Generate not so random data for your database.

Many data generators exists:

- Binary
- Boolean
- Brand name
- Color
- Country
- Culture / Lcid
- Date
- Email
- FileName
- FirstName
- LastName
- FullName
- Gender
- Guid
- Lipsum
- Number
- Password
- Phone number (US and FR)
- String
- Username
- Foreign keys

Currently there is no user interface to choose which data generator to use for each column, but
the system is clever (at least a little :)) and try to choose the best depending of different
criteria such as column type and name.

You can add your own hints to help choosing the right data generator by using an xml file.
```
<hints>
  <hint typeName="data generator type name" table="table name pattern" column="column name pattern" />
  <hint typeName="Meziantou.DataGenerator.Core.DataGenerators.ColorGenerator" column="Color$" />
</hints>
```

## Dependencies

- CodeFluent.Runtime & CodeFluent.Runtime.Database from [CodeFluent Entities](http://www.softfluent.com/products/codefluent-entities)
- [Rex](http://research.microsoft.com/en-us/projects/rex/)

## TODO
- If an error occurs while inserting a row, you will have less than expected rows in the database
- A better user interface (not that hard :))
