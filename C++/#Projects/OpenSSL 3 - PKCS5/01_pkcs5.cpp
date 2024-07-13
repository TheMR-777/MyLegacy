#include <range/v3/view.hpp>
#include <openssl/evp.h>
#include <openssl/pem.h>
#include <random>
#include <ranges>
#include <print>

namespace mr_crypt ...

namespace pk_cs_5
{
	auto generate_key(std::string_view input, std::string_view salt = {}, const size_t key_bits = 256) noexcept
	{
		std::string key(key_bits / 8, '\0');
		PKCS5_PBKDF2_HMAC(input.data(), input.size(), reinterpret_cast<const unsigned char*>(salt.data()), salt.size(), 1000, EVP_sha256(), key_bits / 8, reinterpret_cast<unsigned char*>(key.data()));
		return key;
	}
}

int main()
{
	constexpr auto my_data = std::string_view{ "TheMR" };
	constexpr auto key_bit = 2 * 8 * 8;
	{
		std::println("Bits: {} - {}", key_bit, pk_cs_5::generate_key(my_data, {}, key_bit) | mr_crypt::convert::to_base64);
	}
}
