#include <numeric>
#include <random>
#include <print>

struct GUID 
{
    uint32_t time_low : 32;
    uint16_t time_mid : 16;
    uint16_t time_high_version : 16;
    uint16_t clock_seq : 14;
    uint64_t node : 48;

    auto to_string() const noexcept
    {
        return std::format("{:08X}-{:04X}-{:04X}-{:04X}-{:012X}", time_low, time_mid, time_high_version, clock_seq, node);
    }

    static auto generate() noexcept
    {
        static auto my_engine = std::mt19937_64{ std::random_device{}() };
        auto dis_16 = std::uniform_int_distribution<uint16_t>(0, std::numeric_limits<uint16_t>::max());
        auto dis_32 = std::uniform_int_distribution<uint32_t>(0, std::numeric_limits<uint32_t>::max());
        auto dis_64 = std::uniform_int_distribution<uint64_t>(0, std::numeric_limits<uint64_t>::max());

        return GUID
        {
            .time_low = dis_32(my_engine),
            .time_mid = dis_16(my_engine),
            .time_high_version = static_cast<uint16_t>((dis_16(my_engine) & 0x0FFF) | 0x4000),
            .clock_seq = static_cast<uint16_t>((dis_16(my_engine) & 0x3FFF) | 0x8000),
            .node = dis_64(my_engine),
        };
    }
};

int main() 
{
    std::println("GUID: {}", GUID::generate().to_string());
}