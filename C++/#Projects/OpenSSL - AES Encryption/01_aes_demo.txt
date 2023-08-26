#include <openssl/evp.h>
#include <ranges>
#include <random>
#include <print>
namespace rg = std::ranges;
namespace vs = rg::views;
using byte_t = std::uint8_t;

// namespace convert ...

namespace crypto
{
	auto generate_key(const size_t length) noexcept
	{
		auto random_number_g = []() noexcept -> byte_t
		{
			static auto my_engine = std::mt19937_64{ std::random_device{}() };
			return std::uniform_int_distribution<uint16_t>{ 0, 255 }(my_engine);
		};
		auto my_key = std::string(length, '\0');
		rg::generate(my_key, random_number_g);
		return my_key;
	}

	namespace encrypt
	{
		auto aes_256_ecb(std::string_view input, std::string_view key) noexcept
		{
			auto the_state = EVP_CIPHER_CTX_new();
			auto my_output = std::string(input.size() + EVP_MAX_BLOCK_LENGTH, '\0');
			auto out_len_i = int{};
			auto out_len_f = int{};
			auto aes = EVP_aes_256_ecb();

			EVP_EncryptInit_ex(the_state, aes, nullptr, reinterpret_cast<const byte_t*>(key.data()), nullptr);
			EVP_EncryptUpdate(the_state, reinterpret_cast<byte_t*>(my_output.data()), &out_len_i, reinterpret_cast<const byte_t*>(input.data()), (int)input.size());
			EVP_EncryptFinal_ex(the_state, reinterpret_cast<byte_t*>(my_output.data()) + out_len_i, &out_len_f);
			EVP_CIPHER_CTX_free(the_state);

			my_output.resize(size_t(out_len_i + out_len_f));
			return my_output;
		}

		auto aes_256_ecb(std::string_view input) noexcept
		{
			auto aes = EVP_aes_256_ecb();
			auto key = generate_key(EVP_CIPHER_key_length(aes));
			return std::pair{ aes_256_ecb(input, key), key };
		}
	}

	namespace decrypt
	{
		auto aes_256_ecb(std::string_view input, std::string_view key) noexcept
		{
			auto the_state = EVP_CIPHER_CTX_new();
			auto my_output = std::string(input.size() + EVP_MAX_BLOCK_LENGTH, '\0');
			auto out_len_i = int{};
			auto out_len_f = int{};
			auto aes = EVP_aes_256_ecb();

			EVP_DecryptInit_ex(the_state, aes, nullptr, reinterpret_cast<const byte_t*>(key.data()), nullptr);
			EVP_DecryptUpdate(the_state, reinterpret_cast<byte_t*>(my_output.data()), &out_len_i, reinterpret_cast<const byte_t*>(input.data()), (int)input.size());
			EVP_DecryptFinal_ex(the_state, reinterpret_cast<byte_t*>(my_output.data()) + out_len_i, &out_len_f);
			EVP_CIPHER_CTX_free(the_state);

			my_output.resize(size_t(out_len_i + out_len_f));
			return my_output;
		}
	}
}

int main()
{
	static constexpr auto my_data = std::string_view{ "Hi TheMR" };
	static auto [encrypted, mkey] = crypto::encrypt::aes_256_ecb(my_data);
	static auto decrypted = crypto::decrypt::aes_256_ecb(encrypted, mkey);

	std::println("Original  : {}", my_data);
	std::println("Encrypted : {}", convert::to_base64(encrypted));
	std::println("Key       : {}", convert::to_base64(mkey));
	std::println("Decrypted : {}", decrypted);
}
