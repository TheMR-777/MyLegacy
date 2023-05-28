#include <fmt/ranges.h>

template <std::unsigned_integral auto X, std::unsigned_integral auto Y>
struct GCD
{
	static constexpr auto value = GCD<Y, X % Y>::value;
};

// Partial Specialization
// template<> doesn't matter,
// What matters, is the template parameters GCD<x, y>
// It's like a function overloading, and pattern matching
// It also serves as a base case

template <std::unsigned_integral auto X>
struct GCD<X, 0u>
{
	static constexpr auto value = X;
};

int main()
{
	constexpr auto gcd = GCD<12u, 18u>::value;
}