#include <print>

namespace my
{
	template <class T>
	struct type_is { using type = T; };

	template <bool, class, class F>
	struct if_ : type_is<F> {};

	template <class T, class F>
	struct if_<true, T, F> : type_is<T> {};

	template <bool, class T>
	struct only_if : type_is<T> {};

	template <class T>
	struct only_if<false, T> {};
}

int main()
{
	using value_type_01 = my::if_<true, int, float>::type;
	using value_type_02 = my::only_if<true, size_t>::type;
}