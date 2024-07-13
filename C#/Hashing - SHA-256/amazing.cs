
string my_input = "TheMR";
string the_hash = BitConverter.ToString(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(my_input)));//.Replace("-", "").ToLower();

Console.WriteLine(the_hash);
