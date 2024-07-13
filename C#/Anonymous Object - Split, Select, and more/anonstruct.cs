
const string my_number = "0300-9530478 -> 0314-5401405";

my_number.Split(new string[] { "->" }, StringSplitOptions.RemoveEmptyEntries)
         .Select(x => x.Trim().Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries))
         .Select(x => new { AreaCode = x[0], Number = x[1] })
         .ToList()
         .ForEach(Console.WriteLine);
