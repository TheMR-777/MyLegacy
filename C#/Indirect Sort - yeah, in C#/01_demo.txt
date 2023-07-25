
/* Indirect Sorting Demonstration */

const int myLimit = 27;
static int RandomNumber() => new Random().Next(10, 99);
var myIndices = Enumerable.Range(0, myLimit).ToArray();

foreach (var _ in myIndices)
{
	var myRandoms = myIndices.Select(_ => RandomNumber()).ToArray();
	Array.Sort(myIndices, (x, y) => myRandoms[x].CompareTo(myRandoms[y]));
	Console.WriteLine(string.Join(" ", myIndices.Select(i => myRandoms[i])));
}
