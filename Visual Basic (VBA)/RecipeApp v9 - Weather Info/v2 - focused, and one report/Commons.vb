Module Utility
	Public Class UserQuery
		Private Shared ReadOnly DatabaseTable = "dbo.Credentials"
		Private Shared ReadOnly ConnectionStr = Configuration.ConfigurationManager.ConnectionStrings("TheCS").ConnectionString
		Private Shared ReadOnly DB_Connection = New SqlClient.SqlConnection(ConnectionStr)
		Private Shared DatabaseQuery As SqlClient.SqlCommand

		Public Shared Function Login(ByRef Mail As TextBox, ByRef Pass As TextBox) As String

			Dim Query = $"SELECT [First Name] FROM {DatabaseTable} WHERE [Email] = @mail AND [Password] = @pass"

			DB_Connection.Open()
			DatabaseQuery = New SqlClient.SqlCommand(Query, DB_Connection)
			DatabaseQuery.Parameters.AddWithValue("@mail", Mail.Text)
			DatabaseQuery.Parameters.AddWithValue("@pass", Pass.Text)

			Dim Result = DatabaseQuery.ExecuteScalar()
			DB_Connection.Close()

			Return Result

		End Function

		Public Shared Function Register(
				ByRef F_Name As TextBox,
				ByRef L_Name As TextBox,
				ByRef Email As TextBox,
				ByRef Password As TextBox
			) As Boolean

			Dim Query As String
			Dim Result As Object

			' Check if the Email is already registered
			With DB_Connection
				Query = $"SELECT [First Name] FROM {DatabaseTable} WHERE [Email] = @mail"

				.Open()
				DatabaseQuery = New SqlClient.SqlCommand(Query, DB_Connection)
				DatabaseQuery.Parameters.AddWithValue("@mail", Email.Text)
				Result = DatabaseQuery.ExecuteScalar()
				.Close()

				If Result IsNot Nothing Then
					Return False
				End If
			End With

			' Register the new User
			With DB_Connection
				Query = $"INSERT INTO {DatabaseTable} VALUES (@Email, @F_Name, @L_Name, @Password)"

				.Open()
				DatabaseQuery = New SqlClient.SqlCommand(Query, DB_Connection)
				DatabaseQuery.Parameters.AddWithValue("@F_Name", F_Name.Text)
				DatabaseQuery.Parameters.AddWithValue("@L_Name", L_Name.Text)
				DatabaseQuery.Parameters.AddWithValue("@Email", Email.Text)
				DatabaseQuery.Parameters.AddWithValue("@Password", Password.Text)

				Result = DatabaseQuery.ExecuteNonQuery()
				.Close()

				Return Result
			End With

		End Function

	End Class

	Public ReadOnly IsDebug = True
	Public ReadOnly Delimiters As Char() = {",", Environment.NewLine}

	Public Function EncodeImage(ByRef image As Image) As String

		If image IsNot Nothing Then
			Using stream = New IO.MemoryStream()
				image.Save(stream, image.RawFormat)
				Return Convert.ToBase64String(stream.ToArray())
			End Using
		Else
			Return ""
		End If

	End Function

	Public Function DecodeImage(ByRef imageBase64 As String) As Image

		Dim imageBytes = Convert.FromBase64String(imageBase64)
		Return Image.FromStream(New IO.MemoryStream(imageBytes))

	End Function

End Module