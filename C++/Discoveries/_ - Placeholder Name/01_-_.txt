#include <print>
#include <tuple>
#include <string_view>

auto get() 
{
    return std::tuple{ 0, 0.0, std::string_view{ "TheMR" } };
}

int main()
{
    const auto& [i, _, _] = get();
    const auto _ = 0;
    const auto _ = 0.0;
    const auto _ = "Hi, it's <no-name>";
}