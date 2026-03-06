using DbExport.Providers;
using static DbExport.SqlHelper;

namespace DbExport.Tests;

public class SqlHelperTest
{
    [Fact]
    public void GenericTest()
    {
        // Arrange
        TestInitializer.EnsureInitialized();

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

        var testItem = helper.Query("SELECT * FROM test_table WHERE id = :id", new { Id = 2 }, FromEntity, ToEntity<TestItem>);
        Assert.NotNull(testItem);
        Assert.Equal(2, testItem.Id);

        testItem.Name = "Modified Item";
        testItem.Price = 180M;

        var affected = helper.Execute("UPDATE test_table SET name = $name, price = $price WHERE id = $id", testItem, FromEntity);
        Assert.Equal(1, affected);

        affected = helper.ExecuteBatch("UPDATE test_table SET name = 'Item that costs $' || price WHERE name = @name", fetchedItems, FromEntity);
        Assert.Equal(fetchedItems.Count - 1, affected);

        fetchedItems = helper.Query("SELECT * FROM test_table WHERE name NOT LIKE 'Item that costs $%'", ToEntityList<TestItem>);
        Assert.Equal(testItem, Assert.Single(fetchedItems));
    }

    #region Inner types

    private sealed class TestItem
    {
        public long Id { get; set; }

        public string? Name { get; set; }

        public decimal Price { get; set; }

        public override bool Equals(object? obj) => obj is TestItem item &&
            Equals(Id, item.Id) && Equals(Name, item.Name) && Equals(Price, item.Price);

        public override int GetHashCode() => HashCode.Combine(Id, Name, Price);

        public override string ToString() =>
            $"{nameof(TestItem)} {{ Id: {Id}, Name: {Name}, Price: {Price} }}";
    }

    #endregion
}