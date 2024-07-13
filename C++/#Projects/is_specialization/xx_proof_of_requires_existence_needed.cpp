import std;

namespace my
{
	template <class T>
	struct type_01 {};

	template <class T>
	struct type_02 : type_01<T> {};
}

template <class T, template <class...> class Template>
concept specialization_of = requires
{
	[] <class... Args> (Template<Args...>*)// requires std::same_as<T, Template<Args...>>
	{} (static_cast<T*>(nullptr));
};

int main()
{
	constexpr auto x = specialization_of<my::type_02<int>, my::type_01>;
	std::println("{}", x);
}
