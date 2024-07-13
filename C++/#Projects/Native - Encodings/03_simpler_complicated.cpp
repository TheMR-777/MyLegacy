#include <print>

namespace convert
{
	using namespace std::literals;
	static constexpr auto my_hex_map = "0123456789abcdef"sv;
	static constexpr auto base64_map = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/"sv;

	auto to_hex(std::string_view input) noexcept
	{
		auto output = std::string(input.size() * 2, '\0');

		for (size_t x = 0; x < input.size(); ++x)
		{
			output[x * 2] = my_hex_map[input[x] >> 4];
			output[x * 2 + 1] = my_hex_map[input[x] & 0x0F];
		}

		return output;
	}

	auto to_base64(std::string_view input) noexcept
	{
		auto output = std::string((input.size() + 2) / 3 * 4, '\0');

		for (size_t x = 0, y = 0; x < input.size(); x += 3, y += 4)
		{
			output[y] = base64_map[input[x] >> 2];
			output[y + 1] = base64_map[((input[x] & 0x03) << 4) | ((x + 1 < input.size()) ? (input[x + 1] >> 4) : 0)];
			output[y + 2] = (x + 1 < input.size()) ? base64_map[((input[x + 1] & 0x0F) << 2) | ((x + 2 < input.size()) ? (input[x + 2] >> 6) : 0)] : '=';
			output[y + 3] = (x + 2 < input.size()) ? base64_map[input[x + 2] & 0x3F] : '=';
		}

		return output;
	}
}

int main()
{
	static constexpr auto my_data = "TheMR - 7";
	std::println("{}", convert::to_hex(my_data));
	std::println("{}", convert::to_base64(my_data));
}
