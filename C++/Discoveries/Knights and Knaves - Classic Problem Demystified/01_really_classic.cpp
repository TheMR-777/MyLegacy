#include <string_view>

int main()
{
	// Possibilities:
	// --------------
	// Path A: Leads to the Motel
	// Path B: Leads to the Forest
	// K: is either a Knight or a Knave
	// I want to go to the Motel, regardless of what K is.

	constexpr auto k = false;		// Knave
	constexpr auto a = std::string_view{ "Path A - Motel" };
	constexpr auto b = std::string_view{ "Path B - Forest" };

	constexpr auto correct_path = a;

	// Solution 01: Using if-else
	{
		constexpr auto r = k
			? correct_path								    // ↓ This will always be true
			: (correct_path == a ? b : a) != correct_path ? correct_path : b;
	}

	// Solution 02: Using Inverter for Simplification
	{
		constexpr auto inverter = [](std::string_view path) constexpr noexcept
		{
			return path == a ? b : a;
		};

		constexpr auto r = k
			? correct_path
			: inverter(inverter(correct_path));
	}

	// The Question: If I am to ask K, "Which path leads to the Motel?", what would K say?
}
