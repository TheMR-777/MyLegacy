#include <simdjson>
#include <ranges>
#include <iostream>

int main()
{
	constexpr std::string_view json = R"(
	{
		"name": "TheMR",
		"type": "Student",
		"d_id": 77,
		"is_x": true,
		"lang": ["C++", "C#/.NET", "Rust"],
		"CGPA": 3.79
	})";

	auto parser = simdjson::dom::parser{};
	auto m_json = parser.parse(json);

	for (auto [key, value] : m_json.get_object())
	{
		std::cout << key << ": " << value << '\n';
	}
}
