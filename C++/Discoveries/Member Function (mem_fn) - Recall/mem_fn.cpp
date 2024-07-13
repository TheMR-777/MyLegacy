#include <functional>

struct my_type
{
	constexpr auto operator()() const noexcept
	{
		return __func__;
	}
};

int main()
{
	constexpr auto x = std::mem_fn(&my_type::operator());
	constexpr auto y = x(my_type{}); // y is "operator()"
}