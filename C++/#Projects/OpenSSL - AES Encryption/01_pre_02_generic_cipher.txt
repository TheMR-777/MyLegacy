namespace crypto
{
	namespace detail
	{
		template <bool is_encrypt = true>
		auto process_cipher(std::string_view input, std::string_view key) noexcept
		{
			auto m_output = std::string(input.size() + EVP_MAX_BLOCK_LENGTH, '\0');
			auto i_output = reinterpret_cast<byte_t*>(m_output.data());
			auto out_ln_i = int{};
			auto out_ln_f = int{};

			constexpr auto init = is_encrypt ? EVP_EncryptInit_ex : EVP_DecryptInit_ex;
			constexpr auto update = is_encrypt ? EVP_EncryptUpdate : EVP_DecryptUpdate;
			constexpr auto end = is_encrypt ? EVP_EncryptFinal_ex : EVP_DecryptFinal_ex;

			auto context = EVP_CIPHER_CTX_new();
			init(context, EVP_aes_256_ecb(), nullptr, reinterpret_cast<const byte_t*>(key.data()), nullptr);
			update(context, i_output, &out_ln_i, reinterpret_cast<const byte_t*>(input.data()), input.size());
			end(context, i_output + out_ln_i, &out_ln_f);
			EVP_CIPHER_CTX_free(context);

			m_output.resize(out_ln_i + out_ln_f);
			return m_output;
		}
	}

	auto generate_key(const size_t bytes_n) noexcept
	{
		static auto random_numbers_g = []() -> byte_t
		{
			static auto my_engine = std::mt19937_64{ std::random_device{ }() };
			return std::uniform_int_distribution<uint16_t>{ 0,255 }(my_engine);
		};
		return vs::generate_n(random_numbers_g, bytes_n) | rg::to<std::string>;
	}

	namespace encrypt
	{
		constexpr auto aes_256_ecb = detail::process_cipher<true>;

		auto aes_256_ecb_k(std::string_view input) noexcept -> std::pair<std::string, std::string>
		{
			const auto my_key = generate_key(EVP_CIPHER_key_length(EVP_aes_256_ecb()));
			return { my_key, aes_256_ecb(input, my_key) };
		}
	}

	namespace decrypt
	{
		constexpr auto aes_256_ecb = detail::process_cipher<false>;
	}
}
