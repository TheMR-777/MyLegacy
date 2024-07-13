#include <iostream>
#include <string>
#include <vector>
#include <stdexcept>
#include <openssl/evp.h>
#include <openssl/conf.h>
#include <openssl/err.h>

namespace mr_crypto {

    enum class AesMode {
        ECB,
        CBC,
        CFB,
        OFB,
        CTR
    };

    class Aes256 {
    public:
        Aes256(const std::vector<unsigned char>& key, AesMode mode)
            : mode{ mode } {
            if (key.size() != 32) {
                throw std::invalid_argument("Invalid key size. Key size must be 256 bits (32 bytes).");
            }
            key_ = key;
        }

        std::vector<unsigned char> encrypt(const std::vector<unsigned char>& plaintext, const std::vector<unsigned char>& iv) {
            return process(plaintext, iv, true);
        }

        std::vector<unsigned char> decrypt(const std::vector<unsigned char>& ciphertext, const std::vector<unsigned char>& iv) {
            return process(ciphertext, iv, false);
        }

    private:
        std::vector<unsigned char> process(const std::vector<unsigned char>& input, const std::vector<unsigned char>& iv, bool encrypt) 
        {
            if (iv.size() != 16) {
                throw std::invalid_argument("Invalid IV size. IV size must be 128 bits (16 bytes).");
            }

            EVP_CIPHER_CTX* ctx = EVP_CIPHER_CTX_new();
            if (!ctx) {
                throw
                    std::runtime_error("Failed to create EVP_CIPHER_CTX.");
            }

            const EVP_CIPHER* cipher = get_cipher(mode);
            if (!cipher) {
                EVP_CIPHER_CTX_free(ctx);
                throw std::runtime_error("Invalid AES mode.");
            }

            if (encrypt) {
                if (EVP_EncryptInit_ex(ctx, cipher, NULL, key_.data(), iv.data()) != 1) {
                    EVP_CIPHER_CTX_free(ctx);
                    throw std::runtime_error("Failed to initialize encryption.");
                }
            }
            else {
                if (EVP_DecryptInit_ex(ctx, cipher, NULL, key_.data(), iv.data()) != 1) {
                    EVP_CIPHER_CTX_free(ctx);
                    throw std::runtime_error("Failed to initialize decryption.");
                }
            }

            std::vector<unsigned char> output(input.size() + EVP_CIPHER_CTX_block_size(ctx));
            int outlen1;

            if (encrypt) {
                if (EVP_EncryptUpdate(ctx, output.data(), &outlen1, input.data(), static_cast<int>(input.size())) != 1) {
                    EVP_CIPHER_CTX_free(ctx);
                    throw std::runtime_error("Failed to update encryption.");
                }
            }
            else {
                if (EVP_DecryptUpdate(ctx, output.data(), &outlen1, input.data(), static_cast<int>(input.size())) != 1) {
                    EVP_CIPHER_CTX_free(ctx);
                    throw std::runtime_error("Failed to update decryption.");
                }
            }

            int outlen2;
            if (encrypt) {
                if (EVP_EncryptFinal_ex(ctx, output.data() + outlen1, &outlen2) != 1) {
                    EVP_CIPHER_CTX_free(ctx);
                    throw std::runtime_error("Failed to finalize encryption.");
                }
            }
            else {
                if (EVP_DecryptFinal_ex(ctx, output.data() + outlen1, &outlen2) != 1) {
                    EVP_CIPHER_CTX_free(ctx);
                    throw std::runtime_error("Failed to finalize decryption.");
                }
            }

            EVP_CIPHER_CTX_free(ctx);
            output.resize(outlen1 + outlen2);
            return output;
        }

        const EVP_CIPHER* get_cipher(AesMode mode) {
            switch (mode) {
            case AesMode::ECB: return EVP_aes_256_ecb();
            case AesMode::CBC: return EVP_aes_256_cbc();
            case AesMode::CFB: return EVP_aes_256_cfb();
            case AesMode::OFB: return EVP_aes_256_ofb();
            case AesMode::CTR: return EVP_aes_256_ctr();
            default: return nullptr;
            }
        }

        AesMode mode;
        std::vector<unsigned char> key_;
    };

} // namespace mr_crypto

int main() {
    // 256-bit key
    std::vector<unsigned char> key = {
        0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07,
        0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f,
        0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17,
        0x18, 0x19, 0x1a, 0x1b, 0x1c, 0x1d, 0x1e, 0x1f
    };

    // 128-bit IV
    std::vector<unsigned char> iv = {
        0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07,
        0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f
    };

    std::string plaintext_str = "This is a test message!";
        std::vector<unsigned char> plaintext(plaintext_str.begin(), plaintext_str.end());

    mr_crypto::Aes256 aes(key, mr_crypto::AesMode::CBC);

    std::vector<unsigned char> ciphertext = aes.encrypt(plaintext, iv);
    std::vector<unsigned char> decrypted = aes.decrypt(ciphertext, iv);

    std::string decrypted_str(decrypted.begin(), decrypted.end());
    std::cout << "Decrypted message: " << decrypted_str << std::endl;

    return 0;
}
