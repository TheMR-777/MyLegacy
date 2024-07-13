#include <fmt/ranges.h>
#include <array>

namespace code
{
	using value_type = std::byte;
	using under_type = std::uint8_t;

	static constexpr auto base64_table = std::string_view{ "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/" };
	static constexpr auto my_hex_table = std::string_view{ "0123456789ABCDEF" };
	static constexpr auto padding = '=';

	auto to_base64(std::string_view data) noexcept
	{
		auto result = std::string{};
		result.reserve(data.size() * 4 / 3);

		auto it = data.begin();
		auto const end = data.end();

		while (it != end)
		{
			auto const b0 = static_cast<under_type>(*it++);
			auto const b1 = (it != end) ? static_cast<under_type>(*it++) : 0;
			auto const b2 = (it != end) ? static_cast<under_type>(*it++) : 0;

			auto const b0_idx = b0 >> 2;
			auto const b1_idx = ((b0 & 0b11) << 4) | (b1 >> 4);
			auto const b2_idx = ((b1 & 0b1111) << 2) | (b2 >> 6);
			auto const b3_idx = b2 & 0b111111;

			result += base64_table[b0_idx];
			result += base64_table[b1_idx];
			result += (b1 != 0) ? base64_table[b2_idx] : padding;
			result += (b2 != 0) ? base64_table[b3_idx] : padding;
		}

		return result;
	}

	auto to_hex(std::string_view data) noexcept
	{
		auto result = std::string{};
		result.reserve(data.size() * 2);

		for (auto const b : data)
		{
			auto const b0 = static_cast<under_type>(b) >> 4;
			auto const b1 = static_cast<under_type>(b) & 0b1111;

			result += my_hex_table[b0];
			result += my_hex_table[b1];
		}

		return result;
	}

	auto from_base64(std::string_view data) noexcept
	{
		auto result = std::string{};
		result.reserve(data.size() * 3 / 4);

		auto it = data.begin();
		auto const end = data.end();

		while (it != end)
		{
			auto const b0 = base64_table.find(*it++);
			auto const b1 = (it != end) ? base64_table.find(*it++) : 0;
			auto const b2 = (it != end) ? base64_table.find(*it++) : 0;
			auto const b3 = (it != end) ? base64_table.find(*it++) : 0;

			auto const b0_idx = (b0 << 2) | (b1 >> 4);
			auto const b1_idx = ((b1 & 0b1111) << 4) | (b2 >> 2);
			auto const b2_idx = ((b2 & 0b11) << 6) | b3;

			result += static_cast<char>(b0_idx);
			if (b2 != padding)
				result += static_cast<char>(b1_idx);
			if (b3 != padding)
				result += static_cast<char>(b2_idx);
		}

		return result;
	}

	auto from_hex(std::string_view data) noexcept
	{
		auto result = std::string{};
		result.reserve(data.size() / 2);

		auto it = data.begin();
		auto const end = data.end();

		while (it != end)
		{
			auto const b0 = my_hex_table.find(*it++);
			auto const b1 = my_hex_table.find(*it++);

			result += static_cast<char>((b0 << 4) | b1);
		}

		return result;
	}
}

int main()
{
	static constexpr auto my_data = std::string_view{ "TheMR" };
	static const auto my_base64_e = code::to_base64(my_data);
	static const auto my_base64_d = code::from_base64(my_base64_e);
	static const auto the_hex_enc = code::to_hex(my_data);
	static const auto the_hex_dec = code::from_hex(the_hex_enc);

	fmt::println("Original  : {}", my_data);
	fmt::println("");
	fmt::println("Base64 Encode   : {}", my_base64_e);
	fmt::println("Base64 Decode   : {}", my_base64_d);
	fmt::println("");
	fmt::println("Hex Encode      : {}", the_hex_enc);
	fmt::println("Hex Decode      : {}", the_hex_dec);
}