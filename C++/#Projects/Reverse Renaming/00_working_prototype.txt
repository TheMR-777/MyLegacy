#include <fmt/ranges.h>
#include <filesystem>
#include <ranges>
namespace fs = std::filesystem;
namespace rg = std::ranges;
namespace vs = rg::views;

int main()
{
	const auto my_source = fs::path(R"(D:\WorkShop\Reverse Renaming\org - Copy)");

	constexpr auto my_delimiter = std::string_view{ " - " };

	for (const fs::path& path : fs::directory_iterator(my_source))
	{
		auto my_chunks = path.filename().string() | vs::split(my_delimiter) | vs::transform([](const auto x) { return std::string_view{ x }; });

		fmt::println("{}", my_chunks);
	}
}