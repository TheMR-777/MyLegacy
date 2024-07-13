const string my_sentence = "Hi, it's TheMR, from Punjab University Jhelum Campus. I now work at ACE Company. I am from Pakistan.";
const int sentence_limit = 30;
var sentence_count = 0;

foreach (var word in my_sentence.Split(' '))
{
    if ((sentence_count += word.Length + 1) > sentence_limit) break;
}

Console.WriteLine(my_sentence[..sentence_limit] + "...");
Console.WriteLine(my_sentence[..sentence_count] + "...");