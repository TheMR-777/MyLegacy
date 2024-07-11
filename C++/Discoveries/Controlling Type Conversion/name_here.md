# A -> B
Clarification
- `A` has to be "converted-to" `B`.
- `B` has to be "constructed-from" `A`.

```cpp
template <class T>
concept has_x = requires { std::declval<T>().x; };

struct A
{
	std::size_t x = 47;
};

struct B
{
	std::size_t x = 77;
};
```

### using Constructor
- `A` -> `B`: Through **B**.
- `B` will have the constructor that takes `A` as an argument.
```cpp
struct B
{
    std::size_t x = 77;
    B(has_x auto&& other_a) noexcept : x(other_a.x) {}
};
```

### using Conversion Operator
- `A` -> `B`: Through **A**.
- `A` will have the conversion operator that converts `A` to `B`.
```cpp
struct A
{
    std::size_t x = 47;
    constexpr operator B() const noexcept { return {x}; }
};
```
