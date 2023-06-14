﻿Imports System.Data
Imports System.Data.SqlClient

Public Class MyApp
	Private Function MakeQuery(Name As String, Roll As String, Note As String) As String

		' Format:
		' INSERT INTO [dbo].[MyTable2]
		' (
		'	[Name],
		'	[Roll Number],
		'	[Notes]
		' )
		' VALUES

		Return String.Format(
			"INSERT INTO [dbo].[MyTable2] ([Name], [Roll Number], [Notes]) VALUES ('{0}', '{1}', '{2}')",
			Name, Roll, Note
		)

	End Function

	Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

		Dim The_Connection = "Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=MyDB;Integrated Security=True"
		Dim SQL_Connection As New SqlConnection(The_Connection)
		Dim My_SQL_Command As New SqlCommand(
			MakeQuery(T_Name.Text, T_Roll.Text, T_Note.Text),
			SQL_Connection
		)

		SQL_Connection.Open()
		My_SQL_Command.ExecuteNonQuery()
		SQL_Connection.Close()

	End Sub
End Class
