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

  List<(String, String)> _formatDuration() {
    if (_startDate == null) return [];
    final duration = DateTime.now().difference(_startDate!);

    List<(String, String)> parts = [];

    if (duration.inDays >= 365) {
      final years = duration.inDays ~/ 365;
      final months = (duration.inDays % 365) ~/ 30;
      final days = (duration.inDays % 365) % 30;
      parts.add((years.toString(), 'y'));
      if (months > 0) parts.add((months.toString(), 'm'));
      if (days > 0) parts.add((days.toString(), 'd'));
    } else if (duration.inDays >= 30) {
      final months = duration.inDays ~/ 30;
      final days = duration.inDays % 30;
      final hours = duration.inHours % 24;
      parts.add((months.toString(), 'm'));
      if (days > 0) parts.add((days.toString(), 'd'));
      if (hours > 0) parts.add((hours.toString(), 'h'));
    } else if (duration.inDays > 0) {
      final days = duration.inDays;
      final hours = duration.inHours % 24;
      final minutes = duration.inMinutes % 60;
      parts.add((days.toString(), 'd'));
      if (hours > 0) parts.add((hours.toString(), 'h'));
      if (minutes > 0) parts.add((minutes.toString(), 'm'));
    } else if (duration.inHours > 0) {
      final hours = duration.inHours;
      final minutes = duration.inMinutes % 60;
      final seconds = duration.inSeconds % 60;
      parts.add((hours.toString(), 'h'));
      if (minutes > 0) parts.add((minutes.toString(), 'm'));
      if (seconds > 0) parts.add((seconds.toString(), 's'));
    } else if (duration.inMinutes > 0) {
      final minutes = duration.inMinutes;
      final seconds = duration.inSeconds % 60;
      parts.add((minutes.toString(), 'm'));
      if (seconds > 0) parts.add((seconds.toString(), 's'));
    } else {
      parts.add((duration.inSeconds.toString(), 's'));
    }

    return parts;
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
              const SizedBox(height: 20),
              Text.rich(
                TextSpan(
                  children: _formatDuration().expand((part) => [
                    TextSpan(
                      text: part.$1,
                      style: GoogleFonts.chivoMono(
                        fontSize: 48,
                        fontWeight: FontWeight.bold,
                        color: MyColor.accent,
                      ),
                    ),
                    TextSpan(
                      text: part.$2,
                      style: GoogleFonts.chivoMono(
                        fontSize: 24,
                        fontWeight: FontWeight.normal,
                        color: MyColor.primary,
                      ),
                    ),
                    const TextSpan(text: ' '),
                  ]).toList(),
                ),
              ),
            ],
          ),
        ),
      ),
    ),
  );
}