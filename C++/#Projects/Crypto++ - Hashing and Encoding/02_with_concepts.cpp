#include <print>
#include <cryptopp/sha.h>
#include <cryptopp/md5.h>
#include <cryptopp/hex.h>
#include <cryptopp/base64.h>
using byte_type = CryptoPP::byte;

namespace convert
{
	template <class T>
	concept transformer = requires
	{
		std::convertible_to<T*, CryptoPP::BufferedTransformation*>;
	};

	template <transformer T>
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
	concept hash_algo = requires
	{
		Algo::DIGESTSIZE;
		Algo{}.CalculateDigest((byte_type*)nullptr, (const byte_type*)nullptr, size_t{});
	};

	template <hash_algo Algo, std::string (*convert_func)(std::string_view)>
	auto _digest_output(std::string_view input_data) noexcept
	{
		auto output = std::string(Algo::DIGESTSIZE, '\0');
		Algo{}.CalculateDigest(
			reinterpret_cast<byte_type*>(output.data()),
			reinterpret_cast<const byte_type*>(input_data.data()), input_data.length()
		);
		return convert_func(output);
	}

	template <hash_algo Algo>
	constexpr auto _digest = _digest_output<Algo, convert::to_base64>;

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

	std::println("MD5:     {}", hash::md_v005(my_data));
	std::println("SHA-160: {}", hash::sha_160(my_data));
	std::println("SHA-224: {}", hash::sha_224(my_data));
	std::println("SHA-256: {}", hash::sha_256(my_data));
	std::println("SHA-384: {}", hash::sha_384(my_data));
	std::println("SHA-512: {}", hash::sha_512(my_data));
}
