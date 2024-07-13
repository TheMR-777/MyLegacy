#include <simdjson.h>

int main()
{
	constexpr std::string_view pure_json = R"(
	{
		"id": 77,
		"name": "TheMR",
		"message": "Hello simdjson!"
	}
	)";

	auto parser = simdjson::dom::parser{};
	auto m_json = parser.parse(pure_json);

	const auto id = m_json["id"].get_uint64();
	const auto name = std::string_view{ m_json["name"] };
	const auto message = std::string_view { m_json["message"] };
}
