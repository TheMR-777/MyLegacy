#include <fmt/ranges.h>
#include <vector>

template <class _Type, template <class...> class _Template>
_INLINE_VAR constexpr bool _is_specialization_v = false; // true if and only if _Type is a specialization of _Template
template <template <class...> class _Template, class... _Types>
_INLINE_VAR constexpr bool _is_specialization_v<_Template<_Types...>, _Template> = true;

template <class _Type, template <class...> class _Template>
struct _is_specialization : std::bool_constant<_is_specialization_v<_Type, _Template>> {};

int main()
{
	constexpr auto s = _is_specialization<std::vector<int>, std::vector>::value;
}