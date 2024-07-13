#include <print>

struct my_type_t 
{
	mutable int i = 0;
};

int main()
{
	static constexpr auto my_obj = my_type_t{};
	my_obj.i = 77;

	// This won't work :), for not being part of my_type_t, while still being inside my_type_t
	// static constexpr auto x = my_obj.i;
}