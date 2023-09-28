#include <print>

template <class T>
struct my_type
{
	T x = {}, y = {};
};

template <my_type T>
auto print()
{
	std::println("{} - {}", T.x, T.y);
}

int main()
{
	constexpr auto my_data = my_type<int>{ 1, 2 };
	print<my_data>();
}
