#include <print>

namespace my
{
	template <class T> 
	struct type_is { using type = T; };

	template <class T>
	struct remove_const : type_is<T> {};

	template <class T>
	struct remove_const<const T> : type_is<T> {};

	template <class T>
	using remove_const_t = remove_const<T>::type;
}

int main()
{
	using value_type = my::remove_const_t<const int>;
}