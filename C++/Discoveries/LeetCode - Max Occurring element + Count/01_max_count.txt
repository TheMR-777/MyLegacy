#include <print>
#include <random>
#include <ranges>
#include <unordered_map>
namespace rg = std::ranges;
namespace vs = rg::views;

auto get_random(size_t) noexcept
{
	static auto my_generator = std::mt19937_64{ std::random_device{}() };
	return std::uniform_int_distribution<size_t>{ 0, 100 }(my_generator);
}

int main()
{
	static constexpr auto my_size = 777;
	static const auto m_container = rg::iota_view<size_t, size_t>(0, my_size) | vs::transform(get_random) | rg::to<std::vector>();
	static constexpr auto get_max = []() noexcept
	{
		static auto my_map = std::unordered_map<size_t, size_t>{};
		for (const auto& element : m_container)
		{
			++my_map[element];
		}
		return rg::max_element(my_map, {}, &decltype(my_map)::iterator::value_type::second);
	};

	const auto max = get_max();
	std::println("<{}: {}x>", max->first, max->second);
}
