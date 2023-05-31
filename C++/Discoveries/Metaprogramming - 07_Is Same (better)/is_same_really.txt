#include <print>

namespace my
{
	template <class T1, class T2>
	constexpr auto is_same = false;

	template <class T>
	constexpr auto is_same<T, T> = true;
}

int main()
{
	constexpr auto really_same = my::is_same<int, int>;
}