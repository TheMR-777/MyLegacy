#include <print>
#include <functional>

int main()
{
	auto my_lambda = [](const int x, const int y) noexcept
	{
		std::println("<{}, {}>", x, y);
	};

	auto my_func = std::bind_back(my_lambda, 1);
	my_func(7);
}
