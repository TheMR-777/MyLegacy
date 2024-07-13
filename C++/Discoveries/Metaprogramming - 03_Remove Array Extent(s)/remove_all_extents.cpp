#include <print>

namespace my
{
	template <class T> 
	struct type_is { using type = T; };


	template <class T>
	struct remove_extent : type_is<T> {};

	template <class T, size_t N>
	struct remove_extent<T[N]> : type_is<T> {};

	template <class T>
	using remove_extent_t = remove_extent<T>::type;

	// All Similar, but Specialization is Recursive

	template <class T>
	struct remove_all_extents : type_is<T> {};

	template <class T, size_t N>
	struct remove_all_extents<T[N]> : type_is<remove_all_extents<T>::type> {};

	template <class T>
	using remove_all_extents_t = remove_all_extents<T>::type;
}

int main()
{
	using my_type = size_t[7][7][7];

	using value_type_1 = my::remove_extent_t<my_type>;
	using value_type_2 = my::remove_all_extents_t<my_type>;
}