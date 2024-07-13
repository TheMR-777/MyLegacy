#include <print>
#include <algorithm>
#include <ranges>

int main()
{
	auto summer = [x = 0](auto y) mutable { return x += y; };
	auto my_val = std::views::iota(0, 5);

	// std::ranges::for_each(my_val, summer);
	std::ranges::for_each(my_val, std::reference_wrapper{ summer });

	std::println("{}", summer(0));
}