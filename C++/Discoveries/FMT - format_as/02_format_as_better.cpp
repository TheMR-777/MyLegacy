#include <fmt/ranges.h>

template <std::integral T>
struct my_type
{
	T x = 2011, y = 2077;
};

template <class T, template<class...> class Tp>
concept specialization_of = std::_Is_specialization_v<T, Tp>;

auto format_as(const specialization_of<my_type> auto& v) noexcept
{
	const auto [x, y] = v;
	return fmt::format("<{} - {}>", x, y);
}

int main()
{
	fmt::println("|>{:-^27}<|", my_type<std::size_t>{});
}