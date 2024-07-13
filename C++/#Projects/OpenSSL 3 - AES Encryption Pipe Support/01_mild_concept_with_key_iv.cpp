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
		auto const o_size = ((4 * input.size() / 3) + 3) & ~3;
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

		for (byte_t const b : data)
		{
			*it_res++ = my_hex_table[b >> 4];
			*it_res++ = my_hex_table[b & 0x0F];
		}

		return result;
	}
}

namespace adapter
{
	template <std::string(*just_fun)(std::string_view)>
	struct simple_base : std::ranges::range_adaptor_closure<simple_base<just_fun>>
	{
		auto operator()(std::string_view input) const noexcept
		{
			return just_fun(input);
		}
	};

	constexpr auto to_base64 = simple_base<convert::to_base64>{};
	constexpr auto to_hex = simple_base<convert::to_hex>{};
}

namespace mr_crypto
{
	namespace detail
	{
		template <const EVP_CIPHER* (*cipher_x)(), bool to_encrypt = true>
		auto cipher(std::string_view input, std::string_view key, std::string_view iv) noexcept
		{
			auto output = std::string(input.size() + EVP_MAX_BLOCK_LENGTH, '\0');
			auto it_out = reinterpret_cast<byte_t*>(output.data());
			auto out_in = int{};
			auto out_fn = int{};
			{
				constexpr auto init = to_encrypt ? EVP_EncryptInit : EVP_DecryptInit;
				constexpr auto ping = to_encrypt ? EVP_EncryptUpdate : EVP_DecryptUpdate;
				constexpr auto ends = to_encrypt ? EVP_EncryptFinal : EVP_DecryptFinal;
				auto ctx = EVP_CIPHER_CTX_new();
				init(ctx, cipher_x(), reinterpret_cast<const byte_t*>(key.data()), reinterpret_cast<const byte_t*>(iv.data()));
				ping(ctx, it_out, &out_in, reinterpret_cast<const byte_t*>(input.data()), input.size());
				ends(ctx, it_out + out_in, &out_fn);
				EVP_CIPHER_CTX_free(ctx);
			}
			output.resize(out_in + out_fn);
			return output;
		}
	}

	auto random_bytes(const size_t bytes_n) noexcept
	{
		auto random_numbers_g = []() noexcept -> byte_t
			{
				static auto my_engine = std::mt19937_64{ std::random_device{ }() };
				return std::uniform_int_distribution<uint16_t>{ 0, 255 }(my_engine);
			};
		return vs::generate_n(random_numbers_g, bytes_n) | rg::to<std::string>;
	}

	struct aes_256_cbc
	{
		const std::string my_key = random_bytes(32);
		const std::string the_iv = random_bytes(16);

		template <bool to_encrypt = true>
		struct internal_adapter : std::ranges::range_adaptor_closure<internal_adapter<to_encrypt>>
		{
			std::string_view my_key;
			std::string_view the_iv;

			auto operator()(std::string_view input) const noexcept
			{
				return detail::cipher<EVP_aes_256_cbc, to_encrypt>(input, my_key, the_iv);
			}
		};

		auto encrypt() const noexcept
		{
			return internal_adapter<true>
			{
				.my_key = my_key,
					.the_iv = the_iv,
			};
		}

		auto decrypt() const noexcept
		{
			return internal_adapter<false>
			{
				.my_key = my_key,
					.the_iv = the_iv,
			};
		}
	};
}

int main()
{
	constexpr auto my_data = std::string_view{ "TheMR" };
	const auto my_aes_engine = mr_crypto::aes_256_cbc{ };
	const auto m_enc = my_data | my_aes_engine.encrypt();
	constexpr auto readable_conversion = adapter::to_hex;

	std::println("Original  : {}", my_data);
	std::println("Key       : {}", my_aes_engine.my_key | readable_conversion);
	std::println("IV        : {}", my_aes_engine.the_iv | readable_conversion);
	std::println("Encrypted : {}", m_enc | readable_conversion);
	std::println("Decrypted : {}", m_enc | my_aes_engine.decrypt());
}
