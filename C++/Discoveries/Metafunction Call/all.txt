#include <fmt/ranges.h>

template <std::integral auto N>
struct absolute_98
{
	static constexpr auto value = N < 0 ? -N : N;
};

template <std::integral auto N>
constexpr auto absolute_11()
{
	return N < 0 ? -N : N;
}

template <std::integral auto N>
constexpr auto absolute_17 = N < 0 ? -N : N;

consteval auto absolute_20(std::integral auto N)
{
	return N < 0 ? -N : N;
}

int main()
{
	constexpr std::size_t results[]
	{
		absolute_98<-77>::value,	// C++98
		absolute_11<-77>(),			// C++11
		absolute_17<-77>,			// C++17
		absolute_20(-77),			// C++20
	};
}