#include <print>
#include <mr_crypt>

int main()
{
	constexpr auto my_data = std::string_view{ "TheMR-777" };
	constexpr auto my_size = my_data.size();

	// Stream Cipher Demo
	{
		const auto my_key = mr_crypt::pk_cs_5::pb_kdf2_hmac("Nice", my_size);
		const auto result = mr_crypt::vs::zip_with(std::bit_xor{}, my_data, my_key) | mr_crypt::rg::to<std::string>;
		const auto origin = mr_crypt::vs::zip_with(std::bit_xor{}, result, my_key) | mr_crypt::rg::to<std::string>;

		std::println("Encrypted: {}", result | mr_crypt::convert::to_hex);
		std::println("Decrypted: {}", origin);
	}
}
