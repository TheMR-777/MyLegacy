#include <print>
#include <random>
#include <limits>
using my_t = size_t;

auto get_random()
{
	static auto my_engine = std::mt19937_64{ std::random_device{}() };
	return std::uniform_int_distribution{ std::numeric_limits<my_t>::min(), std::numeric_limits<my_t>::max() }(my_engine);
}

static auto x = get_random();
static auto y = get_random();


int main()
{
	// Original Values
	std::println("<x: {}, y: {}>", x, y);
	{
		// Using the Exchange Technique
		auto x = ::x, y = ::y;
		x = std::exchange(y, x);
		std::println("<x: {}, y: {}>", x, y);
	}
	{
		// Using the STL Technique
		auto x = ::x, y = ::y;
		std::swap(x, y);
		std::println("<x: {}, y: {}>", x, y);
	}
	{
		// Using the XOR Technique
		auto x = ::x, y = ::y;
		x ^= y ^= x ^= y;
		std::println("<x: {}, y: {}>", x, y);
	}
	{
		// Using Addition Technique
		auto x = ::x, y = ::y;
		x -= y = (x += y) - y;
		std::println("<x: {}, y: {}>", x, y);
	}
}
