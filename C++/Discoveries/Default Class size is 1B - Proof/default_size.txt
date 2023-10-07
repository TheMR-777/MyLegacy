#include <print>

struct my_type_t {};

int main()
{
	static constexpr auto size = sizeof(my_type_t);
	std::println("{}: {}", typeid(my_type_t).name(), size);
}