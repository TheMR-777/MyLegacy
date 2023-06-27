Public Class Login

	Public Sub PreLoad(ByRef Email As String, ByRef Password As String)

		Me.Email.Text = Email
		Me.Password.Text = Password

	End Sub


	Private Sub Login_Load(sender As Object, e As EventArgs) Handles MyBase.Load

		If IsDebug Then
			Email.Text = "mail"
			Password.Text = "pass"
		End If

	End Sub

	Private Sub Register_Click(sender As Object, e As EventArgs) Handles Register.Click

		Dim Registration = New Registration() With {
			.TheLogin = Me
		}
		Registration.Show()
		Hide()

	End Sub


	Private Sub LoginButton_Click(sender As Object, e As EventArgs) Handles LoginButton.Click

		For Each Field In {Email, Password}
			If Field.Text = "" Then
				MessageBox.Show(
					$"The Field of {Field.Name} is empty",
					"Empty Field detected",
					MessageBoxButtons.OK,
					MessageBoxIcon.Information
				)
				Return
			End If
		Next

		Dim User As String = UserQuery.Login(Email, Password)

		If User Is Nothing Then
			MessageBox.Show(
				"The Email or Password is incorrect",
				"Incorrect Field detected",
				MessageBoxButtons.OK,
				MessageBoxIcon.Information
			)
			Return
		End If

		Dim TheRecipeApp = New RecipeApp(User)
		RecipeApp.TheLogin = Me
		TheRecipeApp.Show()
		Hide()

	End Sub

	Private Sub CancelButton_Click(sender As Object, e As EventArgs) Handles CancelButton.Click

		Close()

	End Sub

End Class