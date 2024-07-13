#include <vector>

namespace my
{
	template <class T>
	constexpr auto is_vector = false;

	template <class... Args>
	constexpr auto is_vector<std::vector<Args...>> = true;

	template <class T>
	concept only_vector = is_vector<T>;
}

int main()
{
	my::only_vector auto my_vector = std::vector{ 0,1,2,3,4,5 };
}