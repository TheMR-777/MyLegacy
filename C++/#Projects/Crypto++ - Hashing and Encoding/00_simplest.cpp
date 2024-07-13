#include <cryptopp/sha.h>
#include <cryptopp/hex.h>
#include <cryptopp/base64.h>
#include <fmt/ranges.h>
#include <print>
namespace crypto = CryptoPP;
using byte_t = crypto::byte;

namespace hash
{
	auto sha_256(std::string_view data) noexcept
	{
		auto output = std::string{};
		auto m_hash = crypto::SHA256{};
		crypto::StringSource
		{
			data.data(), true, new crypto::HashFilter
			{
				m_hash, new crypto::Base64Encoder
				{
					new crypto::StringSink{ output }, false
				}
			}
		};
		return output;
	}
}

int main()
{
	static constexpr auto my_data = std::string_view{ "TheMR" };
	fmt::println("Hash : {}", hash::sha_256(my_data));
}