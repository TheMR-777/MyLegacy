#include <print>

namespace my
{
	template <class T>
	constexpr auto is_ref = false;

	template <class T>
	constexpr auto is_ref<T&> = true;

	template <class T>
	constexpr auto is_ref<T&&> = true;
}

int main()
{
	using T1 = int;
	using T2 = T1&;
	using T3 = const T2&&;

	constexpr auto r1 = my::is_ref<T1>;
	constexpr auto r2 = my::is_ref<T2>;
	constexpr auto r3 = my::is_ref<T3>;
}