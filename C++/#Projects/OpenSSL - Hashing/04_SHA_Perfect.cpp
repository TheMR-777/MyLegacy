#include <openssl/sha.h>
#include <fmt/ranges.h>
#include <ranges>
#include <array>
#include <span>
namespace rg = std::ranges;
namespace vs = rg::views;

namespace hash
{
	using value_type = unsigned char;

	template <size_t DigestLength, value_type* (*SHA_X)(const value_type*, size_t, value_type*)>
	auto SHA_Internal(std::string_view data) noexcept
	{
		return std::span<value_type, DigestLength>
		{
			SHA_X(reinterpret_cast<const value_type*>(data.data()), data.size(), nullptr),
			DigestLength
		}
		| vs::transform([](const auto byte) { return fmt::format("{:02x}", byte); })
		| vs::join
		| rg::to<std::string>();
	}

	const auto SHA_160 = SHA_Internal<SHA_DIGEST_LENGTH, SHA1>;
	const auto SHA_224 = SHA_Internal<SHA224_DIGEST_LENGTH, SHA224>;
	const auto SHA_256 = SHA_Internal<SHA256_DIGEST_LENGTH, SHA256>;
	const auto SHA_384 = SHA_Internal<SHA384_DIGEST_LENGTH, SHA384>;
	const auto SHA_512 = SHA_Internal<SHA512_DIGEST_LENGTH, SHA512>;
}

int main()
{
	static constexpr auto my_data = "TheMR - gwzD0C8lZ5GRoDYSfDJMgcYI";
	static const auto hashing_alg = std::array
	{ 
		hash::SHA_160, 
		hash::SHA_224, 
		hash::SHA_256, 
		hash::SHA_384, 
		hash::SHA_512 
	};

	for (auto [x, hash] : hashing_alg | vs::enumerate)
	{
		fmt::print("{}: {}\n", x + 1, hash(my_data));
	}
}