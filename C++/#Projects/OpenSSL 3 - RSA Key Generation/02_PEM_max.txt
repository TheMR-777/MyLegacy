#include <openssl/pem.h>
#include <openssl/evp.h>
#include <string>
#include <utility>
#include <print>

auto createRSAPair(int bits) 
{

    EVP_PKEY* pkey = nullptr;
    EVP_PKEY_CTX* ctx = EVP_PKEY_CTX_new_id(EVP_PKEY_RSA, nullptr);
    EVP_PKEY_keygen_init(ctx);
    EVP_PKEY_CTX_set_rsa_keygen_bits(ctx, bits);
    EVP_PKEY_keygen(ctx, &pkey);
    EVP_PKEY_CTX_free(ctx);

    auto pri = BIO_new(BIO_s_mem());
    auto pub = BIO_new(BIO_s_mem());

    PEM_write_bio_PrivateKey(pri, pkey, nullptr, nullptr, 0, nullptr, nullptr);
    PEM_write_bio_PUBKEY(pub, pkey);

    char* pri_ptr = nullptr;
    char* pub_ptr = nullptr;
    auto pri_len = BIO_get_mem_data(pri, &pri_ptr);
    auto pub_len = BIO_get_mem_data(pub, &pub_ptr);

    std::string pvt_key, pub_key;
    pvt_key.assign(pri_ptr, pri_len);
    pub_key.assign(pub_ptr, pub_len);

    BIO_free_all(pub);
    BIO_free_all(pri);
    EVP_PKEY_free(pkey);

    return std::pair{ pvt_key, pub_key };
}

int main()
{
    auto [pvt, pub] = createRSAPair(8 * 256);

    std::println("{}", pvt);
    std::println("{}", pub);
}
