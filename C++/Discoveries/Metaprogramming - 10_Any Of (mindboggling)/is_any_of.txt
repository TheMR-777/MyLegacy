#include <print>

namespace my
{
	// Interface
	template <class T, class... Ts>
	constexpr bool is_any_of;

	// Base Case - Last Resort
	template <class T>
	constexpr auto is_any_of<T> = false;

	// Base Case - Actual Driver
	template <class T, class... Ts>
	constexpr auto is_any_of<T, T, Ts...> = true;

	// Recursive Specialization
	template <class T, class U, class... Ts>
	constexpr auto is_any_of<T, U, Ts...> = is_any_of<T, Ts...>;
}

int main()
{
	constexpr auto x = my::is_any_of<int, float, double>;
}