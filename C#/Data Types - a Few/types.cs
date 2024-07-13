Console.WriteLine("Hello C# World!");

var myArray = new object[]
{
	0x1234,
	1,
	"Hello",
	3.14,
	3.14f,
	true,
	DateTime.Now
};

foreach (var item in myArray)
{
    Console.WriteLine($"{item.GetType().Name} = {item}");
}
