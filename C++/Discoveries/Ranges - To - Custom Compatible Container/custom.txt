#include <fmt/ranges.h>
#include <ranges>
#include <vector>
namespace rg = std::ranges;
namespace vs = rg::views;

template <typename T>
struct CustomContainer {
    std::vector<T> data;

    // Constructor that takes an iterator-range
    template <typename InputIt>
    CustomContainer(InputIt first, InputIt last) 
    {
        data.insert(data.end(), first, last);
    }

    // Member function insert that takes an iterator-range
    template <typename InputIt>
    void insert(InputIt first, InputIt last) 
    {
        data.insert(data.end(), first, last);
    }
};

int main()
{
    const auto v = vs::iota(0, 50) | rg::to<CustomContainer<int>>();
    fmt::println("{}", v.data);
}