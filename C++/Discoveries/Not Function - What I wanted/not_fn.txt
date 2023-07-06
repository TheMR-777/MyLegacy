#include <fmt/ranges.h>
#include <ranges>
#include <functional>
namespace rg = std::ranges;
namespace vs = rg::views;

int main()
{
	auto only_prime = [](const std::integral auto x) noexcept
	{
		for (auto i = x / 2; i > 1; --i)
			if (x % i == 0) return false;
		return true;
	};

	auto my_range = vs::iota(1) | vs::filter(std::not_fn(only_prime)) | vs::take(10);
	fmt::print("{}\n", my_range);
}