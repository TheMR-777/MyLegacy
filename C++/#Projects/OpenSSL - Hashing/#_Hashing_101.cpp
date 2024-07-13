#include <openssl/evp.h>
#include <fmt/ranges.h>
#include <ranges>
#include <array>
#include <span>
namespace rg = std::ranges;
namespace vs = rg::views;

namespace hash
{
	using value_type = unsigned char;

	template <const EVP_MD* (*const MD)()>
	auto digest(std::string_view data) noexcept
	{
		const auto digest = MD();
		const auto md_len = static_cast<size_t>(EVP_MD_size(digest));
		value_type md_val [EVP_MAX_MD_SIZE] = {};

		EVP_Digest(data.data(), data.size(), md_val, nullptr, digest, nullptr);

		return std::span{ md_val, md_len }
			| vs::transform([](auto byte) { return fmt::format("{:02x}", byte); })
			| vs::join
			| rg::to<std::string>();
	}

	constexpr auto MD_v05 = digest<EVP_md5>;
	constexpr auto SHA160 = digest<EVP_sha1>;
	constexpr auto SHA224 = digest<EVP_sha224>;
	constexpr auto SHA256 = digest<EVP_sha256>;
	constexpr auto SHA384 = digest<EVP_sha384>;
	constexpr auto SHA512 = digest<EVP_sha512>;
}

int main()
{
	static constexpr auto my_data = "TheMR";
	static constexpr auto my_hash = std::array
	{
		hash::MD_v05,
		hash::SHA160,
		hash::SHA224,
		hash::SHA256,
		hash::SHA384,
		hash::SHA512,
	};

	for (auto [x, hash] : my_hash | vs::transform([](auto hash) { return hash(my_data); }) | vs::enumerate)
		fmt::println("{} : {}", x + 1, hash);
}