#include <fmt/ranges.h>

template <class T>
static constexpr size_t rank = 0;

template <class T, size_t N>
static constexpr size_t rank<T[N]> = 1 + rank<T>;

int main()
{
	constexpr auto r = rank<int[1][2][3]>;
}