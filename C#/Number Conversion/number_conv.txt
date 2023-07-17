var my_value = 786765;
var new_vals = new List<string>
{
    my_value.ToString("C"),
    my_value.ToString("C", new System.Globalization.CultureInfo("en-US")),
    my_value.ToString("C3"),
};

new_vals.ForEach(Console.WriteLine);