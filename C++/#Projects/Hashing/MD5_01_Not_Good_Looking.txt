#include <openssl/evp.h>
#include <fmt/ranges.h>
#include <ranges>
#include <array>
#include <span>
namespace rg = std::ranges;
namespace vs = rg::views;

namespace hash
{
	auto MD5(std::string_view data) noexcept
	{
		auto mode = EVP_md5();
		auto md5_ctx = EVP_MD_CTX_new();
		EVP_DigestInit_ex(md5_ctx, mode, nullptr);
		EVP_DigestUpdate(md5_ctx, data.data(), data.size());
		unsigned char md5_hash[EVP_MAX_MD_SIZE];
		unsigned int md5_hash_len;
		EVP_DigestFinal_ex(md5_ctx, md5_hash, &md5_hash_len);
		EVP_MD_CTX_free(md5_ctx);

		return std::span(md5_hash, md5_hash_len) 
			| vs::transform([](auto byte) { return fmt::format("{:02x}", byte); })
			| vs::join
			| rg::to<std::string>();
	}
}

int main()
{
	static constexpr auto my_data = "TheMR";
	fmt::println("MD5: {}", hash::MD5(my_data));
}