var now = DateTime.Now;

var my_data_now = new[]
{
    DateTime.Now,
    DateTime.Now.AddMinutes(5),
    DateTime.Now.AddHours(1),
    DateTime.Now.AddDays(1),
};

var my_data_var = new[]
{
    now,
    now.AddMinutes(5),
    now.AddHours(1),
    now.AddDays(1),
};

static void print_differences(DateTime[] my_data)
{
    foreach (var item in my_data)
    {
        Console.WriteLine(item - my_data[0]);
    }
    Console.WriteLine('\n');
}

print_differences(my_data_now);
print_differences(my_data_var);
