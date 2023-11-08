#include <range/v3/view.hpp>
#include <openssl/evp.h>
#include <openssl/pem.h>
#include <random>
#include <ranges>

// namespace mr_crypt ...

int main()
{
	constexpr auto my_data = std::string_view{ "TheMR" };
	{
		using namespace mr_crypt;
		using my_algo = supreme::aes_128_ctr<>;

		const auto& key_01 = my_algo::make_key_using_password(my_data);
		const auto& key_02 = my_algo::using_password<false>(my_data).my_key;
		const auto& key_03 = my_data | convert::to_key(my_algo::key_size());
		const auto& key_04 = my_data | pk_cs_5::as_key(my_algo::key_size());
	}
}
