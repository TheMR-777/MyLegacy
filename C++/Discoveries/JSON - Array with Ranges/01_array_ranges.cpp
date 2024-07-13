#include <simdjson>
#include <ranges>
namespace rg = std::ranges;
namespace vs = rg::views;

template <class T>
constexpr auto convert_to = [](std::convertible_to<T> auto&& source) noexcept
{
	return static_cast<T>(source);
};

int main()
{
	constexpr std::string_view raw_json = R"(
		{
			"id": 77,
			"name": "TheMR",
			"motto": "Since 2001",
			"marks": 2.077,
			"passed": true,
			"subjects": ["Mathematics", "Computer Science", "Computer Engineering"]
		}
	)";

	auto parser = simdjson::dom::parser{};
	const auto element = parser.parse(raw_json);

	auto id = static_cast<uint64_t>(element["id"]);
	auto name = static_cast<std::string_view>(element["name"]);
	auto motto = static_cast<std::string_view>(element["motto"]);
	auto marks = static_cast<double_t>(element["marks"]);
	auto passed = static_cast<bool>(element["passed"]);
	auto subjects = element["subjects"] 
		| vs::transform(convert_to<std::string_view>) 
		| rg::to<std::vector>();
}
