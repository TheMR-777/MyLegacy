using System;
using System.Linq;

var my_array = Enumerable.Range(0, 10).Select(x => new Random().Next(10, 99)).ToArray();
var my_print = (string msg) => {
    Console.WriteLine(msg);
    Array.ForEach(my_array, x => Console.Write(x + " "));
    Console.WriteLine('\n');
};

my_print("Original");

Array.Reverse(my_array);
my_print("Reversed");

Array.Sort(my_array);
my_print("Sorted");