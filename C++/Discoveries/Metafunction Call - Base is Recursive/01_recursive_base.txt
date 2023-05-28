#include <fmt/ranges.h>

template <class T>
struct rank
{
	static constexpr auto value = 0;
};

template <class T, size_t N>
struct rank<T[N]>
{
	static constexpr size_t value = 1 + rank<T>::value;
};

int main()
{
	constexpr auto r = rank<int[2][3][4]>::value;
}