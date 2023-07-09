#include <fmt/ranges.h>
#include <ranges>
namespace rg = std::ranges;
namespace vs = rg::views;

int main()
{
	auto only_prime = []<std::integral T>(const T x) noexcept
	{
		for (T i = std::sqrt(x); i > 1; --i)
			if (x % i == 0) return false;
		return true;
	};

	auto my_range = vs::iota.operator()<size_t>({}) | vs::filter(only_prime) | vs::take(17);
	fmt::print("{}\n", my_range);
}