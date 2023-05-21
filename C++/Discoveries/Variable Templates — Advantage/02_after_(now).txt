#include <print>

template <std::integral auto N>
static constexpr auto absolute_v = N < 0 ? -N : N;

int main()
{
	constexpr auto x = absolute_v<-77>;
}