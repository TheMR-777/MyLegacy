#include <range/v3/view.hpp>
#include <openssl/evp.h>
#include <fmt/ranges.h>
#include <array>
#include <span>
namespace rg = ranges;
namespace vs = rg::views;

namespace hash
{
	using value_type = std::byte;

	template <const EVP_MD* (*evp_x)()>
	auto digest(std::string_view input_data) noexcept
	{
		std::underlying_type_t<value_type> output [EVP_MAX_MD_SIZE];
		unsigned int output_size = 0;

		EVP_Digest(input_data.data(), input_data.size(), output, &output_size, evp_x(), nullptr);

		return std::span{ output, output_size }
			| vs::transform([](auto byte) { return fmt::format("{:02x}", byte); })
			| vs::join
			| rg::to<std::string>;
	}

	constexpr auto md_sha = digest<EVP_md5_sha1>;
	constexpr auto md_v05 = digest<EVP_md5>;
	constexpr auto sha160 = digest<EVP_sha1>;
	constexpr auto sha224 = digest<EVP_sha224>;
	constexpr auto sha256 = digest<EVP_sha256>;
	constexpr auto sha384 = digest<EVP_sha384>;
	constexpr auto sha512 = digest<EVP_sha512>;
}

int main()
{
	constexpr auto input = "TheMR - gwzD0C8lZ5GRoDYSfDJMgcYI";

	fmt::println("md_sha: {}", hash::md_sha(input));
	fmt::println("md_v05: {}", hash::md_v05(input));
	fmt::println("sha160: {}", hash::sha160(input));
	fmt::println("sha224: {}", hash::sha224(input));
	fmt::println("sha256: {}", hash::sha256(input));
	fmt::println("sha384: {}", hash::sha384(input));
	fmt::println("sha512: {}", hash::sha512(input));
}