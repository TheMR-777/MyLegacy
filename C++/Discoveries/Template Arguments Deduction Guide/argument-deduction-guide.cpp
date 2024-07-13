#include <iostream>
#include <functional>

namespace std
{
	template <class T, class R, class... Args>
	function(R (T::*)(Args...) const noexcept) -> function<R(T&, Args...)>;
}

struct my_type
{
	constexpr auto operator()() const noexcept
	{
		return 77;
	}
};

int main()
{
	auto x = std::function(&my_type::operator());
}