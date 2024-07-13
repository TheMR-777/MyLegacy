#include <print>
#include <array>

constexpr std::string_view base64_table = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";

std::string to_base64_old(std::string_view input) 
{
    auto o_size = ((4 * input.size() / 3) + 3) & ~3;
    auto output = std::string(o_size, '=');
    auto it_out = output.begin();

    for (size_t i = 0; i < input.size(); i += 3) 
    {
        // Get the three bytes as an unsigned integer
        auto group = static_cast<uint8_t>(input[i]) << 16;
        if (i + 1 < input.size()) 
        {
            group |= static_cast<uint8_t>(input[i + 1]) << 8;
        }
        if (i + 2 < input.size()) 
        {
            group |= static_cast<uint8_t>(input[i + 2]);
        }

        // Encode the four base64 characters from the group
        *it_out++ = base64_table[(group >> 18) & 0x3F];
        *it_out++ = base64_table[(group >> 12) & 0x3F];
        if (i + 1 < input.size()) 
        {
            *it_out++ = base64_table[(group >> 6) & 0x3F];
        }
        if (i + 2 < input.size()) 
        {
            *it_out++ = base64_table[group & 0x3F];
        }
    }

    return output;
}

std::string to_base64_compact(std::string_view input) 
{
    const size_t n = input.size();
    const size_t out_size = ((4 * n / 3) + 3) & ~3;
    auto out = std::string(); out.resize(out_size);
    auto out_it = out.begin();

    for (size_t i = 0; i < n; i += 3) {
        const uint32_t group = (static_cast<uint32_t>(input[i]) << 16)
            | (i + 1 < n ? static_cast<uint32_t>(input[i + 1]) << 8 : 0)
            | (i + 2 < n ? static_cast<uint32_t>(input[i + 2]) : 0);

        *out_it++ = base64_table[(group >> 18) & 0x3F];
        *out_it++ = base64_table[(group >> 12) & 0x3F];
        *out_it++ = i + 1 < n ? base64_table[(group >> 6) & 0x3F] : '=';
        *out_it++ = i + 2 < n ? base64_table[group & 0x3F] : '=';
    }

    return out;
}

std::string to_base64_space(std::string_view data) {
    const size_t n = data.size();
    const size_t out_size = ((4 * n / 3) + 3) & ~3; // round up to multiple of 4
    std::string out(out_size, '\0');
    auto out_it = out.begin();

    // Pad the input data with null bytes if necessary
    const size_t padded_size = ((n + 2) / 3) * 3;
    std::string padded_data(padded_size, '\0');
    std::copy(data.begin(), data.end(), padded_data.begin());

    for (size_t i = 0; i < padded_size; i += 3) {
        const uint32_t group = (static_cast<uint32_t>(padded_data[i]) << 16)
            | (static_cast<uint32_t>(padded_data[i + 1]) << 8)
            | static_cast<uint32_t>(padded_data[i + 2]);

        *out_it++ = base64_table[(group >> 18) & 0x3F];
        *out_it++ = base64_table[(group >> 12) & 0x3F];
        *out_it++ = base64_table[(group >> 6) & 0x3F];
        *out_it++ = base64_table[group & 0x3F];
    }

    // Replace the last 1 or 2 Base64 characters with padding characters
    const size_t padding_size = padded_size - n;
    if (padding_size > 0) {
        out[out_size - 1] = '=';
    }
    if (padding_size > 1) {
        out[out_size - 2] = '=';
    }

    return out;
}

int main()
{
    // Old is Fastest -> Compact -> Space
}
