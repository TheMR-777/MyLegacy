#include <print>

template <std::integral auto N>
static constexpr auto absolute = N < 0 ? -N : N;

int main()
{
	constexpr auto x = absolute<-47>;
}