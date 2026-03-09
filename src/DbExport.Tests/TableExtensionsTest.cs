using DbExport.Providers;
using DbExport.Schema;

namespace DbExport.Tests;

public class TableExtensionsTest
{
    private const string ConnectionString = "Data Source=InMemorySample;Mode=Memory;Cache=Shared";

    static TableExtensionsTest()
    {
        TestInitializer.EnsureInitialized();
    }

    [Fact]
    public void GenericTest()
    {
        // Arrange
        const string sql = """
                           DROP TABLE IF EXISTS products;
                           DROP TABLE IF EXISTS categories;
                           CREATE TABLE categories
                           (
                               id INTEGER PRIMARY KEY AUTOINCREMENT,
                               name TEXT NOT NULL
                           );
                           CREATE TABLE products
                           (
                               id INTEGER PRIMARY KEY AUTOINCREMENT,
                               name TEXT NOT NULL,
                               price NUMERIC,
                               categoryid INTEGER NOT NULL,
                               FOREIGN KEY (categoryid) REFERENCES categories (id)
                           )
                           """;

        List<Category> categories = [];
        List<Product> products = [];
        var k = 1L;

        for (var i = 1; i <= 5; ++i)
        {
            categories.Add(new Category { Id = i, Name = $"Category #{i}" });

            for (var j = 1; j <= 10; ++j)
            {
                products.Add(new Product { Id = k, Name = $"Product #{i}:{j}", Price = 5M * j, CategoryId = i });
                ++k;
            }
        }

        // Act
        using (var helper = new SqlHelper(ProviderNames.SQLITE, ConnectionString))
            helper.Execute(sql);

        var database = SchemaProvider.GetDatabase(ProviderNames.SQLITE, ConnectionString, null);

        var categoryTable = database.Tables["main.categories"];
        categoryTable.InsertBatch(categories);
        
        var productTable = database.Tables["main.products"];
        productTable.InsertBatch(products);

        // Assert
        var fetchedCategories = categoryTable.Select<Category>();
        Assert.Equal(categories.Count, fetchedCategories.Count);

        for (var i = 0; i < fetchedCategories.Count; ++i)
            Assert.Equal(categories[i], fetchedCategories[i]);

        var fetchedProducts = productTable.Select<Product>();
        Assert.Equal(products.Count, fetchedProducts.Count);

        for (var i = 0; i < fetchedProducts.Count; ++i)
            Assert.Equal(products[i], fetchedProducts[i]);

        fetchedProducts = productTable.Select<Product>(productTable.ForeignKeys[0], 4L);
        Assert.Equal(10, fetchedProducts.Count);

        for (var i = 0; i < fetchedProducts.Count; ++i)
        {
            Assert.Equal(categories[3].Id, fetchedProducts[i].CategoryId);
            Assert.Equal(products[30 + i], fetchedProducts[i]); // products[3 * 10 + i]
        }

        k = 1L;
        foreach (var product in fetchedProducts)
        {
            product.Name = $"Product #{k} of category #{product.CategoryId}";
            ++k;
        }

        Assert.True(productTable.UpdateBatch(fetchedProducts));

        for (var i = 1; i <= 10; ++i)
        {
            var product = Assert.Single(productTable.Select<Product>(productTable.PrimaryKey, 30L + i));
            Assert.Equal($"Product #{i} of category #4", product.Name);
        }

        Assert.True(productTable.DeleteBatch(fetchedProducts));
        Assert.Empty(productTable.Select<Product>(productTable.ForeignKeys[0], 4L));

        fetchedProducts = productTable.Select<Product>();
        Assert.Equal(products.Count - 10, fetchedProducts.Count);
        Assert.DoesNotContain(fetchedProducts, p => p.CategoryId == 4L);

        var newProduct = new Product { Name = "My fancy product", CategoryId = 4L };
        Assert.True(productTable.Insert(newProduct));

        newProduct = Assert.Single(productTable.Select<Product>(productTable.ForeignKeys[0], 4L));
        Assert.NotEqual(0L, newProduct.Id);
        Assert.Equal("My fancy product", newProduct.Name);

        k = newProduct.Id;
        newProduct.Name = "Updated fancy product";
        Assert.True(productTable.Update(newProduct));

        newProduct = Assert.Single(productTable.Select<Product>(productTable.PrimaryKey, k));
        Assert.Equal("Updated fancy product", newProduct.Name);

        Assert.True(productTable.Delete(newProduct));
        Assert.Empty(productTable.Select<Product>(productTable.PrimaryKey, k));
        Assert.False(productTable.Delete(k));
    }

    #region Inner types

    private sealed class Category
    {
        public long Id { get; set; }

        public string? Name { get; set; }

        public override bool Equals(object? obj) => obj is Category cat &&
            Equals(Id, cat.Id) && Equals(Name, cat.Name);

        public override int GetHashCode() => HashCode.Combine(Id, Name);

        public override string ToString() => $"{nameof(Category)} {{ Id: {Id}, Name: {Name} }}";
    }

    private sealed class Product
    {
        public long Id { get; set; }

        public string? Name { get; set; }

        public decimal Price { get; set; }

        public long CategoryId { get; set; }

        public override bool Equals(object? obj) => obj is Product prod &&
            Equals(Id, prod.Id) && Equals(Name, prod.Name) &&
            Equals(Price, prod.Price) && Equals(CategoryId, prod.CategoryId);

        public override int GetHashCode() => HashCode.Combine(Id, Name, Price, CategoryId);

        public override string ToString() =>
            $"{nameof(Product)} {{ Id: {Id}, Name: {Name}, Price: {Price}, CategoryId: {CategoryId} }}";
    }

    #endregion
}
