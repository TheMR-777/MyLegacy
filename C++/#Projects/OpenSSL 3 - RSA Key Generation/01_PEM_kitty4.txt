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
    std::pair<std::string, std::string> keyPair;

    EVP_PKEY_CTX* ctx;
    EVP_PKEY* pkey = NULL;

    ctx = EVP_PKEY_CTX_new_id(EVP_PKEY_RSA, NULL);

    /* Generate the RSA key */
    if (EVP_PKEY_keygen_init(ctx) <= 0) {
        /* Error occurred */
    }

    if (EVP_PKEY_CTX_set_rsa_keygen_bits(ctx, bits) <= 0) {
        /* Error occurred */
    }

    if (EVP_PKEY_keygen(ctx, &pkey) <= 0) {
        /* Error occurred */
    }

    /* Write the private and public key to memory buffers */
    BIO* privateBio, * publicBio;

    privateBio = BIO_new(BIO_s_mem());
    publicBio = BIO_new(BIO_s_mem());

    if (!PEM_write_bio_PrivateKey(privateBio, pkey, NULL, NULL, 0, NULL, NULL)) {
        /* Error occurred */
    }

    if (!PEM_write_bio_PUBKEY(publicBio, pkey)) {
        /* Error occurred */
    }

    char* privateKey;
    char* publicKey;

    BIO_get_mem_data(privateBio, &privateKey);
    BIO_get_mem_data(publicBio, &publicKey);

    keyPair.first.assign(privateKey, BIO_ctrl_pending(privateBio));
    keyPair.second.assign(publicKey, BIO_ctrl_pending(publicBio));

    /* Clean up */
    BIO_free_all(publicBio);
    BIO_free_all(privateBio);
    EVP_PKEY_free(pkey);
    EVP_PKEY_CTX_free(ctx);

    return keyPair;
}

int main()
{
    auto [pvt, pub] = GenerateKeyPair(8 * 256);
}
