#include <print>

namespace my
{
	template <class T>
	struct type_is { using type = T; };

	template <bool, class, class F>
	struct conditional : type_is<F> {};

	template <class T, class F>
	struct conditional <true, T, F> : type_is<T> {};
}

int main()
{
	using my_type = my::conditional<true, int, double>::type;
}