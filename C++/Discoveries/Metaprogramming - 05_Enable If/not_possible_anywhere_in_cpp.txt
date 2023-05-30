#include <print>

namespace my
{
	template<bool, class>
	struct enable_if {};

	template<class T>
	struct enable_if<true, T> { using type = T; };
}

int main()
{
	using my_type = my::enable_if<true, int>::type;
}