#include <functional>
#include <algorithm>
#include <ranges>
#include <print>
#include <array>
namespace rg = std::ranges;
namespace vs = rg::views;
using namespace std::string_view_literals;

int main()
{
	// Input Board
	// -----------

	constexpr auto my_board = std::array
	{
		std::array{'A', 'B', 'C', 'E'},
		std::array{'S', 'F', 'C', 'S'},
		std::array{'A', 'D', 'E', 'E'},
	};

	// Targetted String
	// ----------------

	constexpr auto my_word = "ABCCED"sv;

	// Validation
	// ----------

	// constexpr auto my_result = rg::any_of(my_board | vs::join, [](const auto x) { return my_word.contains(x); });
	constexpr auto my_result = rg::any_of(my_board | vs::join, std::bind_front(std::ranges::contains, my_word));

	// Output
	// ------

	std::println("{}", my_result);
}
