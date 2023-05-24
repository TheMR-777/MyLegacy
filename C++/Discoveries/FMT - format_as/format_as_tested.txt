#include <fmt/ranges.h>

template <class T>
struct my_type
{
	T x = 2101, y = 2077;
};

template <class T> requires std::_Is_specialization_v<T, my_type>
auto format_as(const T& my_obj) noexcept
{
	return fmt::format("<{} - {}>", my_obj.x, my_obj.y);
}

int main()
{
	fmt::println("{}", my_type<size_t>{});
}