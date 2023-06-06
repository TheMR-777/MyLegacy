#include <utility>

namespace my
{
    template <class T, template <class...> class Template>
    concept specialization_of = requires {
        [] <class... Args>(Template<Args...>*) requires std::same_as<T, Template<Args...>>
        {} (static_cast<T*>(nullptr));
    };
}

int main()
{
    return my::specialization_of<std::tuple<int>, std::tuple>;
}
