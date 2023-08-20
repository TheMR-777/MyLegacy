#include <print>
#include <cryptopp/sha.h>
#include <cryptopp/md5.h>
#include <cryptopp/hex.h>
#include <cryptopp/base64.h>
using byte_t = CryptoPP::byte;

namespace hash
{
	template <class Algo>
	concept hash_algo = requires
	{
		Algo::DIGESTSIZE;
		Algo{}.CalculateDigest((byte_t*)nullptr, (const byte_t*)nullptr, size_t{});
	};

	template <class Attachment>
	concept attachment = std::convertible_to<Attachment*, CryptoPP::BufferedTransformation*>;

	template <hash_algo Algo, attachment Encoder>
	auto _digest_output(std::string_view input_data) noexcept
	{
		auto m_hash = Algo{};
		auto output = std::string{};
		CryptoPP::StringSource
		{
			input_data.data(), true, new CryptoPP::HashFilter
			{
				m_hash, new Encoder
				{
					new CryptoPP::StringSink{ output }, false
				}
			}
		};
		return output;
	}

	template <hash_algo Algo>
	constexpr auto _digest = _digest_output<Algo, CryptoPP::Base64Encoder>;
	// constexpr auto _digest = _digest_output<Algo, CryptoPP::HexEncoder>;

	constexpr auto md_v005 = _digest<CryptoPP::MD5>;
	constexpr auto sha_160 = _digest<CryptoPP::SHA1>;
	constexpr auto sha_224 = _digest<CryptoPP::SHA224>;
	constexpr auto sha_256 = _digest<CryptoPP::SHA256>;
	constexpr auto sha_384 = _digest<CryptoPP::SHA384>;
	constexpr auto sha_512 = _digest<CryptoPP::SHA512>;
}

int main()
{
	static constexpr auto my_data = std::string_view{ "TheMR - gwzD0C8lZ5GRoDYSfDJMgcYI" };

	std::println("MD5:     {}", hash::md_v005(my_data));
	std::println("SHA-160: {}", hash::sha_160(my_data));
	std::println("SHA-224: {}", hash::sha_224(my_data));
	std::println("SHA-256: {}", hash::sha_256(my_data));
	std::println("SHA-384: {}", hash::sha_384(my_data));
	std::println("SHA-512: {}", hash::sha_512(my_data));
}
