#include <fmt/ranges.h>

namespace my
{
	template <class T>
	struct type_identity
	{
		using type = T;
	};

	template <class T>
	struct remove_const : type_identity<T> { };

	template <class T>
	struct remove_const<const T> : type_identity<T> { };
}

int main()
{
	using my_type = my::remove_const<const int>::type;
}