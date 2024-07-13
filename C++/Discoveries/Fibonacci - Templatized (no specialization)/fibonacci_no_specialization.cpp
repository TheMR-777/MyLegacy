template <int64_t N>
constexpr int64_t fibonacci = N < 2 ? N : (fibonacci<N - 1> + fibonacci<N - 2>);