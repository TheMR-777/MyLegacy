#include <print>

struct wonderful
{
	std::size_t x = 47;
	[[nodiscard]] constexpr auto get_val_01() const noexcept { return x; }
	[[nodiscard]] constexpr auto&& get_val_02(this auto&& self) noexcept { return self.x; }
};

int main()
{
	constexpr auto data = wonderful{};
	constexpr auto p_01 = &decltype(data)::x;
	constexpr auto p_02 = &decltype(data)::get_val_01;
	constexpr auto p_03 = &decltype(data)::get_val_02<std::add_lvalue_reference_t<decltype(data)>>;

	// Preferred Way:
	{
		constexpr auto v_01 = std::invoke(p_01, data);
		constexpr auto v_02 = std::invoke(p_02, data);
		constexpr auto v_03 = std::invoke(p_03, data);
	}
	// Manual Way:
	{
		constexpr auto v_01 = (data.*p_01);
		constexpr auto v_02 = (data.*p_02)();
		constexpr auto v_03 = p_03(data);
	}
}
