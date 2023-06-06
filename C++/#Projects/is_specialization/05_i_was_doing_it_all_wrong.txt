#include <print>

template <class T, template <class...> class Template>
concept specialization_of = requires
{
	[] <class... Args>(Template<Args...>*) requires std::same_as<T, Template<Args...>>
	{} (static_cast<T*>(nullptr));

	// Lambda is made, to create an implicit Args... list
	// 'requires' is listed to cover possible loop-hole caused by implicit pointer conversion (Derived -> Base)
	// Lambda is called by pointer, to avoid 'Default Constructible' requirement' side-effect
};

int main()
{
	constexpr auto x = specialization_of<std::tuple<int>, std::tuple>;
	std::println("{}", x);
}