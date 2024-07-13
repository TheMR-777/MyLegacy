#include <print>

consteval auto absolute(std::integral auto N)
{
	return N < 0 ? -N : N;
}

int main()
{
	constexpr auto x = absolute(-63);
}