#include <print>

namespace my
{
	template <class T>
	struct type_is { using type = T; };

	template <class T>
	struct remove_all_extents : type_is<T> {};

	template <class T>
	struct remove_all_extents<T[]> : type_is<remove_all_extents<T>::type> {};

	template <class T, size_t N>
	struct remove_all_extents<T[N]> : type_is<remove_all_extents<T>::type> {};


	template <class T>
	constexpr auto rank = 0;
	
	template <class T>
	constexpr auto rank<T[]> = 1 + rank<T>;

	template <class T, size_t N>
	constexpr auto rank<T[N]> = 1 + rank<T>;


	template <class T1, class T2>
	constexpr auto is_same = false;

	template <class T>
	constexpr auto is_same<T, T> = true;


	template <class T, template <class...> class Template>
	constexpr auto is_specialization_of = false;

	template <template <class...> class Template, class... Args>
	constexpr auto is_specialization_of<Template<Args...>, Template> = true;
}

int main()
{
	using my_type = std::tuple<int>[][7][7][7];
	constexpr auto my_rank = my::rank<my_type>;
	constexpr auto is_same = my::is_same<my::remove_all_extents<my_type>::type, std::tuple<int>>;
	constexpr auto special = my::is_specialization_of<my::remove_all_extents<my_type>::type, std::tuple>;
}