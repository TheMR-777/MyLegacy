#include <algorithm>
#include <array>

int main()
{
	constexpr static auto my_array = std::array{ 2, -8, 0, 1, -77, 0, 0, 1, -2, -3, 4, 5 };
	constexpr static auto negative = [](const std::integral auto x) { return x < 0; };
	constexpr static auto positive = [](const std::integral auto x) { return x > 0; };
	constexpr auto count_negatives = std::ranges::count_if(my_array, negative);
	constexpr auto count_positives = std::ranges::count_if(my_array, positive);
}