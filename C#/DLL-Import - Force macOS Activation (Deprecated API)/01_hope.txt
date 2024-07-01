using System;
using System.Runtime.InteropServices;

class Program
{
    // Import the Objective-C runtime functions
    [DllImport("/usr/lib/libobjc.A.dylib", EntryPoint = "sel_registerName")]
    private static extern IntPtr sel_registerName(string selectorName);

    [DllImport("/usr/lib/libobjc.A.dylib", EntryPoint = "objc_getClass")]
    private static extern IntPtr objc_getClass(string className);

    // We need to define separate P/Invoke signatures for objc_msgSend with different argument types
    [DllImport("/usr/lib/libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    private static extern IntPtr objc_msgSend(IntPtr receiver, IntPtr selector);

    [DllImport("/usr/lib/libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    private static extern void objc_msgSend_void(IntPtr receiver, IntPtr selector, int arg);

    const int NSApplicationActivateIgnoringOtherApps = 1 << 1; // Define the activation option

    static void Main(string[] args)
    {
        try
        {
            ActivateMyApplication();
            Console.WriteLine("Application activation attempted.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
        }
    }

    static void ActivateMyApplication()
    {
        // Get the NSApplication class
        IntPtr nsApplicationClass = objc_getClass("NSApplication");

        // Get the sharedApplication method
        IntPtr sharedApplicationSelector = sel_registerName("sharedApplication");
        IntPtr sharedApplication = objc_msgSend(nsApplicationClass, sharedApplicationSelector);

        // Activate application, using objc_msgSend_void since we are passing an int argument
        IntPtr activateIgnoringOtherAppsSelector = sel_registerName("activateIgnoringOtherApps:");
        objc_msgSend_void(sharedApplication, activateIgnoringOtherAppsSelector, NSApplicationActivateIgnoringOtherApps);
    }
}
