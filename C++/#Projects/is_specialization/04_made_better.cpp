#include <print>

namespace my
{
	template <class, template <class...> class>
	constexpr auto is_specialization = false;

	template <template <class...> class Template, class... Args>
	constexpr auto is_specialization<Template<Args...>, Template> = true;
}

int main()
{
	constexpr auto x = my::is_specialization<std::tuple<int>, std::tuple>;
}