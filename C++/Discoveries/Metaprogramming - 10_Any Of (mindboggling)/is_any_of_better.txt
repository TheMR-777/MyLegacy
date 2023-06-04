#include <print>

namespace my
{
	template <class T, class... Ts>
	constexpr bool is_any_of = false;

	template <class T, class... Ts>
	constexpr auto is_any_of<T, T, Ts...> = true;

	template <class T, class U, class... Ts>
	constexpr auto is_any_of<T, U, Ts...> = is_any_of<T, Ts...>;
}

int main()
{
	constexpr auto x = my::is_any_of<int>;
}