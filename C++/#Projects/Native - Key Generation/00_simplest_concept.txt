#include <range/v3/view.hpp>
#include <fmt/ranges.h>
#include <ranges>
#include <random>
namespace rg = ranges;
namespace vs = rg::views;

struct to_hex : std::ranges::range_adaptor_closure<to_hex>
{
	auto operator()(std::string_view input) const noexcept
	{
		return input | vs::transform([](const auto c) { return fmt::format("{:02x}", c); }) | vs::join | rg::to<std::string>;
	}
};

auto generate_key(const size_t length) noexcept
{
	auto random_numbers_g = []
	{
		static auto my_engine = std::mt19937_64{ std::random_device{}() };
		return std::uniform_int_distribution<uint16_t>{ 0,255 }(my_engine);
	};
	return vs::generate_n(random_numbers_g, length) | rg::to<std::string>;
}

int main()
{
	constexpr auto size = 32;
	while (true)
	{
		fmt::println("{}", generate_key(size) | to_hex{});
	}
}
