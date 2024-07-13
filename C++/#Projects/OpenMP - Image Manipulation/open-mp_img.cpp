// Include the necessary libraries
#include <iostream>
#include <opencv2/opencv.hpp>
#include <omp.h>
#include <numbers>

constexpr auto kernel_size = 5;
constexpr auto sigma = 1.0;
constexpr auto threads_n = 4;

constexpr double exp(double base, int exponent)
{
    // Base case: exponent is zero
    if (exponent == 0)
        return 1.0;

    // Recursive case: exponent is positive
    else if (exponent > 0)
        return base * exp(base, exponent - 1);

    // Recursive case: exponent is negative
    else
        return 1.0 / exp(base, -exponent);
}

// A function to blur an image using Gaussian blur
cv::Mat blurImage(const cv::Mat& input)
{
    // Create an output image with the same size and type as the input
    cv::Mat output(input.size(), input.type());

    // Get the half size of the kernel
    int half = kernel_size / 2;

    // Use OpenMP to parallelize the loop over the rows of the image
#pragma omp parallel for num_threads(NUM_THREADS)
    for (int i = half; i < input.rows - half; i++)
    {
        // Loop over the columns of the image
        for (int j = half; j < input.cols - half; j++)
        {
            // Initialize the sum of pixel values for each channel
            // Use std::array instead of C-style array
            std::array<double, 3> sum{ 0.0, 0.0, 0.0 };

            // Loop over the kernel
            for (int k = -half; k <= half; k++)
            {
                for (int l = -half; l <= half; l++)
                {
                    // Get the pixel value at the current position
                    cv::Vec3b pixel = input.at<cv::Vec3b>(i + k, j + l);

                    // Calculate the Gaussian weight for the current position
                    double weight = exp(-(k * k + l * l) / (2 * sigma * sigma)) / (2 * std::numbers::pi * sigma * sigma);

                    // Add the weighted pixel value to the sum for each channel
                    sum[0] += weight * pixel[0];
                    sum[1] += weight * pixel[1];
                    sum[2] += weight * pixel[2];
                }
            }

            // Set the output pixel value as the normalized sum for each channel
            output.at<cv::Vec3b>(i, j) = cv::Vec3b(sum[0], sum[1], sum[2]);
        }
    }

    // Return the output image
    return output;
}

// A function to convert an image to grayscale using OpenMP
auto grayscaleImage(const cv::Mat& input)
{
    // Create an output image with the same size as the input and one channel
    cv::Mat output(input.size(), CV_8UC1);

    // Use OpenMP to parallelize the loop over the rows of the image
#pragma omp parallel for num_threads(NUM_THREADS)
    for (int i = 0; i < input.rows; i++)
    {
        // Loop over the columns of the image
        for (int j = 0; j < input.cols; j++)
        {
            // Get the pixel value at the current position
            cv::Vec3b pixel = input.at<cv::Vec3b>(i, j);

            // Calculate the grayscale value as the average of the three channels
            uchar gray = (pixel[0] + pixel[1] + pixel[2]) / 3;

            // Set the output pixel value as the grayscale value
            output.at<uchar>(i, j) = gray;
        }
    }

    // Return the output image
    return output;
}


int main()
{
    // Read an image from a file
    cv::Mat input = cv::imread(R"(C:\Users\ASC\Downloads\img.jpg)");

    // Check if the image is valid
    if (input.empty())
    {
        std::cerr << "Could not read input image" << std::endl;
        return -1;
    }

    // Blur the image using Gaussian blur
    cv::Mat blurred = blurImage(input);

    // Convert the image to grayscale using constexpr function
    cv::Mat gray = grayscaleImage(blurred);

    // Write the output images to files
    cv::imwrite(R"(C:\Users\ASC\Downloads\blr.jpg)", blurred);
    cv::imwrite(R"(C:\Users\ASC\Downloads\gry.jpg)", gray);

    // Display a message indicating success
    std::cout << "Image processing completed successfully" << std::endl;

    return 0;
}