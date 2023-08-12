#include <openssl/evp.h>
#include <openssl/rand.h>
#include <fmt/ranges.h>
#include <charconv>
#include <ranges>
#include <array>
#include <span>
namespace rg = std::ranges;
namespace vs = rg::views;

/* Encoding wrappers, using OpenSSL 3 */

namespace crypt 
{
	using value_type = unsigned char;

	// To std::string of Hex
	auto to_hex(std::basic_string_view<value_type> bytes)
	{
		return bytes
			| vs::transform([](auto b) { return fmt::format("{:02x}", b); }) 
			| vs::join
			| rg::to<std::string>();
	}

	auto to_hex(std::string_view data)
	{
		const auto new_data = std::span{ reinterpret_cast<const value_type*>(data.data()), data.size() };
		return to_hex(new_data);
	}

	// To std::string of Base64 (with EVP_EncodeBlock)
	auto to_base64(std::basic_string_view<value_type> bytes)
	{
		const auto final_size = EVP_ENCODE_LENGTH(bytes.size());
		auto result = std::string(final_size, '\0');
		EVP_EncodeBlock(reinterpret_cast<unsigned char*>(result.data()), bytes.data(), bytes.size());
		return result;
	}

	auto to_base64(std::string_view data)
	{
		const auto new_data = std::span{ reinterpret_cast<const value_type*>(data.data()), data.size() };
		return to_base64(new_data);
	}

	// From std::string_view of Hex
	auto from_hex(std::string_view hex)
	{
		return hex
			| vs::chunk(2)
			| vs::transform([](auto chunk) 
			{
				char c = 0;
				std::from_chars(chunk.data(), chunk.data() + chunk.size(), c, 0x10);
				return c;
			})
			| rg::to<std::string>();
	}

	// From std::string_view of Base64 (with EVP_DecodeBlock)
	auto from_base64(std::string_view base64)
	{
		const auto final_size = EVP_DECODE_LENGTH(base64.size());
		auto result = std::string(final_size, '\0');
		EVP_DecodeBlock(reinterpret_cast<value_type*>(result.data()), reinterpret_cast<const value_type*>(base64.data()), base64.size());
		return result;
	}
}


int main()
{
	static constexpr auto my_data = "TheMR";
	static const auto hex_encoded = crypt::to_hex(my_data);
	static const auto hex_decoded = crypt::from_hex(hex_encoded);
	static const auto in_base64_e = crypt::to_base64(my_data);
	static const auto in_base64_d = crypt::from_base64(in_base64_e);

	fmt::println("Original Data  : {}", my_data);
	fmt::println(" ");
	fmt::println("Hex Encoded    : {}", hex_encoded);
	fmt::println("Base64 Encoded : {}", in_base64_e);
	fmt::println(" ");
	fmt::println("Hex Decoded    : {}", hex_decoded);
	fmt::println("Base64 Decoded : {}", in_base64_d);
}