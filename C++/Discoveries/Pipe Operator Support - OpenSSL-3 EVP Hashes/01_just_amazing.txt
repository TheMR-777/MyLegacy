#include <openssl/evp.h>
#include <ranges>
#include <print>
namespace rg = std::ranges;
namespace vs = rg::views;

namespace convert
{
	using byte_t = std::uint8_t;

	static constexpr auto base64_table = std::string_view{ "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/" };
	static constexpr auto my_hex_table = std::string_view{ "0123456789abcdef" };
	static constexpr auto padding = '=';

	std::string to_base64(std::string_view input) noexcept
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

namespace hash
{
	template <const EVP_MD* (*const evp_x)()>
	auto digest(std::string_view input) noexcept
	{
		auto digest = evp_x();
		auto output = std::string(EVP_MD_size(digest), '\0');
		EVP_Digest(input.data(), input.size(), reinterpret_cast<convert::byte_t*>(output.data()), nullptr, digest, nullptr);
		return output;
	}
}

namespace evp_adapter
{
	inline namespace detail
	{
		template <std::string(* const just_practice)(std::string_view)>
		struct base : rg::range_adaptor_closure<base<just_practice>>
		{
			auto operator()(std::string_view input_range) const noexcept { return just_practice(input_range); }
		};

		template <const EVP_MD* (* const hash_x)()>
		inline static constexpr auto hash = base<hash::digest<hash_x>>{};
	}

	constexpr auto to_hex = base<convert::to_hex>{};
	constexpr auto to_base64 = base<convert::to_base64>{};

	constexpr auto sm_3 = hash<EVP_sm3>;
	constexpr auto md_5 = hash<EVP_md5>;
	constexpr auto md_5_sha160 = hash<EVP_md5_sha1>;
	constexpr auto shake_128 = hash<EVP_shake128>;
	constexpr auto shake_256 = hash<EVP_shake256>;
	constexpr auto ripe_md160 = hash<EVP_ripemd160>;
	constexpr auto blake_2s256 = hash<EVP_blake2s256>;
	constexpr auto blake_2b512 = hash<EVP_blake2b512>;

	constexpr auto sha_160 = hash<EVP_sha1>;
	constexpr auto sha_224 = hash<EVP_sha224>;
	constexpr auto sha_256 = hash<EVP_sha256>;
	constexpr auto sha_384 = hash<EVP_sha384>;
	constexpr auto sha_512 = hash<EVP_sha512>;
	constexpr auto sha3_224 = hash<EVP_sha3_224>;
	constexpr auto sha3_256 = hash<EVP_sha3_256>;
	constexpr auto sha3_384 = hash<EVP_sha3_384>;
	constexpr auto sha3_512 = hash<EVP_sha3_512>;
	constexpr auto sha_512_224 = hash<EVP_sha512_224>;
	constexpr auto sha_512_256 = hash<EVP_sha512_256>;
}

int main()
{
	using namespace evp_adapter;
	static constexpr auto my_data = std::string_view{ "TheMR" };
	static const auto secure_data = my_data
		| sha_512_256		| sha_512_224
		| sha3_512			| sha3_384
		| sha3_256			| sha3_224
		| sha_512			| sha_384
		| sha_256			| sha_224
		| sha_160			| blake_2b512		| blake_2s256
		| ripe_md160		| shake_256			| shake_128
		| md_5_sha160		| md_5				| to_hex;
	std::println("{} - {}", my_data, secure_data);
}
