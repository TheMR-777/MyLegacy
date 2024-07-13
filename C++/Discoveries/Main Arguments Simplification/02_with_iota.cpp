#include <print>
#include <ranges>

int main(const size_t n, const char* args[])
{
	auto my_arguments = std::views::iota(*args, *args + n);
	
	for (const std::string_view arg : my_arguments)
	{
		std::println("{}", arg);
	}
}
