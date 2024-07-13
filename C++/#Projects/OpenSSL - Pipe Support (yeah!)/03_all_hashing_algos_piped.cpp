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
	template <const EVP_MD* (*evp_x)()>
	auto digest(std::string_view input) noexcept
	{
		auto digest = evp_x();
		auto output = std::string(EVP_MD_size(digest), '\0');
		EVP_Digest(input.data(), input.size(), reinterpret_cast<convert::byte_t*>(output.data()), nullptr, digest, nullptr);
		return output;
	}
}

namespace my_adapter
{
	template <std::string (*just_fun)(std::string_view)>
	struct base : rg::range_adaptor_closure<base<just_fun>>
	{
		auto operator()(std::string_view input) const noexcept
		{
			return just_fun(input);
		}
	};

	constexpr auto to_hex = base<convert::to_hex>{};
	constexpr auto to_base64 = base<convert::to_base64>{};

	constexpr auto md4 = base<hash::digest<EVP_md4>>{};
	constexpr auto md5 = base<hash::digest<EVP_md5>>{};
	constexpr auto md5_sha160 = base<hash::digest<EVP_md5_sha1>>{};
	constexpr auto mdc2 = base<hash::digest<EVP_mdc2>>{};
	constexpr auto ripe_md160 = base<hash::digest<EVP_ripemd160>>{};
	constexpr auto whirlpool = base<hash::digest<EVP_whirlpool>>{};
	constexpr auto sm3 = base<hash::digest<EVP_sm3>>{};
	constexpr auto blake_2s256 = base<hash::digest<EVP_blake2s256>>{};
	constexpr auto blake_2b512 = base<hash::digest<EVP_blake2b512>>{};
	constexpr auto shake_128 = base<hash::digest<EVP_shake128>>{};
	constexpr auto shake_256 = base<hash::digest<EVP_shake256>>{};

	constexpr auto sha_160 = base<hash::digest<EVP_sha1>>{};
	constexpr auto sha_224 = base<hash::digest<EVP_sha224>>{};
	constexpr auto sha_256 = base<hash::digest<EVP_sha256>>{};
	constexpr auto sha_384 = base<hash::digest<EVP_sha384>>{};
	constexpr auto sha_512 = base<hash::digest<EVP_sha512>>{};
	constexpr auto sha_512_224 = base<hash::digest<EVP_sha512_224>>{};
	constexpr auto sha_512_256 = base<hash::digest<EVP_sha512_256>>{};
	constexpr auto sha3_224 = base<hash::digest<EVP_sha3_224>>{};
	constexpr auto sha3_256 = base<hash::digest<EVP_sha3_256>>{};
	constexpr auto sha3_384 = base<hash::digest<EVP_sha3_384>>{};
	constexpr auto sha3_512 = base<hash::digest<EVP_sha3_512>>{};

}

int main()
{
	static constexpr auto my_data = std::string_view{ "TheMR" };
	{
		using namespace my_adapter;
		static constexpr auto into = to_base64;

		std::println("Hex:    {}", my_data | to_hex);
		std::println("Base64: {}", my_data | to_base64);
		std::println(" ");
		std::println("MD4:         {}", my_data | md4 | into);
		std::println("MD5:         {}", my_data | md5 | into);
		std::println("MD5-SHA160:  {}", my_data | md5_sha160 | into);
		std::println("MDC2:        {}", my_data | mdc2 | into);
		std::println("Ripe-MD160:  {}", my_data | ripe_md160 | into);
		std::println("Whirlpool:   {}", my_data | whirlpool | into);
		std::println("SM3:         {}", my_data | sm3 | into);
		std::println("Blake-2s256: {}", my_data | blake_2s256 | into);
		std::println("Blake-2b512: {}", my_data | blake_2b512 | into);
		std::println("Shake-128:   {}", my_data | shake_128 | into);
		std::println("Shake-256:   {}", my_data | shake_256 | into);
		std::println(" ");
		std::println("SHA-160:     {}", my_data | sha_160 | into);
		std::println("SHA-224:     {}", my_data | sha_224 | into);
		std::println("SHA-256:     {}", my_data | sha_256 | into);
		std::println("SHA-384:     {}", my_data | sha_384 | into);
		std::println("SHA-512:     {}", my_data | sha_512 | into);
		std::println("SHA-512/224: {}", my_data | sha_512_224 | into);
		std::println("SHA-512/256: {}", my_data | sha_512_256 | into);
		std::println("SHAv3-224:   {}", my_data | sha3_224 | into);
		std::println("SHAv3-256:   {}", my_data | sha3_256 | into);
		std::println("SHAv3-384:   {}", my_data | sha3_384 | into);
		std::println("SHAv3-512:   {}", my_data | sha3_512 | into);
	}
}
