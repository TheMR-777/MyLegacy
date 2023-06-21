#include <algorithm>

int main()
{
	constexpr auto x = 77;
	constexpr auto y = 27;
	constexpr auto z = 33;

	constexpr auto result = std::minmax({ x, y, z });
}