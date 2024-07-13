#include <print>

namespace my
{
	template <class T, class... Ts>
	constexpr bool is_any_of = (std::is_same_v<T, Ts> || ...);
}

int main()
{
	constexpr auto x = my::is_any_of<int, float, double, int>;
}