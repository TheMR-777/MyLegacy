#include <openssl/sha.h>
#include <ranges>
#include <print>
namespace rg = std::ranges;
namespace vs = rg::views;

auto SHA_256(std::string_view input) 
{
	using openssl_value_type = unsigned char;

	openssl_value_type my_buffer[SHA256_DIGEST_LENGTH];
	SHA256(reinterpret_cast<const openssl_value_type*>(input.data()), input.size(), my_buffer);
	return my_buffer | vs::transform([](openssl_value_type c) { return std::format("{:02x}", c); }) | vs::join | rg::to<std::string>();
}

int main()
{
	constexpr auto the_data = "TheMR-1853112305";
	const auto sha_256_hash = SHA_256(the_data);
	std::println("Hash: {}", sha_256_hash);
}
