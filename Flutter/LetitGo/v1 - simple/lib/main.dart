import 'package:flutter/material.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:google_fonts/google_fonts.dart';
import 'dart:async';

void main() => runApp(const LetGo());

class MyColor
{
  static const Color primary = Color(0xFFAD8746);
  static const Color secondary = Color(0xFF221F1F);
  static const Color accent = Color(0xFFCCBEA1);
}

class LetGo extends StatelessWidget {
  const LetGo({super.key});

  @override
  Widget build(BuildContext context) => MaterialApp(
    title: 'Let Go',
    themeMode: ThemeMode.dark,
    darkTheme: ThemeData(
      colorScheme: const ColorScheme.dark(
        // 60%: #221F1F  // Dark Brown
        // 40%: #CCBEA1  // Light Brown
        // 20%: #AD8746  // Gold Brown
        primary: MyColor.primary,
        secondary: MyColor.secondary,
        tertiary: MyColor.accent,
        surface: MyColor.secondary,
        error: MyColor.accent,
        onPrimary: MyColor.secondary,
        onSecondary: MyColor.primary,
        onSurface: MyColor.accent,
        onError: MyColor.secondary,
      ),
    ),
    home: const HomePage(),
  );
}

class HomePage extends StatefulWidget {
  const HomePage({super.key});

  @override
  _HomePageState createState() => _HomePageState();
}

class _HomePageState extends State<HomePage> {
  DateTime? _startDate;
  late Timer _timer;

  @override
  void initState() {
    super.initState();
    _loadStartDate();
    _timer = Timer.periodic(const Duration(seconds: 1), (timer) {
      setState(() {});
    });
  }

  @override
  void dispose() {
    _timer.cancel();
    super.dispose();
  }

  Future<void> _loadStartDate() async {
    final prefs = await SharedPreferences.getInstance();
    final startDateMillis = prefs.getInt('start_date');
    setState(() {
      _startDate = startDateMillis != null
          ? DateTime.fromMillisecondsSinceEpoch(startDateMillis)
          : DateTime.now();
    });
  }

  Future<void> _resetStartDate() async {
    final prefs = await SharedPreferences.getInstance();
    final now = DateTime.now();
    await prefs.setInt('start_date', now.millisecondsSinceEpoch);
    setState(() {
      _startDate = now;
    });
  }

  String _formatDuration() {
    if (_startDate == null) return '';
    final duration = DateTime.now().difference(_startDate!);

    if (duration.inDays >= 365) {
      final years = duration.inDays ~/ 365;
      final months = (duration.inDays % 365) ~/ 30;
      final days = (duration.inDays % 365) % 30;
      return '$years:${months.toString().padLeft(2, '0')}:${days.toString().padLeft(2, '0')}';
    } else if (duration.inDays >= 30) {
      final months = duration.inDays ~/ 30;
      final days = duration.inDays % 30;
      final hours = duration.inHours % 24;
      return '$months:${days.toString().padLeft(2, '0')}:${hours.toString().padLeft(2, '0')}';
    } else {
      final days = duration.inDays;
      final hours = duration.inHours % 24;
      final minutes = duration.inMinutes % 60;
      final seconds = duration.inSeconds % 60;
      return '${days.toString().padLeft(2, '0')}:${hours.toString().padLeft(2, '0')}:${minutes.toString().padLeft(2, '0')}:${seconds.toString().padLeft(2, '0')}';
    }
  }

  Future<void> _showResetConfirmation() async => showDialog<void>(
      context: context,
      barrierDismissible: false,
      builder: (BuildContext context) {
        return AlertDialog(
          title: const Text('Reset Counter'),
          content: const Text('Are you sure you want to reset the counter?'),
          actions: <Widget>[
            TextButton(
              child: const Text('Cancel'),
              onPressed: () {
                Navigator.of(context).pop();
              },
            ),
            TextButton(
              child: const Text('Reset'),
              onPressed: () {
                _resetStartDate();
                Navigator.of(context).pop();
              },
            ),
          ],
        );
      },
    );

  @override
  Widget build(BuildContext context) => Scaffold(
      body: GestureDetector(
        onTap: _showResetConfirmation,
        child: Container(
          color: MyColor.secondary,
          child: Center(
            child: Column(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                Text.rich(
                  TextSpan(
                    children: [
                      TextSpan(
                        text: 'Let',
                        style: GoogleFonts.dancingScript(
                          color: MyColor.accent,
                          fontSize: 50,
                        ),
                      ),
                      TextSpan(
                        text: 'it',
                        style: GoogleFonts.dancingScript(
                          color: MyColor.primary,
                          fontSize: 50,
                        ),
                      ),
                      TextSpan(
                        text: 'Go',
                        style: GoogleFonts.dancingScript(
                          color: MyColor.accent,
                          fontSize: 50,
                        ),
                      ),
                    ],
                  ),
                ),
                Text(
                  _formatDuration(),
                  style: GoogleFonts.chivoMono(
                    fontSize: 48,
                    fontWeight: FontWeight.bold,
                    color: MyColor.accent,
                  ),
                ),
              ],
            ),
          ),
        ),
      ),
    );
}