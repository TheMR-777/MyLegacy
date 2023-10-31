using System.Text.Json;

namespace MySpace
{
    /* -------------------------------------
     * System.Text.Json Demo
     * ------------------------------------- */
    public class Product
    {
        public int Id { get; set; } = 0;
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; } = 0.0m;
    }

    class MyProgram
    {
        public static void Main()
        {
            var myProduct = new Product
            {
                Id = 1,
                Name = "MGS Cologne",
                Price = 9.99m
            };

            var serialized = JsonSerializer.Serialize(myProduct, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            System.Console.WriteLine(serialized);
        }
    }
}
