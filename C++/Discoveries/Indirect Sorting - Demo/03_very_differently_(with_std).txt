#include <print>
#include <ranges>
#include <random>
#include <algorithm>
namespace rg = std::ranges;
namespace vs = rg::views;

int main()
{
	using my_type = std::size_t;

	static constexpr auto m_vector = rg::to<std::vector>();
	static constexpr auto my_limit = my_type{ 27 };
	static auto my_random_number_g = [](...) noexcept
	{
		static auto my_engine = std::mt19937_64{ std::random_device{}() };
		return std::uniform_int_distribution<my_type>{ 10, 99 }(my_engine);
	};
	static auto the_sorted_indices = vs::iota(my_type{ 0 }, my_limit) | m_vector;

	for (const auto _ : the_sorted_indices)
	{
		auto my_random_numbers = the_sorted_indices | vs::transform(my_random_number_g) | m_vector;
		static auto projection = [&](const std::integral auto x) noexcept
		{
			return my_random_numbers[x];
		};

		std::ranges::sort(the_sorted_indices, {}, projection);

		for (const auto x : the_sorted_indices | vs::transform(projection))
		{
			std::print("{} ", x);
		}
		std::print("\n");
	}
}
