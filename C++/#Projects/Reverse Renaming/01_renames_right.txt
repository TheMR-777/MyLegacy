#include <fmt/ranges.h>
#include <filesystem>
#include <ranges>
namespace fs = std::filesystem;
namespace rg = std::ranges;
namespace vs = rg::views;

int main()
{
	const auto my_source = fs::path(R"(D:\WorkShop\Reverse Renaming\org - Copy)");
	constexpr auto delim = std::string_view{ " - " };

	for (const fs::path& path : fs::directory_iterator(my_source))
	{
		auto my_chunks = path.filename()
			.string() 
			| vs::split(delim) 
			| vs::transform([](const auto x) { return std::string_view{ x }; }) 
			| vs::take(2) 
			| rg::to<std::vector>()
		;
		auto my_target = fmt::format(
			"{}{}{}{}", 
			my_chunks.back(), 
			delim, 
			my_chunks.front(), 
			path.extension().string()
		);

		fmt::println("{}", my_target);
	}
}