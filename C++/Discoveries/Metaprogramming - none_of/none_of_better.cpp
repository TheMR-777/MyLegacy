#include <print>

namespace my
{
	template <class, class...>
	constexpr auto is_none_of = true;

	template <class T, class... Ts>
	constexpr auto is_none_of<T, T, Ts...> = false;

	template <class T, class U, class... Ts>
	constexpr auto is_none_of<T, U, Ts...> = is_none_of<T, Ts...>;
}

int main()
{
	using _01 = int;
	using _02 = float;
	constexpr auto x = my::is_none_of<_01, _02, _02>;
}