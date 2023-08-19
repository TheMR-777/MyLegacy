#include <print>
#include <array>

namespace code
{
	using value_type = std::byte;
	using under_type = std::uint8_t;

	template <class T>
	concept my_type = requires(T t)
	{
		{ t.get() } -> std::convertible_to<std::string_view>;
	};

	static constexpr auto base64_table = std::string_view{ "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/" };
	static constexpr auto my_hex_table = std::string_view{ "0123456789abcdef" };
	static constexpr auto padding = '=';

	template <my_type T>
	constexpr auto to_base64() noexcept
	{
		constexpr auto data = std::string_view{ T{}.get() };
		auto result = std::array<char, (data.size() / 3) * 4 + 4>{};

		for (auto [i, it] = std::pair{ size_t{}, result.begin() }; i < data.size(); i += 3)
		{
			auto const b0 = static_cast<under_type>(data[i]);
			auto const b1 = (i + 1 < data.size()) ? static_cast<under_type>(data[i + 1]) : 0;
			auto const b2 = (i + 2 < data.size()) ? static_cast<under_type>(data[i + 2]) : 0;

			auto const b0_idx = b0 >> 2;
			auto const b1_idx = ((b0 & 0b11) << 4) | (b1 >> 4);
			auto const b2_idx = ((b1 & 0b1111) << 2) | (b2 >> 6);
			auto const b3_idx = b2 & 0b111111;

			*it++ = base64_table[b0_idx];
			*it++ = base64_table[b1_idx];
			*it++ = (b1 != 0) ? base64_table[b2_idx] : padding;
			*it++ = (b2 != 0) ? base64_table[b3_idx] : padding;
		}

		return result;
	}

	template <my_type T>
	constexpr auto to_hex() noexcept
	{
		constexpr auto data = std::string_view{ T{}.get() };
		auto result = std::array<char, data.size() * 2>{};

		for (auto [i, it] = std::pair{ size_t{}, result.begin() }; i < data.size(); ++i)
		{
			auto const b0 = static_cast<under_type>(data[i]) >> 4;
			auto const b1 = static_cast<under_type>(data[i]) & 0b1111;

			*it++ = my_hex_table[b0];
			*it++ = my_hex_table[b1];
		}

		return result;
	}

	template <my_type T>
	constexpr auto from_base64() noexcept
	{
		constexpr auto data = std::string_view{ T{}.get() };
		auto result = std::array<char, (data.size() / 4) * 3>{};

		for (auto [i, it] = std::pair{ size_t{}, result.begin() }; i < data.size(); i += 4)
		{
			auto const b0 = base64_table.find(data[i]);
			auto const b1 = (i + 1 < data.size()) ? base64_table.find(data[i + 1]) : 0;
			auto const b2 = (i + 2 < data.size()) ? base64_table.find(data[i + 2]) : 0;
			auto const b3 = (i + 3 < data.size()) ? base64_table.find(data[i + 3]) : 0;

			auto const b0_idx = (b0 << 2) | (b1 >> 4);
			auto const b1_idx = ((b1 & 0b1111) << 4) | (b2 >> 2);
			auto const b2_idx = ((b2 & 0b11) << 6) | b3;

			*it++ = static_cast<char>(b0_idx);
			if (b2 != padding)
				*it++ = static_cast<char>(b1_idx);
			if (b3 != padding)
				*it++ = static_cast<char>(b2_idx);
		}

		return result;
	}

	template <my_type T>
	constexpr auto from_hex() noexcept
	{
		constexpr auto data = std::string_view{ T{}.get() };
		auto result = std::array<char, data.size() / 2>{};

		for (auto [i, it] = std::pair{ size_t{}, result.begin() }; i < data.size(); i += 2)
		{
			auto const b0 = my_hex_table.find(data[i]);
			auto const b1 = my_hex_table.find(data[i + 1]);

			*it++ = static_cast<char>((b0 << 4) | b1);
		}

		return result;
	}
}

int main()
{
	struct my_data
	{
		constexpr auto get() const noexcept
		{
			return "TheMR";		// Plain
		}
	};

	constexpr auto b64 = code::to_base64<my_data>();
	constexpr auto hex = code::to_hex<my_data>();
	std::println("{} | {}", std::string_view{ b64 }, std::string_view{ hex });
}