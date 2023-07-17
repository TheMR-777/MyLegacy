
var my_text = "TheMR";
var in_bits = BitConverter.ToString(System.Text.Encoding.UTF8.GetBytes(my_text)).Replace("-", " ");

Console.WriteLine(in_bits);
