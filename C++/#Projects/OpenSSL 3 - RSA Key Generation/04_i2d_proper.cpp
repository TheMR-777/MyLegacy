#include <openssl/evp.h>
#include <openssl/pem.h>
#include <print>

auto random_bytes_rsa(const size_t bits_n)
{
	auto key_loc = std::unique_ptr<EVP_PKEY, decltype(&EVP_PKEY_free)>(nullptr, EVP_PKEY_free);
	{
		auto key_ctx = std::unique_ptr<EVP_PKEY_CTX, decltype(&EVP_PKEY_CTX_free)>(EVP_PKEY_CTX_new_id(EVP_PKEY_RSA, nullptr), &EVP_PKEY_CTX_free);
		EVP_PKEY_keygen_init(key_ctx.get());
		EVP_PKEY_CTX_set_rsa_keygen_bits(key_ctx.get(), bits_n);
		EVP_PKEY_generate(key_ctx.get(), std::out_ptr(key_loc));
	}

	auto keys = std::pair
	{
		std::string(i2d_PrivateKey(key_loc.get(), nullptr), '\0'),
		std::string(i2d_PUBKEY(key_loc.get(), nullptr), '\0'),
	};

	auto pvt_buf = reinterpret_cast<mr_crypt::byte_t*>(keys.first.data());
	auto pub_buf = reinterpret_cast<mr_crypt::byte_t*>(keys.second.data());

	i2d_PrivateKey(key_loc.get(), &pvt_buf);
	i2d_PUBKEY(key_loc.get(), &pub_buf);

	return keys;
}
