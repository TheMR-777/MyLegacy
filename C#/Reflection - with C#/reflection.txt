internal class Program
{
    public class my_type_01
    {
        public float id = 2101.77f;
        public string name = "TheMR";
    }

    public class my_type_02
    {
        public int v_01 = 27;
        public int v_02 = 42;
    }

    static void power_print(object val)
    {
        foreach (var field in val.GetType().GetFields())
        {
            Console.WriteLine("{0} = {1}", field.Name, field.GetValue(val));
        }
        Console.WriteLine();
    }

    private static void Main(string[] args)
    {
        power_print(new my_type_01());
        power_print(new my_type_02());
    }
}