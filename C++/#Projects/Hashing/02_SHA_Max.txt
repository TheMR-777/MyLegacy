#include <openssl/sha.h>
#include <functional>
#include <ranges>
#include <print>
#include <array>
namespace rg = std::ranges;
namespace vs = rg::views;

namespace hash
{
	using openssl_value_type = unsigned char;

	template <size_t digest_length, auto* (*SHA)(const openssl_value_type*, size_t, openssl_value_type*)>
	auto SHA_Internal(std::string_view input) noexcept
	{
		return std::basic_string_view{ SHA(reinterpret_cast<const openssl_value_type*>(input.data()), input.size(), nullptr) }
			| vs::transform([](auto c) { return std::format("{:02x}", c); })
			| vs::join 
			| rg::to<std::string>();
	}

	auto SHA_160 = SHA_Internal<SHA_DIGEST_LENGTH, SHA1>;
	auto SHA_224 = SHA_Internal<SHA224_DIGEST_LENGTH, SHA224>;
	auto SHA_256 = SHA_Internal<SHA256_DIGEST_LENGTH, SHA256>;
	auto SHA_384 = SHA_Internal<SHA384_DIGEST_LENGTH, SHA384>;
	auto SHA_512 = SHA_Internal<SHA512_DIGEST_LENGTH, SHA512>;
}

int main()
{
	// TheMR - gwzD0C8lZ5GRoDYSfDJMgcYI
	constexpr auto the_data = "TheMR";
	const auto m_all_hashes = std::array
	{
		hash::SHA_160(the_data),
		hash::SHA_224(the_data),
		hash::SHA_256(the_data),
		hash::SHA_384(the_data),
		hash::SHA_512(the_data)
	};
	for (auto [x, hash] : m_all_hashes | vs::enumerate)
	{
		std::println("{}: {}", x + 1, hash);
	}
}
