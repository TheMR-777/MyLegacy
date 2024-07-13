#include <fmt/ranges.h>

template <std::unsigned_integral auto X, std::unsigned_integral auto Y>
static constexpr auto GCD = GCD<Y, X % Y>;

template <std::unsigned_integral auto X>
static constexpr auto GCD<X, 0u> = X;

int main()
{
	constexpr auto gcd = GCD<12u, 18u>;
}