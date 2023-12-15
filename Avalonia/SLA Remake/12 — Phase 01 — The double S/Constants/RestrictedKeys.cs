using Avalonia.Input;
using System.Collections.Generic;

namespace SLA_Remake;

public static class Restricted
{
	// It contains the Restricted Keys, and Modifiers.
	// Restricting Modifiers restrict key-combinations.

	public static readonly List<Key> Keys =
	[
		// Alt, Ctrl, etc. are being handled separately
		Key.Escape,     Key.Insert,     Key.Home,       Key.End,        Key.Delete,
		Key.F1,         Key.F2,         Key.F3,         Key.F4,         Key.F5,
		Key.F6,         Key.F7,         Key.F8,         Key.F9,         Key.F10,
		Key.F11,        Key.F12,        Key.F13,        Key.F14,        Key.F15,
		Key.F16,        Key.F17,        Key.F18,        Key.F19,        Key.F20,
		Key.F21,        Key.F22,        Key.F23,        Key.F24,

		Key.PageUp,     Key.PageDown,                   Key.MediaNextTrack,
		Key.KanaMode,   Key.KanjiMode,                  Key.MediaPreviousTrack,
		Key.VolumeUp,   Key.VolumeDown,                 Key.VolumeMute,
		Key.MediaStop,  Key.MediaPlayPause,             Key.SelectMedia,
		Key.LaunchMail, Key.LaunchApplication1,         Key.LaunchApplication2
	];

	public static readonly List<KeyModifiers> Modifiers =
	[
		KeyModifiers.Alt,       KeyModifiers.Control,       KeyModifiers.Meta,
	];
}