#include <print>

template <class T>
concept has_x = requires { std::declval<T>().x; };

struct B
{
	constexpr B() noexcept = default;
	constexpr B(const std::size_t x) noexcept : x(x) {}
	constexpr B(has_x auto&& other_a) noexcept : x(other_a.x) {}

	std::size_t x = 77;
};

struct A
{
	std::size_t x = 47;

	constexpr operator B() const noexcept { return B{ x }; }
};

int main()
{
	constexpr auto my_a = A{};
	constexpr auto my_b = B{};

	constexpr auto x_01 = static_cast<B>(my_a);
}
