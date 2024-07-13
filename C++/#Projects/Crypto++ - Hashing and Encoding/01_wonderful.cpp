#include <print>
#include <cryptopp/sha.h>
#include <cryptopp/md5.h>
#include <cryptopp/hex.h>
#include <cryptopp/base64.h>
#include <botan/base64.h>
using byte_type = CryptoPP::byte;

namespace convert
{
	template <class T> requires std::convertible_to<T*, CryptoPP::BufferedTransformation*>
	auto _convert(std::string_view input_data) noexcept
	{
		auto output = std::string{};
		CryptoPP::StringSource{ 
			reinterpret_cast<const byte_type*>(input_data.data()), input_data.length(), true,
			new T{ new CryptoPP::StringSink{ output } }
		};
		return output;
	}

	constexpr auto to_hex = _convert<CryptoPP::HexEncoder>;
	constexpr auto from_hex = _convert<CryptoPP::HexDecoder>;
	constexpr auto to_base64 = _convert<CryptoPP::Base64Encoder>;
	constexpr auto from_base64 = _convert<CryptoPP::Base64Decoder>;
}

namespace hash
{
	template <class Algo>
	auto _digest(std::string_view input_data) noexcept
	{
		auto output = std::string(Algo::DIGESTSIZE, '\0');
		Algo{}.CalculateDigest(
			reinterpret_cast<byte_type*>(output.data()),
			reinterpret_cast<const byte_type*>(input_data.data()), input_data.length()
		);
		return output;
	}

	constexpr auto md_v005 = _digest<CryptoPP::MD5>;
	constexpr auto sha_160 = _digest<CryptoPP::SHA1>;
	constexpr auto sha_224 = _digest<CryptoPP::SHA224>;
	constexpr auto sha_256 = _digest<CryptoPP::SHA256>;
	constexpr auto sha_384 = _digest<CryptoPP::SHA384>;
	constexpr auto sha_512 = _digest<CryptoPP::SHA512>;
}

int main()
{
	static constexpr auto my_data = std::string_view{ "TheMR" };
}
