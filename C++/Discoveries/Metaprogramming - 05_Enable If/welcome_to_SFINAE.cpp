#include <print>

namespace my
{
	template <class T>
	struct type_is { using type = T; };

	template <bool B, class T>
	struct enable_if : type_is<T> {};

	template <class T>
	struct enable_if<false, T> {};
}

int main()
{
	using my_type = my::enable_if<true, int>::type;
}