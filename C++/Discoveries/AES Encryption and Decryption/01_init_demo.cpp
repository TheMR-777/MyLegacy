#include <range/v3/view.hpp>
#include <openssl/evp.h>
#include <random>
#include <ranges>
#include <print>
namespace rg = ranges;
namespace vs = rg::views;
using byte_t = std::uint8_t;

namespace convert
{
	static constexpr auto base64_table = std::string_view{ "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/" };
	static constexpr auto my_hex_table = std::string_view{ "0123456789abcdef" };
	static constexpr auto m_padding = '=';

	auto to_base64(std::string_view input) noexcept
	{
		auto o_size = ((4 * input.size() / 3) + 3) & ~3;
		auto output = std::string(o_size, '=');
		auto it_out = output.begin();

		for (size_t i = 0; i < input.size(); i += 3)
		{
			// Get the three bytes as an unsigned integer
			auto group = static_cast<byte_t>(input[i]) << 16;
			if (i + 1 < input.size())
			{
				group |= static_cast<byte_t>(input[i + 1]) << 8;
			}
			if (i + 2 < input.size())
			{
				group |= static_cast<byte_t>(input[i + 2]);
			}

			// Encode the four base64 characters from the group
			*it_out++ = base64_table[(group >> 18) & 0x3F];
			*it_out++ = base64_table[(group >> 12) & 0x3F];
			if (i + 1 < input.size())
			{
				*it_out++ = base64_table[(group >> 6) & 0x3F];
			}
			if (i + 2 < input.size())
			{
				*it_out++ = base64_table[group & 0x3F];
			}
		}

		return output;
	}

	auto to_hex(std::string_view data) noexcept
	{
		auto result = std::string(data.size() * 2, '\0');
		auto it_res = result.begin();

		for (auto const b : data)
		{
			auto const bx = static_cast<byte_t>(b);
			*it_res++ = my_hex_table[bx >> 4];
			*it_res++ = my_hex_table[bx & 0b1111];
		}

		return result;
	}
}

namespace crypto
{
	auto generate_key(const size_t bytes_n) noexcept
	{
		static auto random_numbers_g = []() -> byte_t
		{
			static auto my_engine = std::mt19937_64{ std::random_device{ }() };
			return std::uniform_int_distribution<uint16_t>{ 0,255 }(my_engine);
		};
		return vs::generate_n(random_numbers_g, bytes_n) | rg::to<std::string>;
	}

	namespace encrypt
	{
		auto aes_256_ecb(std::string_view input, std::string_view key) noexcept
		{
			auto m_output = std::string(input.size() + EVP_MAX_BLOCK_LENGTH, '\0');
			auto out_ln_i = int{};
			auto out_ln_f = int{};

			auto context = EVP_CIPHER_CTX_new();
			EVP_EncryptInit_ex(context, EVP_aes_256_ecb(), nullptr, reinterpret_cast<const byte_t*>(key.data()), nullptr);
			EVP_EncryptUpdate(context, reinterpret_cast<byte_t*>(m_output.data()), &out_ln_i, reinterpret_cast<const byte_t*>(input.data()), input.size());
			EVP_EncryptFinal_ex(context, reinterpret_cast<byte_t*>(m_output.data()) + out_ln_i, &out_ln_f);
			EVP_CIPHER_CTX_free(context);

			m_output.resize(out_ln_i + out_ln_f);
			return m_output;
		}

		auto aes_256_ecb(std::string_view input) noexcept -> std::pair<std::string, std::string>
		{
			const auto my_key = generate_key(EVP_CIPHER_key_length(EVP_aes_256_ecb()));
			return { my_key, aes_256_ecb(input, my_key) };
		}
	}

	namespace decrypt
	{
		auto aes_256_ecb(std::string_view input, std::string_view key) noexcept
		{
			auto m_output = std::string(input.size() + EVP_MAX_BLOCK_LENGTH, '\0');
			auto out_ln_i = int{};
			auto out_ln_f = int{};

			auto context = EVP_CIPHER_CTX_new();
			EVP_DecryptInit_ex(context, EVP_aes_256_ecb(), nullptr, reinterpret_cast<const byte_t*>(key.data()), nullptr);
			EVP_DecryptUpdate(context, reinterpret_cast<byte_t*>(m_output.data()), &out_ln_i, reinterpret_cast<const byte_t*>(input.data()), input.size());
			EVP_DecryptFinal_ex(context, reinterpret_cast<byte_t*>(m_output.data()) + out_ln_i, &out_ln_f);
			EVP_CIPHER_CTX_free(context);

			m_output.resize(out_ln_i + out_ln_f);
			return m_output;
		}
	}
}

namespace adapter
{
	template <std::string (*just_fun)(std::string_view)>
	struct base : std::ranges::range_adaptor_closure<base<just_fun>>
	{
		auto operator()(std::string_view input) const noexcept
		{
			return just_fun(input);
		}
	};

	constexpr auto to_hex = base<convert::to_hex>{};
	constexpr auto to_base64 = base<convert::to_base64>{};
}

int main()
{
	static constexpr auto m_data = "TheMR";
	static auto [key, encrypted] = crypto::encrypt::aes_256_ecb(m_data);
	static auto m_decrypted_data = crypto::decrypt::aes_256_ecb(encrypted, key);

	std::println("Original   : {}", m_data);
	std::println("Key        : {}", key | adapter::to_hex);
	std::println("Encrypted  : {}", encrypted | adapter::to_hex);
	std::println("Decrypted  : {}", m_decrypted_data);
}
