#include <print>
#include <cryptopp/hex.h>
#include <cryptopp/base64.h>
using byte_type = CryptoPP::byte;

namespace convert
{
	template <class T> requires std::convertible_to<T*, CryptoPP::BufferedTransformation*>
	auto _convert(std::string_view input_data) noexcept
	{
		auto output = std::string{};
		CryptoPP::StringSource{ reinterpret_cast<const byte_type*>(input_data.data()), input_data.length(), true,
			new T{ new CryptoPP::StringSink{ output } }
		};
		return output;
	}

	constexpr auto to_hex = _convert<CryptoPP::HexEncoder>;
	constexpr auto from_hex = _convert<CryptoPP::HexDecoder>;
	constexpr auto to_base64 = _convert<CryptoPP::Base64Encoder>;
	constexpr auto from_base64 = _convert<CryptoPP::Base64Decoder>;
}

int main()
{
	static constexpr auto my_data = std::string_view{ "TheMR" };
	static const auto in_base64_e = convert::to_base64(my_data);
	static const auto in_base64_d = convert::from_base64(in_base64_e);
	static const auto the_hex_enc = convert::to_hex(my_data);
	static const auto the_hex_dec = convert::from_hex(the_hex_enc);

	std::println("Original Data : {}", my_data);
	std::println("Base64 Encode : {}", in_base64_e);
	std::println("Base64 Decode : {}", in_base64_d);
	std::println("Hex Encode    : {}", the_hex_enc);
	std::println("Hex Decode    : {}", the_hex_dec);
}
