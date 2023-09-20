#include <print>

int main()
{
	size_t x = {}, y = {};
	constexpr auto my_data = std::string_view{ "47 - 63" };
	sscanf_s(my_data.data(), "%zu - %zu", &x, &y);
}
