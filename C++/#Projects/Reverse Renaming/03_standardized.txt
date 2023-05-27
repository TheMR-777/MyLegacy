#include <filesystem>
#include <ranges>
#include <print>
namespace fs = std::filesystem;
namespace rg = std::ranges;
namespace vs = rg::views;
namespace pr = std;

int main()
{
	const auto my_source = fs::path(R"(D:\WorkShop\Reverse Renaming\org - Copy)");
	constexpr auto to_sv = [](const auto& x) { return std::string_view{ x }; };
	constexpr auto delim = to_sv(" - ");

	for (const fs::path& path : fs::directory_iterator(my_source))
	{
		// Heavy Lifting
		auto filtered = path.stem()		// "1 - 2 - 3"
			.string()						// "1 - 2 - 3"s
			| vs::split(delim)				// "1"s | "2"s | "3"s
			| vs::take(2)					// "1"s | "2"s
			| vs::transform(to_sv)			// "1"sv | "2"sv
			| rg::to<std::vector>()			// { "1"sv, "2"sv }
			;

		// New Name creation
		const auto my_target = pr::format(
			"{}{}{}{}",
			filtered.back(),				// "2"sv
			delim,							// " - "sv
			filtered.front(),				// "1"sv
			path.extension().string()		// ".ext"s
		);									// "2 - 1.ext"s

		// Renaming
		const auto new_path = path.parent_path() / my_target;
		// rename(path, new_path);

		// Output Printing
		pr::println("Old Path: {}", path.string());
		pr::println("New Path: {}", new_path.string());
		pr::println("");
	}
}