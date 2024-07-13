#include <vector>

template <class T, template <class...> class Template>
struct is_specialization : std::false_type {};

template <template <class...> class Template, class... Args>
struct is_specialization<Template<Args...>, Template> : std::true_type {};

template <class T, template <class...> class Template>
constexpr auto is_specialization_v = is_specialization<T, Template>::value;

template <class T, template <class...> class Template>
concept specialization_of = is_specialization_v<T, Template>;

int main()
{
	constexpr auto result = specialization_of<std::vector<int>, std::vector>;
}