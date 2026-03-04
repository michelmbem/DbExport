using DbExport.Providers;
using static DbExport.SqlHelper;

namespace DbExport.Tests;

public class TestItem
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public decimal Price { get; set; }
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
                               price NUMERIC
                           )
                           """;

        List<TestItem> testItems = [];
        
        for (var i = 0; i < 10; i++)
            testItems.Add(new TestItem { Name = $"Item #{i}", Price = 5M * i });

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

        var item2 = helper.Query("SELECT * FROM test_table WHERE id = :id", new { Id = 2 }, FromEntity, ToEntity<TestItem>);
        Assert.NotNull(item2);
        Assert.Equal(2, item2.Id);

        item2.Name = "Modified Item";
        item2.Price = 180M;

        int affected = helper.Execute("UPDATE test_table SET name = $name, price = $price WHERE id = $id", item2, FromEntity);
        Assert.Equal(1, affected);
    }
}