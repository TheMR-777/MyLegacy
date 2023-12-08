// Demo: GivenText should partial-match the SavedText in database
// Hint: SavedText must be the (case-insensitive) Superset of GivenText
// Cases:
// 1. SavedText: "Mr Lance Henry" GivenText: "Mr,Lance,Henry" Result: True
// 2. SavedText: "Mr Lance Henry" GivenText: ",,Mr Lance Henry" Result: True
// 3. SavedText: "Close Mtahc" GivenText: "Mrs,Close,Mtahc" Result: True
// 5. SavedText: "Close Mtahc" GivenText: "Close" Result: True
// 4. SavedText: "Close Mtahc" GivenText: "Mrs" Result: False
// 6. SavedText: "Close Mtahc" GivenText: ",no-match" Result: False

internal class MyProgram
{
	public static readonly char[] supportedSeparators = [',', ' ','.'];

	public static void Main()
	{
		const string saved = "Close Mtahc";
		const string given = ",no-match";

		var result = PartialMatch(given, saved);
	}

	public static bool PartialMatch(string given, string saved)
	{
		var givenParts = given.Split(supportedSeparators, StringSplitOptions.RemoveEmptyEntries);
		var savedParts = saved.Split(supportedSeparators, StringSplitOptions.RemoveEmptyEntries);
		return givenParts.All(sec => savedParts.Contains(sec, StringComparer.OrdinalIgnoreCase));
	}
}