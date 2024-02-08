#include <print>
#include <array>
using my_t = intmax_t;

int main()
{
	constexpr static auto my_array = std::array{ 2, -8, 0, 1, -77, 0, 0, 1, -2, -3, 4, 5 };

	auto get_sums = []() noexcept
	{
		auto ps = my_t{ 0 };
		auto ns = my_t{ 0 };
		for (const auto i : my_array)
		{
			i > 0 ? ++ps : i < 0 ? ++ns : 0;
		}
		return std::pair{ ps, ns };
	};

	constexpr auto result = get_sums();
}