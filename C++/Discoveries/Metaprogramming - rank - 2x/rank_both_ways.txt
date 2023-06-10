#include <iostream>

namespace my_old
{
	template <class>
	constexpr auto rank = 0;

	template <class T, size_t N>
	constexpr auto rank<T[N]> = 1 + rank<T>;

	template <class T>
	constexpr auto rank<T[]> = 1 + rank<T>;
}

namespace my_new
{
	template <class T>
	constexpr auto rank_details() noexcept
	{
		if constexpr (std::is_array_v<T>)
		{
			return size_t{ 1 } + rank_details<std::remove_extent_t<T>>();
		}
		else
		{
			return 0;
		}
	}

	template <class T>
	constexpr auto rank = rank_details<T>();
}

int main()
{
	using my_t = int[][1][2][3][4][5];

	constexpr auto the_old = my_old::rank<my_t>;
	constexpr auto the_new = my_new::rank<my_t>;
}