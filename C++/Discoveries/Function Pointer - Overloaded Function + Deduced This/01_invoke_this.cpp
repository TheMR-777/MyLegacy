#include <print>

using x_t = std::size_t;
using c_t = std::double_t;

struct wonderful;
template <class T>
concept is_wonderful = std::same_as<std::remove_cvref_t<T>, wonderful>;

struct wonderful
{
	x_t x = 47;

	[[nodiscard]] constexpr auto mutate_val_01(const x_t val) const noexcept { return x + val; }
	[[nodiscard]] constexpr auto mutate_val_01(const c_t val) const noexcept
	{
		const auto temp = static_cast<x_t>(val);
		return x + temp + (val > static_cast<c_t>(temp));
	}
	[[nodiscard]] constexpr auto mutate_val_02(this is_wonderful auto&& self, const x_t val) noexcept { return self.mutate_val_01(val); }
	[[nodiscard]] constexpr auto mutate_val_02(this is_wonderful auto&& self, const c_t val) noexcept { return self.mutate_val_01(val); }
};

template <class T>
constexpr auto wonderful_mutate_01_t = static_cast<x_t (wonderful::*)(T) const noexcept>(&wonderful::mutate_val_01);

template <class T>
constexpr auto wonderful_mutate_02_t = static_cast<x_t (*)(const wonderful&, T) noexcept>(&wonderful::mutate_val_02);

int main()
{
	constexpr auto data = wonderful{};
	constexpr auto p_01 = &decltype(data)::x;
	constexpr auto p_02_01 = wonderful_mutate_01_t<x_t>;
	constexpr auto p_02_02 = wonderful_mutate_01_t<c_t>;
	constexpr auto p_01_01 = wonderful_mutate_02_t<x_t>;
	constexpr auto p_01_02 = wonderful_mutate_02_t<c_t>;

	// Preferred Way:
	{
		constexpr auto v_01 = std::invoke(p_01, data);
		constexpr auto v_01_01 = std::invoke(p_02_01, data, 3);
		constexpr auto v_01_02 = std::invoke(p_02_02, data, 2.3);
		constexpr auto v_02_01 = std::invoke(p_01_01, data, 3);
		constexpr auto v_02_02 = std::invoke(p_01_02, data, 2.3);
	}
	// Manual Way:
	{
		constexpr auto v_01 = (data.*p_01);
		constexpr auto v_01_01 = (data.*p_02_01)(3);
		constexpr auto v_01_02 = (data.*p_02_02)(2.3);
		constexpr auto v_02_01 = p_01_01(data, 3);
		constexpr auto v_02_02 = p_01_02(data, 2.3);
	}
}
