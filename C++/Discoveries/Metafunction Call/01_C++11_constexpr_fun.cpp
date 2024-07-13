#include <print>

template <std::integral auto N>
constexpr auto absolute()
{
	return N < 0 ? -N : N;
}

int main()
{
	constexpr auto x = absolute<-77>();
}