#include <print>

namespace my
{
	template <class T>
	struct type_is { using type = T; };

	template <auto N>
	struct value_is { static constexpr auto value = N; };

	namespace math
	{
		template <std::integral auto X, std::integral auto Y>
		constexpr auto GCD = GCD<Y, X% Y>;

		template <std::integral auto X>
		constexpr auto GCD<X, 0> = X;


		template <int64_t N>
		struct fibonacci
		{
			static constexpr int64_t value = N < 2 ? N : (fibonacci<N - 1>::value + fibonacci<N - 2>::value);
		};

		template <int64_t N>
		constexpr auto fibonacci_v = fibonacci<N>::value;
	}

	namespace remove
	{
		template <class T>
		struct ref : type_is<T> {};

		template <class T>
		struct ref<T&> : type_is<T> {};

		template <class T>
		struct ref<const T&> : type_is<T> {};

		template <class T>
		using ref_t = ref<T>::type;


		template <class T>
		struct extent : type_is<T> {};

		template <class T, size_t N>
		struct extent<T[N]> : type_is<T> {};

		template <class T>
		using extent_t = extent<T>::type;


		template <class T>
		struct all_extents : type_is<T> {};

		template <class T, size_t N>
		struct all_extents<T[N]> : type_is<typename all_extents<T>::type> {};

		template <class T>
		using all_extents_t = all_extents<T>::type;
	}

	namespace is
	{
		template <class T, template <class...> class Template>
		constexpr auto specialization = false;

		template <template <class...> class Template, class... Args>
		constexpr auto specialization<Template<Args...>, Template> = true;

		template <class T, template <class...> class Template>
		concept specialization_of = specialization<T, Template>;
	}


	template <bool, class, class F>
	struct conditional_type : type_is<F> {};

	template <class T, class F>
	struct conditional_type<true, T, F> : type_is<T> {};

	template <bool B, class T, class F>
	using conditional_type_t = conditional_type<B, T, F>::type;


	template <class T>
	struct rank : value_is<0> {};

	template <class T, size_t N>
	struct rank<T[N]> : value_is<1 + rank<T>::value> {};
}

int main()
{
	constexpr auto gcd_result = my::math::GCD<18, 24>;
	constexpr auto fib_result = my::math::fibonacci_v<10>;

	using my_type = size_t[7][7][7];
	using value_type_1 = my::remove::ref_t<float&>;
	using value_type_2 = my::remove::extent_t<my_type>;
	using value_type_3 = my::remove::all_extents_t<my_type>;
	using value_type_4 = my::conditional_type_t<true, my_type, int>;

	constexpr auto my_rank = my::rank<my_type>::value;

	constexpr auto x = my::is::specialization_of<std::tuple<int>, std::tuple>;
}