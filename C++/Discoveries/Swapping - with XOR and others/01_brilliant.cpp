#include <print>

static constexpr auto x = 42;
static constexpr auto y = 77;


int main()
{
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
