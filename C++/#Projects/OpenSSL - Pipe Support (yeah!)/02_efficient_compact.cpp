#include <openssl/evp.h>
#include <ranges>
#include <print>
#include <array>
using namespace std::literals;
namespace rg = std::ranges;
namespace vs = rg::views;

namespace convert
{
	using byte_t = std::uint8_t;

	static constexpr auto base64_table = std::string_view{ "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/" };
	static constexpr auto my_hex_table = std::string_view{ "0123456789abcdef" };
	static constexpr auto padding = '=';

	auto to_base64(std::string_view data) noexcept
	{
		auto result = std::string(data.size() * 4 / 3 + 4, '\0');
		auto it_res = result.begin();

		auto it = data.begin();
		auto const end = data.end();

		while (it != end)
		{
			auto const b0 = static_cast<byte_t>(*it++);
			auto const b1 = (it != end) ? static_cast<byte_t>(*it++) : 0;
			auto const b2 = (it != end) ? static_cast<byte_t>(*it++) : 0;

			*it_res++ = base64_table[b0 >> 2];
			*it_res++ = base64_table[(size_t(b0 & 0b11) << 4) | (b1 >> 4)];
			*it_res++ = (b1 == 0) ? padding : base64_table[(size_t(b1 & 0b1111) << 2) | (b2 >> 6)];
			*it_res++ = (b2 == 0) ? padding : base64_table[b2 & 0b111111];
		}

		return result;
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
	auto digest(std::string_view data) noexcept
	{
		auto digest = evp_x();
		auto output = std::string(EVP_MD_get_size(digest), '\0');
		EVP_Digest(data.data(), data.size(), reinterpret_cast<convert::byte_t*>(output.data()), nullptr, digest, nullptr);
		return output;
	}
}

namespace my_views
{
	template <std::string (*const f)(std::string_view)>
	struct base : rg::range_adaptor_closure<base<f>>
	{
		auto operator()(std::string_view data) const noexcept { return f(data); }
	};

	constexpr auto to_base64 = base<convert::to_base64>{};
	constexpr auto to_hex = base<convert::to_hex>{};

	constexpr auto sha_160 = base<hash::digest<EVP_sha1>>{};
	constexpr auto sha_224 = base<hash::digest<EVP_sha224>>{};
	constexpr auto sha_256 = base<hash::digest<EVP_sha256>>{};
	constexpr auto sha_384 = base<hash::digest<EVP_sha384>>{};
	constexpr auto sha_512 = base<hash::digest<EVP_sha512>>{};
}

int main()
{
	static constexpr auto my_data = "TheMR - gwzD0C8lZ5GRoDYSfDJMgcYI"sv;
	static const auto my_hashed_d = my_data | my_views::sha_512 | my_views::to_hex;
	std::println("Original : {}", my_data);
	std::println("Hashed   : {}", my_hashed_d);
}
