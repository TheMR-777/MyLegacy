#include <print>
#include <span>

int main(const std::size_t n, const char** args)
{
	const auto my_arguments = std::span{ args, n };
	for (std::string_view argument : my_arguments)
	{
		std::println("{}", argument);
	}
}
