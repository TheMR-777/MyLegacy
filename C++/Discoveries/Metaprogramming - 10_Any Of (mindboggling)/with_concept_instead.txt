#include <print>

namespace my
{
	template <class T, class... Ts>
	concept is_any_of = (std::same_as<T, Ts> && ...);
}

int main()
{
	constexpr auto x = my::is_any_of<int, float, double>;
}