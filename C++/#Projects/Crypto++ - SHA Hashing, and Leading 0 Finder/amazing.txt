#include <print>
#include <random>
#include <cryptopp/sha.h>
#include <cryptopp/hex.h>
#include <bitset>
#include <chrono>
using namespace std::literals;
using my_type = std::size_t;

auto random_string(my_type length)
{
    auto prefix = "TheMR - "sv;
    auto char_m = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789"sv;
    
    static auto my_engine = std::mt19937_64{ std::random_device{}() };
    auto distribution = std::uniform_int_distribution<my_type>(0, char_m.size() - 1);
    
    auto result = std::string();
    result.reserve(length);
    result.append(prefix);
    
    std::generate_n(std::back_inserter(result), length - prefix.length(), [&]() { return char_m[distribution(my_engine)]; });

    return result;
}

template <typename T>
auto SHA(std::string_view input)
{
    CryptoPP::byte digest[T::DIGESTSIZE];
	auto output = std::string{};

	T{}.CalculateDigest(digest, (CryptoPP::byte*)input.data(), input.size());
    auto encoder = CryptoPP::HexEncoder{};
	encoder.Attach(new CryptoPP::StringSink(output));
	encoder.Put(digest, sizeof(digest));
	encoder.MessageEnd();
	return output;
}

template <char C>
auto count_leading(std::string_view hex) 
{
    my_type count = 0;
    for (char c : hex) 
    {
        if (c != C)
			break;
        ++count;
    }
    return count;
}

int main() 
{
    static constexpr my_type length = my_type{ 32 };
    static my_type maxZeros = 0;

    while (true) 
    {
        auto random_data = random_string(length);

        auto sha160Hash = SHA<CryptoPP::SHA1>(random_data);
        auto sha256Hash = SHA<CryptoPP::SHA256>(random_data);
        auto sha512Hash = SHA<CryptoPP::SHA512>(random_data);

        auto count = count_leading<'0'>;
        auto sha160Zeros = count(sha160Hash);
        auto sha256Zeros = count(sha256Hash);
        auto sha512Zeros = count(sha512Hash);

        if (auto gotZeros = std::max({ sha160Zeros, sha256Zeros, sha512Zeros }); gotZeros > maxZeros)
        {
            maxZeros = gotZeros;

            std::println("New record found at {:%c}", std::chrono::system_clock::now());
            std::println("Zeros : {} - SHA-1   : {}", sha160Zeros, sha160Hash);
            std::println("Zeros : {} - SHA-256 : {}", sha256Zeros, sha256Hash);
            std::println("Zeros : {} - SHA-512 : {}", sha512Zeros, sha512Hash);
            std::println("Data  : {}\n", random_data);
        }
    }
}