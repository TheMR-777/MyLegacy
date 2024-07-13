#include <print>

namespace test_case
{
	template <class T = void>
	struct base {};

	template <class T = void>
	struct derv : base<T> {};
}

template <class T, template <class...> class Template>
concept specialization_of = requires
{
	[] <class... Args>(Template<Args...>*) requires std::same_as<T, Template<Args...>>
	{} (static_cast<T*>(nullptr));
};

int main()
{
	// Pointer Conversion Sample:

	constexpr auto pt_01 = static_cast<test_case::base<>*>(nullptr);
	constexpr auto pt_02 = static_cast<test_case::derv<>*>(nullptr);
	constexpr decltype(pt_01) pt_03 = pt_02;

	// Loophole in the Implementation (if not used 'require')

	constexpr auto result = specialization_of<test_case::derv<>, test_case::base>;
	std::println("{}", result);
}