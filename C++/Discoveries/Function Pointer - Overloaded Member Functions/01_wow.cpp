#include <print>

using x_t = std::size_t;
using c_t = std::double_t;

struct wonderful
{
	x_t x = 47;
	[[nodiscard]] constexpr auto mutate_val(const x_t val) const noexcept { return x + val; }
	[[nodiscard]] constexpr auto mutate_val(const c_t val) const noexcept
	{
		const auto temp = static_cast<x_t>(val);
		return x + temp + (val > static_cast<c_t>(temp));
	}
};

template <class T>
constexpr auto wonderful_mutate_t = static_cast<x_t (wonderful::*)(T) const noexcept>(&wonderful::mutate_val);

int main()
{
	constexpr auto data = wonderful{};
	constexpr auto p_01 = &decltype(data)::x;
	constexpr auto p_02 = wonderful_mutate_t<x_t>;
	constexpr auto p_03 = wonderful_mutate_t<c_t>;

	// Preferred Way:
	{
		constexpr auto v_01 = std::invoke(p_01, data);
		constexpr auto v_02 = std::invoke(p_02, data, 3);
		constexpr auto v_03 = std::invoke(p_03, data, 2.3);
	}
	// Manual Way:
	{
		constexpr auto v_01 = (data.*p_01);
		constexpr auto v_02 = (data.*p_02)(3);
		constexpr auto v_03 = (data.*p_03)(2.3);
	}
}
