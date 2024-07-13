#include <range/v3/view.hpp>
#include <fmt/ranges.h>
#include <random>
#include <algorithm>
namespace rg = ranges;
namespace vs = rg::views;

auto random_number_gen() noexcept
{
	static auto my_engine = std::mt19937_64{ std::random_device{}() };
	return std::uniform_int_distribution<size_t>{ 10, 99 }(my_engine);
}

int main()
{
	for (constexpr auto my_limit = 27; auto _ : vs::indices(my_limit))
	{
		static const auto to_v = rg::to<std::vector>;
		const auto my_random_n = vs::generate_n(random_number_gen, my_limit) | to_v;
		static auto my_indices = vs::indices(my_limit) | to_v;
		static auto projection = [&](const auto x) { return my_random_n[x]; };

		std::ranges::sort(my_indices, {}, projection);

		fmt::println("{}", my_indices | vs::transform(projection));
	}
}
