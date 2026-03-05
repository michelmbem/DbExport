namespace DbExport.Tests;

public static class TestInitializer
{
    static TestInitializer()
    {
        Utility.RegisterDbProviderFactories();
    }

    public static void EnsureInitialized() { }
}
