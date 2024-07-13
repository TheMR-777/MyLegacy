#include <fmt/ranges.h>
#include <vector>

template <class _Type, template <class...> class _Template>
constexpr auto is_specialization_v = false;

template <template <class...> class _Template, class... _Types>
constexpr auto is_specialization_v<_Template<_Types...>, _Template> = true;

int main()
{
	constexpr auto s = is_specialization_v<std::vector<int>, std::vector>;
}