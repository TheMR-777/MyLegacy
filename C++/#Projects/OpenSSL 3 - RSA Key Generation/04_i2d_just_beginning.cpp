#include <openssl/bio.h>
#include <openssl/evp.h>
#include <openssl/pem.h>
#include <fmt/ranges.h>
#include <vector>

auto createRSAPair(int bits)
{
    std::unique_ptr<EVP_PKEY, decltype(&EVP_PKEY_free)> key_loc(nullptr, EVP_PKEY_free);
    {
        std::unique_ptr<EVP_PKEY_CTX, decltype(&EVP_PKEY_CTX_free)> key_ctx(EVP_PKEY_CTX_new_id(EVP_PKEY_RSA, nullptr), &EVP_PKEY_CTX_free);
        EVP_PKEY_keygen_init(key_ctx.get());
        EVP_PKEY_CTX_set_rsa_keygen_bits(key_ctx.get(), bits);
        EVP_PKEY_keygen(key_ctx.get(), std::out_ptr(key_loc));
    }

    int private_len = i2d_PrivateKey(key_loc.get(), nullptr);
    int public_len = i2d_PUBKEY(key_loc.get(), nullptr);

    std::vector<unsigned char> private_key(private_len);
    std::vector<unsigned char> public_key(public_len);

    unsigned char* private_buf = private_key.data();
    unsigned char* public_buf = public_key.data();

    i2d_PrivateKey(key_loc.get(), &private_buf);
    i2d_PUBKEY(key_loc.get(), &public_buf);

    return std::pair(std::move(private_key), std::move(public_key));
}

int main()
{
    constexpr auto key_size = 8 * 512;
    {
        auto [pvt, pub] = createRSAPair(key_size);

        fmt::println("{}", pvt);
        fmt::println("{}", pub);
    }
}
