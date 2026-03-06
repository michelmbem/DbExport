namespace DbExport.Tests;

public static class TestInitializer
{
    /// <summary>
    /// A static constructor that registers the DbProviderFactories for the test environment.
    /// </summary>
    static TestInitializer()
    {
        Utility.RegisterDbProviderFactories();
    }

    /// <summary>
    /// A dummy method that ensures that the static constructor has been executed.
    /// </summary>
    public static void EnsureInitialized()
    {
        // This method does nothing, but it ensures that the static constructor has been executed.
    }
}
