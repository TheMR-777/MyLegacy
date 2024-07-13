#include <fmt/ranges.h>
#include <fmt/std.h>
#include <array>

namespace my
{
	template <class>
	constexpr auto rank = 0;

	template <class T>
	constexpr auto rank<T[]> = 1 + rank<T>;

	template <class T, size_t N>
	constexpr auto rank<T[N]> = 1 + rank<T>;


	template <class, template <class...> class Template>
	constexpr auto is_specialization = false;

	template <template <class...> class Template, class...Args>
	constexpr auto is_specialization<Template<Args...>, Template> = true;

	namespace modern
	{
		template <class T>
		constexpr auto rank = []
		{
			if constexpr (std::is_array_v<T>)
			{
				return 1 + modern::rank<std::remove_extent_t<T>>;
			}
			return 0;
		}();

		template <class T, template <class...> class Template>
		concept specialization_of = requires
		{
			[] <class...Args>(Template<Args...>*) requires std::same_as<Template<Args...>, T>
			{} (static_cast<T*>(nullptr));
		};
	}
}

int main()
{
	using my_type = int[][7][7][7];
	using my_spec = std::tuple<my_type>;

	constexpr std::remove_all_extents_t<my_type> x[]
	{
		my::rank<my_type>,
		my::is_specialization<my_spec, std::tuple>,
		my::modern::rank<my_type>,
		my::modern::specialization_of<my_spec, std::tuple>,
	};

	fmt::println("{}", x);
}