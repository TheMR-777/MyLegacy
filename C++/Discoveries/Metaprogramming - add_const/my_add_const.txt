#include <print>

namespace my
{
	template <class T>
	struct type_is { using type = T; };

	template <class T>
	struct add_const : type_is<const T> {};
}

int main()
{
	using target = int;
	using m_type = my::add_const<target>::type;
	using std_on = std::add_const_t<target>;

	constexpr auto result = std::same_as<m_type, std_on>;
}