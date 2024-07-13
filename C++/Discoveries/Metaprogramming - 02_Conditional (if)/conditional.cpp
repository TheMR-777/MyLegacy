#include <print>

namespace my
{
	template <class T>
	struct type_is { using type = T; };

	template <bool, class T, class>
	struct conditional: type_is<T> {};

	template <class T, class F>
	struct conditional<false, T, F>: type_is<F> {};

	template <bool B, class T, class F>
	using conditional_t = conditional<B, T, F>::type;
}

int main()
{
	using value_type = my::conditional_t<true, int, double>;
}