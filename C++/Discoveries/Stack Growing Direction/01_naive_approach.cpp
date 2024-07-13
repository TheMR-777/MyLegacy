#include <print>

constexpr bool polarity()
{
	constexpr int x = 0, y = 1;
	return &x < &y;
}

int main()
{
	const auto result = polarity();
	std::print("Polarity: {}\n", result);
}
