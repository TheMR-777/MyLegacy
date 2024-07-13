#include <openssl/pem.h>
#include <openssl/evp.h>
#include <string>
#include <utility>
#include <print>

auto createRSAPair(int bits) 
{
    struct key_holder : std::string_view
    {
        using std::string_view::string_view;
        std::unique_ptr<BIO, decltype(&BIO_free)> bio{ BIO_new_mem_buf((void*)data(), size()), &BIO_free };
    };
    EVP_PKEY* key_loc = nullptr;
    {
        auto state = EVP_PKEY_CTX_new_id(EVP_PKEY_RSA, nullptr);
        EVP_PKEY_keygen_init(state);
        EVP_PKEY_CTX_set_rsa_keygen_bits(state, bits);
        EVP_PKEY_generate(state, &key_loc);
        EVP_PKEY_CTX_free(state);
    }
    auto pvt_bio_buf = BIO_new(BIO_s_mem());
    auto pub_bio_buf = BIO_new(BIO_s_mem());

    PEM_write_bio_PrivateKey(pvt_bio_buf, key_loc, nullptr, nullptr, 0, nullptr, nullptr);
    PEM_write_bio_PUBKEY(pub_bio_buf, key_loc);

    char* pri_ptr = nullptr;
    char* pub_ptr = nullptr;
    size_t pri_len = BIO_get_mem_data(pvt_bio_buf, &pri_ptr);
    size_t pub_len = BIO_get_mem_data(pub_bio_buf, &pub_ptr);
    EVP_PKEY_free(key_loc);

    return std::pair{ key_holder{ pri_ptr, pri_len }, key_holder{ pub_ptr, pub_len } };
}

int main()
{
    constexpr auto key_size = 8 * 512;
    {
        auto [pvt, pub] = createRSAPair(key_size);

        std::println("{}", std::string_view{ pvt });
        std::println("{}", std::string_view{ pub });
    }
}
