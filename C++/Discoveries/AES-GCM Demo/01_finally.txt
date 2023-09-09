// namespace mr_crypt ...

// Tag will be appended to the end of the encrypted data
auto aes_256_gcm_e(mr_crypt::view_t input, mr_crypt::view_t my_key, mr_crypt::view_t the_iv) noexcept
{
	auto cipher = EVP_aes_256_gcm();
	auto output = std::string(input.size() + 16, '\0');
	auto it_out = reinterpret_cast<mr_crypt::byte_t*>(output.data());
	auto size_i = int{}, size_f = int{};
	{
		auto state = EVP_CIPHER_CTX_new();
		EVP_EncryptInit(state, cipher, reinterpret_cast<const unsigned char*>(my_key.data()), reinterpret_cast<const unsigned char*>(the_iv.data()));
		EVP_EncryptUpdate(state, it_out, &size_i, reinterpret_cast<const unsigned char*>(input.data()), input.size());
		EVP_EncryptFinal(state, it_out + size_i, &size_f);
		EVP_CIPHER_CTX_ctrl(state, EVP_CTRL_GCM_GET_TAG, 16, it_out + size_i + size_f);
		EVP_CIPHER_CTX_free(state);
	}
	return output;
}

// std::string_views can be used over input, for separating cipher and tag
auto aes_256_gcm_d(mr_crypt::view_t input, mr_crypt::view_t my_key, mr_crypt::view_t the_iv) noexcept
{
	auto cipher = EVP_aes_256_gcm();
	auto output = std::string(input.size() - 16, '\0');
	auto it_out = reinterpret_cast<mr_crypt::byte_t*>(output.data());
	auto size_i = int{}, size_f = int{};
	{
		auto state = EVP_CIPHER_CTX_new();
		EVP_DecryptInit(state, cipher, reinterpret_cast<const unsigned char*>(my_key.data()), reinterpret_cast<const unsigned char*>(the_iv.data()));
		EVP_DecryptUpdate(state, it_out, &size_i, reinterpret_cast<const unsigned char*>(input.data()), input.size() - 16);
		EVP_CIPHER_CTX_ctrl(state, EVP_CTRL_GCM_SET_TAG, 16, const_cast<mr_crypt::byte_t*>(reinterpret_cast<const mr_crypt::byte_t*>(input.data()) + input.size() - 16));
		EVP_DecryptFinal(state, it_out + size_i, &size_f);
		EVP_CIPHER_CTX_free(state);
	}
	return output;
}

int main()
{
	auto my_data = std::string{};
	while (1)
	{
		std::println("");
		std::println(" AES-256-GCM Encryption Demo");

		const auto my_key = mr_crypt::produce::random_bytes(32);
		const auto the_iv = mr_crypt::produce::random_bytes(12);

		std::println("");
		std::println(" Key : {}", my_key | mr_crypt::convert::to_base64);
		std::println(" IV  : {}", the_iv | mr_crypt::convert::to_base64);
		std::println("");
		std::print(" Enter data to encrypt : "); std::getline(std::cin, my_data);

		const auto encrypt = aes_256_gcm_e(my_data, my_key, the_iv);
		const auto decrypt = aes_256_gcm_d(encrypt, my_key, the_iv);

		std::println("");
		std::println(" Encrypted : {}", encrypt | mr_crypt::convert::to_base64);
		std::println(" Decrypted : {}", decrypt);
		std::cin.get();
		system("cls");
	}
}
