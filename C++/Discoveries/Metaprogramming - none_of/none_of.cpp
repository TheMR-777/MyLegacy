#include <print>

namespace my
{
	template <class T, class... Ts>
	constexpr bool none_of;

	template <class T>
	constexpr auto none_of<T> = true;

	template <class T, class... Ts>
	constexpr auto none_of<T, T, Ts...> = false;

	template <class T, class U, class...Ts>
	constexpr auto none_of<T, U, Ts...> = none_of<T, Ts...>;
}

int main()
{
	constexpr auto x = my::none_of<int, float, double>;
}