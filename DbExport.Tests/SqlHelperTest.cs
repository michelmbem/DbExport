using DbExport.Providers;
using static DbExport.SqlHelper;

namespace DbExport.Tests;

public class TestItem
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public double Price { get; set; }
}

public class SqlHelperTest
{
    static SqlHelperTest()
    {
        Utility.RegisterDbProviderFactories();
    }

    [Fact]
    public void GenericTest()
    {
        // Arrange
        const string sql = """
                           CREATE TABLE test_table (
                               id INTEGER PRIMARY KEY AUTOINCREMENT,
                               name TEXT NOT NULL,
                               price REAL
                           )
                           """;

        List<TestItem> testItems = [];
        
        for (var i = 0; i < 10; i++)
            testItems.Add(new TestItem { Name = $"Item #{i}", Price = 5.0 * i });

        // Act
        using var helper = new SqlHelper(ProviderNames.SQLITE, "Data Source=:memory:");
        helper.Execute(sql);
        helper.ExecuteBatch("INSERT INTO test_table (name, price) VALUES (@name, @price)", testItems, FromEntity);
        
        var fetchedItems = helper.Query("SELECT * FROM test_table", ToEntityList<TestItem>);

        // Assert
        Assert.Equal(testItems.Count, fetchedItems.Count);

        for (var i = 0; i < fetchedItems.Count; i++)
        {
            Assert.Equal(testItems[i].Name, fetchedItems[i].Name);
            Assert.Equal(testItems[i].Price, fetchedItems[i].Price);
        }
    }
}