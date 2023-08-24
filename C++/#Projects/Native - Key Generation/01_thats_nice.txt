#include <numeric>
#include <random>
#include <print>
using byte_t = uint8_t;
namespace rg = std::ranges;
namespace vs = rg::views;

// namespace convert ...

auto generate_key(const size_t length) noexcept
{
	auto random_number_gen = []() noexcept -> byte_t
	{
		static constexpr auto min = std::numeric_limits<byte_t>::min();
		static constexpr auto max = std::numeric_limits<byte_t>::max();
		static auto my_engine = std::mt19937{ std::random_device{}() };
		return std::uniform_int_distribution<uint16_t>{ min, max }(my_engine);
	};
	auto output_key = std::string(length, '\0');
	rg::generate(output_key, random_number_gen);
	return output_key;
}

int main()
{
	auto const key = generate_key(32);
	auto const key_hex = convert::to_hex(key);
	auto const key_base64 = convert::to_base64(key);

	std::println("Key Length   : {}", key.size());
	std::println("Key (hex)    : {}", key_hex);
	std::println("Key (base64) : {}", key_base64);
}
