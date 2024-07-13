#include <fmt/ranges.h>

namespace my
{
	template <class T>
	struct remove_const
	{
		using type = T;
	};

	template <class T>
	struct remove_const<const T>
	{
		using type = T;
	};
}

int main()
{
	using my_type = my::remove_const<const int>::type;
}