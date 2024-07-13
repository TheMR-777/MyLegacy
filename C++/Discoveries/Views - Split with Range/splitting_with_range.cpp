#include <fmt/ranges.h>
#include <ranges>
#include <vector>
namespace rg = std::ranges;
namespace vs = rg::views;

int main()
{
	const auto my_range = vs::iota(0, 100) | rg::to<std::vector>();
	const auto my_split = vs::iota(10, 90) | rg::to<std::vector>();

	fmt::println("{}", my_range | vs::split(my_split));
}