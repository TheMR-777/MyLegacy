#include <openssl/evp.h>
#include <omp.h>
#include <random>
#include <bitset>
#include <chrono>
#include <print>
#include <array>
using namespace std::literals;
using my_type = std::size_t;

namespace detail
{
    using byte_ty = unsigned char;

    auto to_hex(std::string_view data) noexcept
    {
        static constexpr auto my_hex_table = std::string_view{ "0123456789abcdef" };

        auto result = std::string{};
        result.reserve(data.size() * 2);

        for (auto const b : data)
        {
            auto const b0 = static_cast<byte_ty>(b) >> 4;
            auto const b1 = static_cast<byte_ty>(b) & 0b1111;

            result += my_hex_table[b0];
            result += my_hex_table[b1];
        }

        return result;
    }

    auto random_string(my_type length)
    {
        static constexpr auto prefix = "TheMR - "sv;
        static constexpr auto char_m = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789"sv;

        static auto my_engine = std::mt19937_64{ std::random_device{}() };
        auto distribution = std::uniform_int_distribution<my_type>(0, char_m.size() - 1);

        auto result = std::string{ prefix };
        result.reserve(length);

        std::generate_n(std::back_inserter(result), length - prefix.length(), [&]() { return char_m[distribution(my_engine)]; });

        return result;
    }

    template <const EVP_MD* (*evp_x)()>
    auto SHA(std::string_view input)
    {
        auto output = std::string(EVP_MAX_MD_SIZE, '\0');
        auto out_ln = unsigned{};

        EVP_Digest(input.data(), input.size(), (byte_ty*)output.data(), &out_ln, evp_x(), nullptr);
        output.resize(out_ln);

        return to_hex(output);
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
}

int main()
{
    static constexpr my_type length = my_type{ 32 };
    static my_type maxZeros = 0;

    while (true)
    {
#pragma omp parallel // Create a parallel region
        {
            auto random_data = detail::random_string(length); // Generate a random string

            const auto hashes = std::array
            {
                detail::SHA<EVP_sha1>(random_data),
                detail::SHA<EVP_sha224>(random_data),
                detail::SHA<EVP_sha256>(random_data),
                detail::SHA<EVP_sha384>(random_data),
                detail::SHA<EVP_sha512>(random_data)
            };

            const auto zeroes = std::array
            {
                detail::count_leading<'0'>(hashes[0]),
                detail::count_leading<'0'>(hashes[1]),
                detail::count_leading<'0'>(hashes[2]),
                detail::count_leading<'0'>(hashes[3]),
                detail::count_leading<'0'>(hashes[4])
            };

            if (auto gotZeros = std::max(std::initializer_list<my_type>(zeroes.begin()._Unwrapped(), zeroes.end()._Unwrapped())); gotZeros > maxZeros)
            {
#pragma omp critical // Protect the shared variables
                {
                    if (gotZeros > maxZeros) // Check again to avoid race condition
                    {
                        maxZeros = gotZeros;
                        random_data = random_data; // Update the shared variable

                        std::println("New record found at {:%c}", std::chrono::system_clock::now());
                        std::println("Max Zeroes : {}", maxZeros);
                        std::println("-------------------------------------");
                        std::println("Data  : {}", random_data);
                        std::println("Zeros : {} - SHA-1   : {}", zeroes[0], hashes[0]);
                        std::println("Zeros : {} - SHA-224 : {}", zeroes[1], hashes[1]);
                        std::println("Zeros : {} - SHA-256 : {}", zeroes[2], hashes[2]);
                        std::println("Zeros : {} - SHA-384 : {}", zeroes[3], hashes[3]);
                        std::println("Zeros : {} - SHA-512 : {}", zeroes[4], hashes[4]);
                        std::println("");
                    }
                }
            }
        }
    }

}