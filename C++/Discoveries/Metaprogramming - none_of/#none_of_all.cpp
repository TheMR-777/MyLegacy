#include <concepts>

namespace v0
{
    template <class, class...>
    constexpr bool none_of;

    template <class T>
    constexpr auto none_of<T> = true;

    template <class T, class... Ts>
    constexpr auto none_of<T, T, Ts...> = false;

    template <class T, class U, class... Ts>
    constexpr auto none_of<T, U, Ts...> = none_of<T, Ts...>;
}

namespace v1
{
    template <class, class...>
    constexpr auto none_of = true;

    template <class T, class... Ts>
    constexpr auto none_of<T, T, Ts...> = false;

    template <class T, class U, class... Ts>
    constexpr auto none_of<T, U, Ts...> = none_of<T, Ts...>;
}

namespace v2
{
    template <class T, class... Ts>
    constexpr auto none_of = (!std::is_same_v<T, Ts> && ...);
}

namespace v3
{
    template <class T, class... Ts>
    concept none_of = (!std::same_as<T, Ts> && ...);
}

int main()
{
    using T0 = int;
    using T1 = float;
    return v0::none_of<T0, T1, T1, T1>;
}