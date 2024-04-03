#include <print>
#include <functional>

int main()
{
	constexpr auto m_variable_1 = 5, m_variable_2 = 47, m_test = 777;
	constexpr auto super_lambda = [](const std::integral auto a, const std::integral auto b) 
	{
		std::println(" 1st: {} \n 2nd: {} \n", a, b);
	};

	constexpr auto m_bound_fn_1 = std::bind_front(super_lambda, m_variable_1);
	constexpr auto m_bound_fn_2 = std::bind_back(super_lambda, m_variable_2);

	m_bound_fn_1(m_test);
	m_bound_fn_2(m_test);
}
