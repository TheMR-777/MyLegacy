Set objShell = CreateObject("Shell.Application")

strProgram = "program.exe"
strProgramPath = "C:\path\to\program\exe\file" & strProgram

Do
    strQuery = "SELECT * FROM Win32_Process WHERE Name = '" & strProgram & "'"
    Set colProcesses = GetObject("winmgmts:\\.\root\cimv2").ExecQuery(strQuery)

    If colProcesses.Count = 0 Then
        objShell.ShellExecute strProgramPath, "", "", "runas", 1
    End If

    WScript.Sleep(1 * 60 * 1000)
Loop
