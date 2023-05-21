#include <print>

template <std::integral auto N>
struct absolute
{
	static constexpr auto value = N < 0 ? -N : N;
};

int main()
{
	constexpr auto x = absolute<-42>::value;
}