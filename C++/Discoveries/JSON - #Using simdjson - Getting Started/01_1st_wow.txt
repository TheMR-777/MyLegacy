#include <simdjson.h>

int main()
{
	constexpr std::string_view my_json = R"(
	{
		"id": 77,
		"name": "TheMR",
		"message": "Hello simdjson!"
	}
	)";

	simdjson::dom::parser parser;
	auto json = parser.parse(my_json);

	const auto id = json["id"].get_uint64();
	const auto name = json["name"].get_c_str();
	const auto message = json["message"].get_c_str();
}
