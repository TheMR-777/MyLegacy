#include <fmt/ranges.h>
#include <range/v3/view.hpp>
#include <algorithm>
#include <random>
namespace rg = ranges;
namespace vs = rg::views;

int main()
{
	using my_type = std::size_t;
	static constexpr auto limit = my_type{ 27 };
	static constexpr auto to_vc = rg::to<std::vector>;
	static auto random_number_g = []
	{
		static auto my_engine = std::mt19937_64{ std::random_device {}() };
		return std::uniform_int_distribution<my_type>{ 10, 99 }(my_engine);
	};
	static auto my_indices = vs::indices.operator()<my_type>(limit) | to_vc;
	
	for (auto _ : vs::indices(limit))
	{
		const auto my_random_n = vs::generate_n(random_number_g, limit) | to_vc;
		static auto projection = [&] (const auto& i) { return my_random_n[i]; };

		std::ranges::sort(my_indices, {}, projection);
		fmt::println("{}", my_indices | vs::transform(projection));
	}
}