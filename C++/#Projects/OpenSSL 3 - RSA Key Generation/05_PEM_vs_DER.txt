#include <range/v3/view.hpp>
#include <openssl/evp.h>
#include <openssl/pem.h>
#include <random>
#include <ranges>
#include <print>

// namespace mr_crypt...

namespace test
{
	auto random_bytes_rsa(const size_t bits_n) noexcept
	{
		auto key_loc = std::unique_ptr<EVP_PKEY, decltype(&EVP_PKEY_free)>(nullptr, EVP_PKEY_free);
		{
			auto key_ctx = std::unique_ptr<EVP_PKEY_CTX, decltype(&EVP_PKEY_CTX_free)>(EVP_PKEY_CTX_new_id(EVP_PKEY_RSA, nullptr), &EVP_PKEY_CTX_free);
			EVP_PKEY_keygen_init(key_ctx.get());
			EVP_PKEY_CTX_set_rsa_keygen_bits(key_ctx.get(), bits_n);
			EVP_PKEY_generate(key_ctx.get(), std::out_ptr(key_loc));
		}

		// RAW Keys:
		auto keys_dem = std::pair
		{
			std::string(i2d_PrivateKey(key_loc.get(), nullptr), '\0'),
			std::string(i2d_PUBKEY(key_loc.get(), nullptr), '\0'),
		};

		{
			auto pvt_buf = reinterpret_cast<mr_crypt::byte_t*>(keys_dem.first.data());
			auto pub_buf = reinterpret_cast<mr_crypt::byte_t*>(keys_dem.second.data());

			i2d_PrivateKey(key_loc.get(), &pvt_buf);
			i2d_PUBKEY(key_loc.get(), &pub_buf);
		}

		// PEM Keys:
		auto pvt_buf = std::unique_ptr<BIO, decltype(&BIO_free_all)>(BIO_new(BIO_s_mem()), &BIO_free_all);
		auto pub_buf = std::unique_ptr<BIO, decltype(&BIO_free_all)>(BIO_new(BIO_s_mem()), &BIO_free_all);
		{
			PEM_write_bio_PrivateKey(pvt_buf.get(), key_loc.get(), nullptr, nullptr, 0, nullptr, nullptr);
			PEM_write_bio_PUBKEY(pub_buf.get(), key_loc.get());
		}

		char* pvt_ptr = nullptr, * pub_ptr = pvt_ptr;
		auto pvt_len = BIO_get_mem_data(pvt_buf.get(), &pvt_ptr);
		auto pub_len = BIO_get_mem_data(pub_buf.get(), &pub_ptr);

		// Return Pair of: Pair of PEM Keys, Pair of RAW Keys
		return std::pair
		{
			std::pair
			{
				std::string(pvt_ptr, pvt_len),
				std::string(pub_ptr, pub_len),
			},
			std::move(keys_dem),
		};
	}
}

int main()
{
	constexpr auto my_data = std::string_view{ "TheMR" };
	{
		const auto& [key_pem, key_der] = test::random_bytes_rsa(2048);
		const auto& [pvt_pem, pub_pem] = key_pem;
		const auto& [pvt_der, pub_der] = key_der;

		std::println("{}", pvt_pem);
		std::println("{}", pub_pem);
		std::println("");
		std::println("DER Pvt-Key: \n{}", pvt_der | mr_crypt::convert::to_base64);
		std::println("");
		std::println("DER Pub-Key: \n{}", pub_der | mr_crypt::convert::to_base64);
	}
}