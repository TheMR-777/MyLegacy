namespace MyPlayground;

public interface IEndpointProvider<T> where T : IEndpointProvider<T>
{
    static abstract string Endpoint { get; }
}

public class UserService : IEndpointProvider<UserService>
{
    public static string Endpoint => "https://api.example.com/user";
}

public class ProductService : IEndpointProvider<ProductService>
{
    public static string Endpoint => "https://api.example.com/product";
}

public class OrderService : IEndpointProvider<OrderService>
{
    public static string Endpoint => "https://api.example.com/order";
}

class Program
{
    private static string GetEndpoint<T>() where T : IEndpointProvider<T> => T.Endpoint;

    static void Main()
    {
        Console.WriteLine(GetEndpoint<UserService>());
        Console.WriteLine(GetEndpoint<ProductService>());
        Console.WriteLine(GetEndpoint<OrderService>());
    }
}
