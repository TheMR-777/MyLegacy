#include <print>
#include <span>
#include <ranges>

int main(const size_t n, const char* args[])
{
	auto my_arguments = std::span{ args, n };
	//| std::views::transform([](const auto arg) { return std::string_view{ arg }; });
	
	for (const std::string_view arg : my_arguments)
	{
		std::println("{}", arg);
	}
}
