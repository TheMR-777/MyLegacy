var my_sentence = "Hi, this is TheMR, from Punjab University Jhelum Campus. I now work at ACE Company. I am from Pakistan.";
const int limit = 30;
var mySummarize = string.Empty;

{
    var count = 0;
    var words = my_sentence.Split(' ');

    foreach (var word in words)
    {
        if (count + word.Length > limit)
        {
            break;
        }

        count += word.Length + 1;
    }

    mySummarize = my_sentence[..count] + "...";
}

Console.WriteLine(mySummarize);