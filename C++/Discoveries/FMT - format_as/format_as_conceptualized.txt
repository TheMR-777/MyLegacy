#include <fmt/ranges.h>

template <class T>
struct my_type
{
	T x = 2101, y = 2077;
};

template <class T, template<class...> class Tp>
concept is_specialization = std::_Is_specialization_v<T, Tp>;

auto format_as(const is_specialization<my_type> auto& my_obj) noexcept
{
	return fmt::format("<{} - {}>", my_obj.x, my_obj.y);
}

int main()
{
	fmt::println("|>{:-^27}<|", my_type<size_t>{});
}