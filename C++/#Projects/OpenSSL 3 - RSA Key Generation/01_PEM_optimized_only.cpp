#include <openssl/bio.h>
#include <openssl/err.h>
#include <openssl/evp.h>
#include <openssl/pem.h>
#include <openssl/rsa.h>
#include <string>
#include <utility>
#include <vector>

std::pair<std::string, std::string> GenerateKeyPair(int bits)
{
    int r = {};
    std::pair<std::string, std::string> keyPair;

    EVP_PKEY_CTX* ctx;
    EVP_PKEY* pkey = NULL;

    ctx = EVP_PKEY_CTX_new_id(EVP_PKEY_RSA, NULL);

    r = EVP_PKEY_keygen_init(ctx);
    r = EVP_PKEY_CTX_set_rsa_keygen_bits(ctx, bits);
    r = EVP_PKEY_keygen(ctx, &pkey);

    BIO* pvt_buf, * pub_buf;

    pvt_buf = BIO_new(BIO_s_mem());
    pub_buf = BIO_new(BIO_s_mem());

    r = PEM_write_bio_PrivateKey(pvt_buf, pkey, NULL, NULL, 0, NULL, NULL);
    r = PEM_write_bio_PUBKEY(pub_buf, pkey);

    char* pvt_key = nullptr;
    char* pub_key = nullptr;

    BIO_get_mem_data(pvt_buf, &pvt_key);
    BIO_get_mem_data(pub_buf, &pub_key);

    keyPair = { 
        std::string(pvt_key, BIO_ctrl_pending(pvt_buf)), 
        std::string(pub_key, BIO_ctrl_pending(pub_buf)),
    };

    BIO_free_all(pub_buf);
    BIO_free_all(pvt_buf);
    EVP_PKEY_free(pkey);
    EVP_PKEY_CTX_free(ctx);

    return keyPair;
}

int main()
{
    auto [pvt, pub] = GenerateKeyPair(8 * 256);
}
